log 'Starting Pre-Relativity Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Copy Installer Files
include_recipe 'Relativity::pre_relativity_copy_install_files'

# Install Windows Features and Services
include_recipe 'Relativity::pre_relativity_install_windows_features_and_services'

# Create shared folders
include_recipe 'Relativity::pre_relativity_create_shared_folders'

# Install Microsoft SQL Server
include_recipe 'Relativity::pre_relativity_install_sqlserver'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Pre-Relativity Setup'
