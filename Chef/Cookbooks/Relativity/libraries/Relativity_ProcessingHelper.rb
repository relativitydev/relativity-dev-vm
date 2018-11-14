class Chef::Recipe::ProcessingHelper
    
      include Chef::Mixin::ShellOut
    
      def self.query_processing_worker_manager_servers(domain, username, password, name_of_worker_manager_server)
        result = nil
    
        # Check Processing Exists
        url = "http://#{domain}/Relativity.Rest/api/Relativity.Services.ResourceServer.IResourceServerModule/Resource%20Server%20Manager/QueryAsync"
        uri = URI(url)
        http = Net::HTTP.new(uri.host, uri.port)
        request = Net::HTTP::Post.new(uri.request_uri)
        request.basic_auth(username, password)
        request['X-CSRF-Header'] = ' '
        request['Content-Type'] = 'application/json'
        request_body_json = JSON.parse(
          <<-QUERY
          {
            "Query": {
              "Condition": "'ServerType' == 'Worker Manager Server' AND 'Name' == '#{name_of_worker_manager_server}'",
              "Sorts": []
            }
          }
          QUERY
        )
        request.body = request_body_json.to_json
        error_message = 'An unexpected error occured when Checking if Processing Exists.'
        result = Chef::Recipe::RetryHelper.execute_rest_call(http, request, 3, error_message)
        result
      end

      def self.query_resourcepool_processing_source_locations(domain, username, password, resource_pool_artifact_id)
        result = nil
    
        url = "http://#{domain}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/GetProcessingSourceLocationsAsync"
        uri = URI(url)
        http = Net::HTTP.new(uri.host, uri.port)
        request = Net::HTTP::Post.new(uri.request_uri)
        request.basic_auth(username, password)
        request['X-CSRF-Header'] = ' '
        request['Content-Type'] = 'application/json'
        request_body_json = JSON.parse(
          <<-QUERY
          {
            "resourcePool": {
              "ArtifactID": #{resource_pool_artifact_id}
            }
          }
          QUERY
        )
        request.body = request_body_json.to_json
        error_message = "An unexpected error occured when checking to see if Processing Server Location already exists in 'Default' Resource Pool."
        result = Chef::Recipe::RetryHelper.execute_rest_call(http, request, 3, error_message)
        result
      end

      def self.query_resourcepool_resource_servers(domain, username, password, resource_pool_artifact_id)
        result = nil
    
        url = "http://#{domain}/Relativity.Rest/api/Relativity.Services.ResourcePool.IResourcePoolModule/Resource%20Pool%20Service/RetrieveResourceServersAsync"
        uri = URI(url)
        http = Net::HTTP.new(uri.host, uri.port)
        request = Net::HTTP::Post.new(uri.request_uri)
        request.basic_auth(username, password)
        request['X-CSRF-Header'] = ' '
        request['Content-Type'] = 'application/json'
        request_body_json = JSON.parse(
          <<-QUERY
          {
            "resourcePool": {
              "ArtifactID": #{resource_pool_artifact_id}
            }
          }
          QUERY
        )
        request.body = request_body_json.to_json
        error_message = "An unexpected error occured when querying resource pool resource servers."
        result = Chef::Recipe::RetryHelper.execute_rest_call(http, request, 3, error_message)
        result
      end

    end
    