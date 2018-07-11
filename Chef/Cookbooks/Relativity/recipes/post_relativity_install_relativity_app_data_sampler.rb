log 'Starting Data Sampler Relativity App Install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
create_workspace_with_sample_data_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_create_workspace_with_sample_data.ps1'))
relativity_data_sampler_app_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'Relativity_App_Data_Sampler.rap'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

# Copy the powershell script template file from the cookbook to the Chef cache
template create_workspace_with_sample_data_script_path do
  source 'powershell_post_relativity_install_relativity_app_data_sampler.ps1.erb'
end

# Copy the relativity data sample app file from the cookbook to the Chef cache
cookbook_file relativity_data_sampler_app_path do
  source 'Relativity_App_Data_Sampler.rap'
end

# Run the powershell scripts to create a workspace with sample data
powershell_script 'create workspace with sample data' do
  code create_workspace_with_sample_data_script_path
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Data Sampler Relativity App Install'