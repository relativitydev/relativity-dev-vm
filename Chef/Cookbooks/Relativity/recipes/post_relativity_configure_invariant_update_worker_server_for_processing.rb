custom_log 'custom_log' do msg 'Starting update worker server for processing' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

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
    "workerArtifactId": #{node.run_state['worker_resource_server_artifact_id']}
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
    "workerArtifactId": #{node.run_state['worker_resource_server_artifact_id']},
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
custom_log 'custom_log' do msg "Finished update worker server for processing\n\n\n" end
