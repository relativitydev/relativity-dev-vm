log 'Starting Powershell V5 install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Powershell 5
include_recipe 'powershell::powershell5'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Powershell V5 install'