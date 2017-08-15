log 'Starting addition of processing to Default resource pool'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

require 'net/http'
require 'json'

# Check Resource Pool Exists
log "Querying for 'Default' resource pool artifact id."
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = "An unexpected error occured when querying for 'Default' resource pool"
  log error_message
  raise error_message
end
response_json = JSON.parse(response.body)
if response_json['TotalCount'] > 0
  node.run_state['default_resource_pool_artifact_id'] = response_json['Results'][0]['Artifact']['ArtifactID']
  log "default_resource_pool_artifact_id = #{node.run_state['default_resource_pool_artifact_id']}"
elsif response_json['TotalCount'] == 0
  log "Coundn't find 'Default' resource pool artifact id."
  raise "Unexpected Failure in check for 'Default' Resource Pool."
end


# Get ResourceServerType Choices
log 'Querying for ResourceServerType Choices.'
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/GetResourceServerTypeChoicesAsync"
uri = URI(url)
http = Net::HTTP.new(uri.host, uri.port)
request = Net::HTTP::Post.new(uri.request_uri)
request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
request['X-CSRF-Header'] = ' '
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = 'An unexpected error occured when querying for ResourceServerType Choices.'
  log error_message
  raise error_message
end
response_json = JSON.parse(response.body)
response_json.each do |type|
  if type['Name'] == 'Worker Manager Server'
    node.run_state['resource_server_types_choice_processing_artifact_id'] = type['ArtifactID']
    log "resource_server_types_choice_processing_artifact_id = #{node.run_state['resource_server_types_choice_processing_artifact_id']}"
  end
end


# Add Processing Server to Default Resource Pool
log "Adding Processing Server to 'Default' Resource Pool."
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = "An unexpected error occured when adding Processing Server to 'Default' Resource Pool."
  log error_message
  raise error_message
end
if is_http_request_success
  log "Added Processing Server to 'Default' Resource Pool."
end


# Query Processing location
log 'Querying for Processing location.'
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = 'An unexpected error occured when querying Processing location.'
  log error_message
  raise error_message
end
response_json = JSON.parse(response.body)
node.run_state['processing_location_artifact_id'] = response_json['Results'][0]['Artifact ID']
log "processing_location_artifact_id = #{node.run_state['processing_location_artifact_id']}"


# Add Processing Server Location to Default Resource Pool
log "Adding Processing Server Location to 'Default' Resource Pool."
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = "An unexpected error occured when adding Processing Server Location to 'Default' Resource Pool."
  log error_message
  raise error_message
end
if is_http_request_success
  log "Added Processing Server Location to 'Default' Resource Pool."
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished addition of processing to Default resource pool'