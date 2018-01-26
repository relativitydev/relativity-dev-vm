log 'Starting Pre-Windows Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Nuget provider
include_recipe 'Relativity::pre_windows_install_nuget_provider'

# Change Computer Name
include_recipe 'Relativity::windows_change_computer_name'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Pre-Windows Setup'
