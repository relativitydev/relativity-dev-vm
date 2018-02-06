log 'Starting Pre-Windows Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create default folders on VM
include_recipe 'Relativity::pre_windows_create_default_folders'

# Setup Log file
include_recipe 'Relativity::pre_windows_setup_log_file'

# Install Nuget provider
include_recipe 'Relativity::pre_windows_install_nuget_provider'

# Change Computer Name
include_recipe 'Relativity::pre_windows_change_computer_name'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Pre-Windows Setup'
