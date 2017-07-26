log 'Starting Smoke Test Relativity App Install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
smoke_test_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_post_relativity_install_relativity_app_smoke_test.ps1'))
relativity_app_smoke_test_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'Relativity_App_Smoke_Test.rap'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

# Copy the powershell script template file from the cookbook to the Chef cache
template smoke_test_script_path do
  source 'powershell_post_relativity_install_relativity_app_smoke_test.ps1.erb'
end

# Copy the relativity smoke test app file from the cookbook to the Chef cache
cookbook_file relativity_app_smoke_test_path do
  source 'Relativity_App_Smoke_Test.rap'
end

# Run the powershell scripts to install smoke test relativity app
powershell_script 'install smoke test relativity app' do
  code smoke_test_script_path
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Smoke Test Relativity App Install'
