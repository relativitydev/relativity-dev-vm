custom_log 'custom_log' do msg 'Installing RAP File in Relativity' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Iteration variables
workspaces = [node['sample_workspace_name'], node['sample_data_grid_workspace_name']]
script_count = 0
script_number_placeholder = '<?>'
script_name = 'powershell_post_relativity_install_relativity_application'
script_extension = '.ps1'

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
install_relativity_application_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], "#{script_name}#{script_number_placeholder}#{script_extension}"))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
    source 'powershell_functions.ps1'
end

# Install apps in each workpsace
for workspace_name in workspaces
    
    # Loop through array populated with RAP File names and install each one
    for rap_file_name in node['relativity_apps_to_install']

        # replace placeholder with the loop iteration number
        script_fullpath_with_number = install_relativity_application_script_path.gsub(script_number_placeholder, script_count.to_s)

        rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], rap_file_name))

        # Copy the powershell script template file from the cookbook to the Chef cache, 1 copy for each loop iteration
        custom_log 'custom_log' do msg "copying template for: #{rap_file_name}" end
        template script_fullpath_with_number do
            variables(
                'rap_file_name': rap_file_name,
                'rap_file_path': rap_file_path,
                'sample_workspace_name': workspace_name,
                'relativity_services_url': node['relativity']['services_url'],
                'relativity_username': node['sample_data_population']['relativity_admin_account']['login'],
                'relativity_password': node['sample_data_population']['relativity_admin_account']['password']
            )
            source 'powershell_post_relativity_install_relativity_application.ps1.erb'
        end
        
        rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], rap_file_name))
        
        # Copy the RAP file from the cookbook to the Chef cache
        custom_log 'custom_log' do msg "copying the following rap file to cache: #{rap_file_name}" end
        cookbook_file rap_file_path do
            source rap_file_name
        end

        # Run the powershell scripts to install rap file
        custom_log 'custom_log' do msg "Running powershell script to install the following rap file: #{rap_file_name}" end
        powershell_script "install Relativity Application - #{rap_file_name}" do
            code script_fullpath_with_number
        end

        # Increment counter
        script_count += 1

    end

end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Installing RAP Files\n\n\n" end
