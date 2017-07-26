log 'Starting update worker server for processing'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# uses kepler to query resource server table and get the worker server ArtifactID
log 'using kepler to query resource server table and get the worker server ArtifactID.'
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = 'An unexpected error occured when using kepler to query resource server table and get the worker server ArtifactID'
  log error_message
  raise error_message
end
response_json = JSON.parse(response.body)
node.run_state['agent']['agent_server'] = response_json
node.run_state['agent']['agent_server']['Results'].each do |key|
  log "Server name is #{key['Artifact']['ServerType']['Name']} and its Artifact ID is #{key['Artifact']['ServerType']['ArtifactID']}"
  if key['Artifact']['Name'].casecmp(node['windows']['hostname']) == 0 && key['Artifact']['ServerType']['Name'] == 'Worker'
    log key['Artifact']['ArtifactID']
    node.run_state['agent']['servers']['worker']['artifact_id'] = key['Artifact']['ArtifactID']
    log "node.run_state['agent']['servers']['worker']['artifact_id'] = #{node.run_state['agent']['servers']['worker']['artifact_id']}"
  else
    next
  end
end
if node.run_state['agent']['servers']['worker']['artifact_id'].nil?
  raise 'Worker artifact ID not found!'
end


# Kepler call to enable\license a worker
log 'Kepler call to enable\license a worker'
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = 'An unexpected error occured when using Kepler call to enable\license a worker'
  log error_message
  raise error_message
end
log response.code


# Kepler call to add processing designated work to a worker
log 'Kepler call to add processing designated work to a worker'
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
response = http.request(request)
is_http_request_success = (response.is_a? Net::HTTPSuccess)
unless is_http_request_success
  error_message = 'An unexpected error occured when using Kepler call to add processing designated work to a worker'
  log error_message
  raise error_message
end
log response.code

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished update worker server for processing'
