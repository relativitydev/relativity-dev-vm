custom_log 'custom_log' do msg 'Starting push resource file recipe' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Iteration variables
script_count = 0
script_number_placeholder = '<?>'
script_name = 'powershell_post_relativity_push_resource'
script_extension = '.ps1'

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
push_resource_file_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], "#{script_name}#{script_number_placeholder}#{script_extension}"))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

for resource in node['relativity_resource_files_to_push']
    
    # replace placeholder with the loop iteration number
    script_fullpath_with_number = push_resource_file_script_path.gsub(script_number_placeholder, script_count.to_s)

    file_name = resource[0]
    file_location = win_friendly_path(File.join(Chef::Config[:file_cache_path], file_name))
    application_guid = resource[1]

    # Copy resource if it doesn't exist(DataPopulateConfiguration.Json is created dynamically and will be there already)
    if not File.exist?(file_location)

        # Copy the resource file from the cookbook to the Chef cache
        cookbook_file file_location do
            source file_name
        end

    end

    # Copy the powershell script template file from the cookbook to the Chef cache
    template script_fullpath_with_number do
        variables(
            'file_name': file_name,
            'file_location': file_location,
            'application_guid': application_guid,
            'relativity_services_url': node['relativity']['services_url'],
            'relativity_username': node['sample_data_population']['relativity_admin_account']['login'],
            'relativity_password': node['sample_data_population']['relativity_admin_account']['password']
          )
        source 'powershell_post_relativity_push_resource.ps1.erb'
    end
 
    # Run the powershell scripts to create a workspace with sample data
    powershell_script 'Push Resource File' do
        code script_fullpath_with_number
    end

    # Increment counter
    script_count += 1

end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished Pushing Resource Files' end
