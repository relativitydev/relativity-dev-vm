custom_log 'custom_log' do msg 'Starting creation of worker manager server' end
  start_time = DateTime.now
  custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
  
  require 'net/http'
  require 'json'
  
  # Initialize Run State Objects if not already
  node.run_state['processing_server_artifact_id'] = nil
  
  # Check Processing Exists
  custom_log 'custom_log' do msg 'Checking if Processing Exists.' end
  response = ProcessingHelper.query_processing_worker_manager_servers(node['windows']['hostname'], node['relativity']['admin']['login'], node['relativity']['admin']['password'], node['windows']['hostname'])
  response_json = JSON.parse(response.body)
  unless response_json['Results'].empty?
    node.run_state['processing_server_artifact_id'] = response_json['Results'][0]['Artifact']['ArtifactID']
    custom_log 'custom_log' do msg "Processing Server exists. processing_server_artifact_id = #{node.run_state['processing_server_artifact_id']}" end
  end
  
  # Create Processing Server if it not already exists
  if node.run_state['processing_server_artifact_id'].nil?
    custom_log 'custom_log' do msg 'Processing Server does not already exists. Create new Processing Server.' end
    url = "http://#{node['windows']['hostname']}/Relativity.Rest/api/Relativity.Services.ResourceServer.IResourceServerModule/Worker%20Manager%20Resource%20Server%20Manager/CreateSingleAsync"
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
          "Name": "#{node['windows']['hostname']}",
          "IsDefault": true,
          "InventoryPriority": 100,
          "DiscoveryPriority": 100,
          "PublishPriority": 100,
          "ImageOnTheFlyPriority": 1,
          "MassImagingPriority": 100,
          "SingleSaveAsPDFPriority": 100,
          "MassSaveAsPDFPriority": 100,
          "URL": "#{node['windows']['hostname']}"
        }
      }
      QUERY
    )
    request.body = request_body_json.to_json
    error_message = 'An unexpected error occured when creating new Processing Server.'
    response = RetryHelper.execute_rest_call(http, request, 3, error_message)
    node.run_state['processing_server_artifact_id'] = response.body
    custom_log 'custom_log' do msg "New Processing Server created. processing_server_artifact_id = #{node.run_state['processing_server_artifact_id']}" end
  
    # Wait until Processing Server can be queried
    processing_server_queryable = false
    psq_timer = 0
    psq_interval = 10
    psq_max_time = 300
  
    while processing_server_queryable == false && psq_timer < psq_max_time
      # Sleep
      sleep(psq_interval)
  
      # Check Processing Exists'
      response = ProcessingHelper.query_processing_worker_manager_servers(node['windows']['hostname'], node['relativity']['admin']['login'], node['relativity']['admin']['password'], node['windows']['hostname'])
      response_json = JSON.parse(response.body)
      unless response_json['Results'].empty?
        processing_server_queryable = true
      end
  
      # Increment Interval
      psq_timer += psq_interval
    end
    
    if psq_interval >= psq_max_time
      raise "Timed out while waiting for newly created processing server to become queryable"
    end
    if processing_server_queryable == false
      raise "Unable to find newly created processing server"
    end
  end
  
  end_time = DateTime.now
  custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
  custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
  custom_log 'custom_log' do msg "Finished creation of worker manager server\n\n\n" end
  