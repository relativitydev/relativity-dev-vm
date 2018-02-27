custom_log 'custom_log' do msg 'Starting addition of processing to Default resource pool' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

require 'net/http'
require 'json'

# Check Resource Pool Exists
custom_log 'custom_log' do msg "Querying for 'Default' resource pool artifact id." end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/QueryAsync"
uri = URI(url)
http = Net::HTTP.new(uri.host, uri.port)
request = Net::HTTP::Post.new(uri.request_uri)
request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
request['X-CSRF-Header'] = ' '
request['Content-Type'] = 'application/json'
request_body_json = JSON.parse(
  <<-QUERY
  {
    "Query": {
      "Condition": "'Name' STARTSWITH 'Default'",
      "Sorts": []
    }
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = "An unexpected error occured when querying for 'Default' resource pool"
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
response_json = JSON.parse(response.body)
if response_json['TotalCount'] > 0
  node.run_state['default_resource_pool_artifact_id'] = response_json['Results'][0]['Artifact']['ArtifactID']
  custom_log 'custom_log' do msg "default_resource_pool_artifact_id = #{node.run_state['default_resource_pool_artifact_id']}" end
elsif response_json['TotalCount'] == 0
  custom_log 'custom_log' do msg "Coundn't find 'Default' resource pool artifact id." end
  raise "Unexpected Failure in check for 'Default' Resource Pool."
end

# Get ResourceServerType Choices
custom_log 'custom_log' do msg 'Querying for ResourceServerType Choices.' end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/GetResourceServerTypeChoicesAsync"
uri = URI(url)
http = Net::HTTP.new(uri.host, uri.port)
request = Net::HTTP::Post.new(uri.request_uri)
request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
request['X-CSRF-Header'] = ' '
error_message = 'An unexpected error occured when querying for ResourceServerType Choices.'
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
response_json = JSON.parse(response.body)
response_json.each do |type|
  if type['Name'] == 'Worker Manager Server'
    node.run_state['resource_server_types_choice_processing_artifact_id'] = type['ArtifactID']
    custom_log 'custom_log' do msg "resource_server_types_choice_processing_artifact_id = #{node.run_state['resource_server_types_choice_processing_artifact_id']}" end
  end
end

if node.run_state['resource_server_types_choice_processing_artifact_id'] == nil
  raise "Unable to find Worker Manager Server"
end

# Add Processing Server to Default Resource Pool
custom_log 'custom_log' do msg "Adding Processing Server to 'Default' Resource Pool." end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/AddServerAsync"
uri = URI(url)
http = Net::HTTP.new(uri.host, uri.port)
request = Net::HTTP::Post.new(uri.request_uri)
request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
request['X-CSRF-Header'] = ' '
request['Content-Type'] = 'application/json'
request_body_json = JSON.parse(
  <<-QUERY
  {
    "resourceServer": {
      "ArtifactID": #{node.run_state['processing_server_artifact_id']},
      "ServerType": {
        "ArtifactID": #{node.run_state['resource_server_types_choice_processing_artifact_id']}
      }
    },
    "resourcePool": {
      "ArtifactID": #{node.run_state['default_resource_pool_artifact_id']}
    }
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = "An unexpected error occured when adding Processing Server to 'Default' Resource Pool."
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
if is_http_request_success
  custom_log 'custom_log' do msg "Added Processing Server to 'Default' Resource Pool." end
end

# Processing Source Location should have been created via powershell, query the choice list for 5 minutes and wait until it is available
processing_location_exists = false
ple_timer = 0
ple_interval = 10
ple_max_time = 300

while processing_location_exists == false && ple_timer < ple_max_time
  # Sleep
  sleep(ple_interval)
    
  # Query Processing location
  custom_log 'custom_log' do msg 'Querying for Processing location.' end
  url = "http://#{node['windows']['hostname']}/Relativity.REST/Relativity/Choice/QueryResult"
  uri = URI(url)
  http = Net::HTTP.new(uri.host, uri.port)
  request = Net::HTTP::Post.new(uri.request_uri)
  request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
  request['X-CSRF-Header'] = ' '
  request['Content-Type'] = 'application/json'
  request_body_json = JSON.parse(
    <<-QUERY
    {
      "condition": " 'Choice Type ID' == 1000017",
      "fields": [
        "*"
      ]
    }
    QUERY
  )
  request.body = request_body_json.to_json
  error_message = 'An unexpected error occured when querying Processing location.'
  response = RetryHelper.execute_rest_call(http, request, 3, error_message)
  response_json = JSON.parse(response.body)
  if response_json['TotalResultCount'] > 0
    node.run_state['processing_location_artifact_id'] = response_json['Results'][0]['Artifact ID']
    processing_location_exists = true
    custom_log 'custom_log' do msg "processing_location_artifact_id = #{node.run_state['processing_location_artifact_id']}" end
  end

  # Increment Interval
  ple_timer += ple_interval
end

if ple_timer >= ple_max_time
  custom_log 'custom_log' do msg "Timed out while querying processing source location" end
  raise "Timed out while querying processing source location"
end
if processing_location_exists == false
  custom_log 'custom_log' do msg "Unable to find Processing Location" end
  raise "Unable to find Processing Location"
end

# Check if Processing Server Location already added to Default Resource Pool
processing_server_already_exists_in_resourcepool = false
custom_log 'custom_log' do msg "Checking to see if Processing Server Location already exists in 'Default' Resource Pool." end
response = ProcessingHelper.query_resourcepool_processing_source_locations(node['windows']['hostname'], node['relativity']['admin']['login'], node['relativity']['admin']['password'], node.run_state['default_resource_pool_artifact_id'])
response_json = JSON.parse(response.body)
if response_json.length > 0 && response_json[0]['ArtifactID'] == node.run_state['processing_location_artifact_id'].to_i
  processing_server_already_exists_in_resourcepool = true
  custom_log 'custom_log' do msg "Processing Server Location already added to Default Resource Pool" end
end

if processing_server_already_exists_in_resourcepool == false
  # Add Processing Server Location to Default Resource Pool
  custom_log 'custom_log' do msg "Adding Processing Server Location to 'Default' Resource Pool." end
  url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/AddProcessingSourceLocationAsync"
  uri = URI(url)
  http = Net::HTTP.new(uri.host, uri.port)
  request = Net::HTTP::Post.new(uri.request_uri)
  request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
  request['X-CSRF-Header'] = ' '
  request['Content-Type'] = 'application/json'
  request_body_json = JSON.parse(
  <<-QUERY
  {
    "resourcePool": {
    "ArtifactID": #{node.run_state['default_resource_pool_artifact_id']}
    },
    "ProcessingSourceLocation": {
    "ArtifactID": #{node.run_state['processing_location_artifact_id']}
    }
  }
  QUERY
  )
  request.body = request_body_json.to_json
  error_message = "An unexpected error occured when adding Processing Server Location to 'Default' Resource Pool. "
  response = RetryHelper.execute_rest_call(http, request, 3, error_message)
  is_http_request_success = (response.is_a? Net::HTTPSuccess)
  if is_http_request_success
    custom_log 'custom_log' do msg "Added Processing Server Location to 'Default' Resource Pool." end
  end

  # Processing Sever Location should have been created above, query for 5 minutes and wait until it is available
  processing_server_location_queryable = false
  pslq_timer = 0
  pslq_interval = 10
  pslq_max_time = 300

  while processing_server_location_queryable == false && pslq_timer < pslq_max_time
    # Sleep
    sleep(pslq_interval)

    custom_log 'custom_log' do msg "Checking to see if Processing Server Location already exists in 'Default' Resource Pool." end
    response = ProcessingHelper.query_resourcepool_processing_source_locations(node['windows']['hostname'], node['relativity']['admin']['login'], node['relativity']['admin']['password'], node.run_state['default_resource_pool_artifact_id'])
    response_json = JSON.parse(response.body)
    if response_json.length > 0 && response_json[0]['ArtifactID'] == node.run_state['processing_location_artifact_id'].to_i
      processing_server_location_queryable = true
      custom_log 'custom_log' do msg "Succesfully queried Processing Server Location in default resource pool" end
    end

    # Increment Interval
    pslq_timer += pslq_interval
  end
  if pslq_timer >= pslq_max_time
    custom_log 'custom_log' do msg "Timed out while querying processing server location in resource pool" end
    raise "Timed out while querying processing server location in resource pool"
  end
  if processing_server_location_queryable == false
    custom_log 'custom_log' do msg "Unable to find Processing Server Location in resource pool" end
    raise "Unable to find Processing Server Location in resource pool"
  end
end



end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished addition of processing to Default resource pool\n\n\n" end
