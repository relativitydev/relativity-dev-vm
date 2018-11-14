custom_log 'custom_log' do msg 'Starting Workspace Creation' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
create_workspace_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_post_relativity_create_workspace.ps1'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

# Copy the powershell script template file from the cookbook to the Chef cache
template create_workspace_script_path do
  variables(
    'workspace_name': node['sample_workspace_name'],
    'relativity_services_url': node['relativity']['services_url'],
    'relativity_username': node['sample_data_population']['relativity_admin_account']['login'],
    'relativity_password': node['sample_data_population']['relativity_admin_account']['password']
  )
  source 'powershell_post_relativity_create_workspace.ps1.erb'
end

# Run the powershell scripts to create a workspace
powershell_script 'create workspace' do
  code create_workspace_script_path
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Data Sampler Relativity App Install\n\n\n" end
