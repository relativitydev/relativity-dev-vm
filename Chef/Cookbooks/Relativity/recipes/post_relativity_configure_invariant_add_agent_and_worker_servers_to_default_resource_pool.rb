custom_log 'custom_log' do msg 'Starting addition of agent and worker servers to default resource pool' end
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
error_message = "An unexpected error occured when querying for 'Default' resource pool."
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
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
  if type['Name'] == 'Agent'
    node.run_state['resource_server_types_choice_agent_artifact_id'] = type['ArtifactID']
    custom_log 'custom_log' do msg "resource_server_types_choice_agent_artifact_id = #{node.run_state['resource_server_types_choice_agent_artifact_id']}" end
  end
  if type['Name'] == 'Worker'
    node.run_state['resource_server_types_choice_worker_artifact_id'] = type['ArtifactID']
    custom_log 'custom_log' do msg "resource_server_types_choice_worker_artifact_id = #{node.run_state['resource_server_types_choice_worker_artifact_id']}" end
  end
end

# Get All ResourceServers
custom_log 'custom_log' do msg 'Querying for All ResourceServers.' end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourceServer.IResourceServerModule/Resource%20Server%20Manager/QueryAsync"
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
      "Condition": "",
      "Sorts": []
    }
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = 'An unexpected error occured when querying for All ResourceServers.'
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
response_json = JSON.parse(response.body)
if response_json['TotalCount'] > 0
  response_json['Results'].each do |ser|
    if ser['Artifact']['ServerType']['Name'] == 'Agent'
      node.run_state['agent_resource_server_artifact_id'] = ser['Artifact']['ArtifactID']
      custom_log 'custom_log' do msg "agent_resource_server_artifact_id = #{node.run_state['agent_resource_server_artifact_id']}" end
    end
    if ser['Artifact']['ServerType']['Name'] == 'Worker'
      node.run_state['worker_resource_server_artifact_id'] = ser['Artifact']['ArtifactID']
      custom_log 'custom_log' do msg "worker_resource_server_artifact_id = #{node.run_state['worker_resource_server_artifact_id']}" end
    end
  end
elsif response_json['TotalCount'] == 0
  custom_log 'custom_log' do msg 'Cound not retrieve a list of all resource servers.' end
  raise 'Unexpected Failure when querying for all resource servers.'
end

# Add Agent ResourceServer to Default Resource Pool
custom_log 'custom_log' do msg "Adding Agent Server to 'Default' Resource Pool." end
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
      "ArtifactID": #{node.run_state['agent_resource_server_artifact_id']},
      "ServerType": {
        "ArtifactID": #{node.run_state['resource_server_types_choice_agent_artifact_id']}
      }
    },
    "resourcePool": {
      "ArtifactID": #{node.run_state['default_resource_pool_artifact_id']}
    }
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = "An unexpected error occured when adding Agent ResourceServer to 'Default' Resource Pool."
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
if is_http_request_success
  custom_log 'custom_log' do msg "Added Agent ResourceServer to 'Default' Resource Pool." end
end

# Add Worker ResourceServer to Default Resource Pool
custom_log 'custom_log' do msg "Adding Worker Server to 'Default' Resource Pool." end
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
      "ArtifactID": #{node.run_state['worker_resource_server_artifact_id']},
      "ServerType": {
        "ArtifactID": #{node.run_state['resource_server_types_choice_worker_artifact_id']}
      }
    },
    "resourcePool": {
      "ArtifactID": #{node.run_state['default_resource_pool_artifact_id']}
    }
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = "An unexpected error occured when adding Worker ResourceServer to 'Default' Resource Pool."
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
if is_http_request_success
  custom_log 'custom_log' do msg "Added Worker ResourceServer to 'Default' Resource Pool." end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished addition of agent and worker servers to default resource pool\n\n\n" end
