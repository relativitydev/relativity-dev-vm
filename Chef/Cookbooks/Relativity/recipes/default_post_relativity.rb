log 'Starting Post-Relativity Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Setup Relativity to be accessed via http
include_recipe 'Relativity::post_relativity_instance_setting_update_cookie_secure'

# Setup Relativity Developer Mode
include_recipe 'Relativity::post_relativity_instance_setting_update_developer_mode'

# IIS Reset
include_recipe 'Relativity::post_relativity_reset_iis'

# Change Relativity windows services startup type to Automatic
include_recipe 'Relativity::post_relativity_update_kcura_services_startup_type'

# Create Workspace With Sample Data
include_recipe 'Relativity::post_relativity_install_relativity_app_data_sampler'

# Install Single File Upload Relativity App
include_recipe 'Relativity::post_relativity_install_relativity_app_single_file_upload'

# Install Smoke Test Relativity App
include_recipe 'Relativity::post_relativity_install_relativity_app_smoke_test'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Post-Relativity Setup'
