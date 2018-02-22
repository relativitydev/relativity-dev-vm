custom_log 'custom_log' do msg 'Starting update worker server for processing' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# uses kepler to query resource server table and get the worker server ArtifactID
custom_log 'custom_log' do msg 'using kepler to query resource server table and get the worker server ArtifactID.' end
node.run_state['agent'] = {}
node.run_state['agent']['servers'] = {}
node.run_state['agent']['servers']['agent'] = {}
node.run_state['agent']['servers']['worker'] = {}
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
    "Query": {}
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = 'An unexpected error occured when using kepler to query resource server table and get the worker server ArtifactID'
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
response_json = JSON.parse(response.body)
node.run_state['agent']['agent_server'] = response_json
node.run_state['agent']['agent_server']['Results'].each do |key|
  custom_log 'custom_log' do msg "Server name is #{key['Artifact']['ServerType']['Name']} and its Artifact ID is #{key['Artifact']['ServerType']['ArtifactID']}" end
  if key['Artifact']['Name'].casecmp(node['windows']['hostname']) == 0 && key['Artifact']['ServerType']['Name'] == 'Worker'
    custom_log 'custom_log' do msg key['Artifact']['ArtifactID'] end
    node.run_state['agent']['servers']['worker']['artifact_id'] = key['Artifact']['ArtifactID']
    custom_log 'custom_log' do msg "node.run_state['agent']['servers']['worker']['artifact_id'] = #{node.run_state['agent']['servers']['worker']['artifact_id']}" end
  else
    next
  end
end
if node.run_state['agent']['servers']['worker']['artifact_id'].nil?
  raise 'Worker artifact ID not found!'
end

# Kepler call to enable\license a worker
custom_log 'custom_log' do msg 'Kepler call to enable\license a worker' end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.WorkerStatus.IWorkerStatusModule/WorkerStatus/EnableProcessingOnWorkerAsync"
uri = URI(url)
http = Net::HTTP.new(uri.host, uri.port)
request = Net::HTTP::Post.new(uri.request_uri)
request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
request['X-CSRF-Header'] = ' '
request['Content-Type'] = 'application/json'
request_body_json = JSON.parse(
  <<-QUERY
  {
    "workerArtifactId": #{node.run_state['agent']['servers']['worker']['artifact_id']}
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = 'An unexpected error occured when using Kepler call to enable\license a worker'
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
custom_log 'custom_log' do msg response.code end

# Kepler call to add processing designated work to a worker
custom_log 'custom_log' do msg 'Kepler call to add processing designated work to a worker' end
url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.WorkerStatus.IWorkerStatusModule/WorkerStatus/UpdateCategoriesOnWorkerAsync"
uri = URI(url)
http = Net::HTTP.new(uri.host, uri.port)
request = Net::HTTP::Post.new(uri.request_uri)
request.basic_auth(node['relativity']['admin']['login'], node['relativity']['admin']['password'])
request['X-CSRF-Header'] = ' '
request['Content-Type'] = 'application/json'
request_body_json = JSON.parse(
  <<-QUERY
  {
    "workerArtifactId": #{node.run_state['agent']['servers']['worker']['artifact_id']},
    "categories": [
      "NativeImaging",
      "BasicImaging",
      "SaveAsPDF",
      "Processing"
    ]
  }
  QUERY
)
request.body = request_body_json.to_json
error_message = 'An unexpected error occured when using Kepler call to add processing designated work to a worker'
response = RetryHelper.execute_rest_call(http, request, 3, error_message)
custom_log 'custom_log' do msg response.code end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished update worker server for processing' end
