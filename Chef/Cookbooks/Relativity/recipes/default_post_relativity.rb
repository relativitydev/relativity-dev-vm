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

# Create Workspace
include_recipe 'Relativity::post_relativity_create_workspace'

# Create Data Sampler Config File
include_recipe 'Relativity::post_relativity_create_data_sampler_config_file'

# Push Resource Files
include_recipe 'Relativity::post_relativity_push_resource_file'

# Install Relativity Applications
include_recipe 'Relativity::post_relativity_install_rap_files'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Post-Relativity Setup'
