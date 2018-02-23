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
  custom_log 'custom_log' do msg "processing_location_artifact_id = #{node.run_state['processing_location_artifact_id']}" end
else
  raise "Unable to find Processing Location"
end

# Check if Processing Server Location already added to Default Resource Pool
processing_server_already_exists_in_resourcepool = false
custom_log 'custom_log' do msg "Checking to see if Processing Server Location already exists in 'Default' Resource Pool." end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/GetProcessingSourceLocationsAsync"
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
    }
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = "An unexpected error occured when checking to see if Processing Server Location already exists in 'Default' Resource Pool."
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
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
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished addition of processing to Default resource pool\n\n\n" end
