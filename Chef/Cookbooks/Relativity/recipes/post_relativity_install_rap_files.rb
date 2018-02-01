log 'Installing RAP File in Relativity'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Iteration variables
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
  
# Loop through array populated with RAP File names and install each one
for rap_file_name in node['relativity_apps_to_install']

    # replace placeholder with the loop iteration number
    script_fullpath_with_number = install_relativity_application_script_path.gsub(script_number_placeholder, script_count.to_s)

    rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], rap_file_name))

    # Copy the powershell script template file from the cookbook to the Chef cache, 1 copy for each loop iteration
    log "copying template for: #{rap_file_name}"
    template script_fullpath_with_number do
        variables(
            'rap_file_name': rap_file_name,
            'rap_file_path': rap_file_path,
            'sample_workspace_name': node['sample_workspace_name'],
            'relativity_services_url': node['relativity']['services_url'],
            'relativity_username': node['sample_data_population']['relativity_admin_account']['login'],
            'relativity_password': node['sample_data_population']['relativity_admin_account']['password']
          )
        source 'powershell_post_relativity_install_relativity_application.ps1.erb'
    end
    
    rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], rap_file_name))
    
    # Copy the RAP file from the cookbook to the Chef cache
    log "copying the followign rap file to cache: #{rap_file_name}"
    cookbook_file rap_file_path do
        source rap_file_name
    end

    # Run the powershell scripts to install rap file
    log "Running powershell script to install the following rap file: #{rap_file_name}"
    powershell_script 'install Relativity Application' do
        code script_fullpath_with_number
    end

    # Increment counter
    script_count += 1

end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Installing RAP Files'
