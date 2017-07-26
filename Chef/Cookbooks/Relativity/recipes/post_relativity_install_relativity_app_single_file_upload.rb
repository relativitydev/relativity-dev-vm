log 'Starting Single File Upload Relativity App Install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
single_file_upload_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_post_relativity_install_relativity_app_single_file_upload.ps1s'))
relativity_app_single_file_upload_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'Single File Upload 1.2.0.16 (for Relativity 9.4 - 9.5 - RelOne).rap'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

# Copy the powershell script template file from the cookbook to the Chef cache
template single_file_upload_script_path do
  source 'powershell_post_relativity_install_relativity_app_single_file_upload.ps1.erb'
end

# Copy the relativity single file upload app file from the cookbook to the Chef cache
cookbook_file relativity_app_single_file_upload_path do
  source 'Single File Upload 1.2.0.16 (for Relativity 9.4 - 9.5 - RelOne).rap'
end

# Run the powershell scripts to install single file upload relativity app
powershell_script 'install single file upload relativity app' do
  code single_file_upload_script_path
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Single File Upload Relativity App Install'
