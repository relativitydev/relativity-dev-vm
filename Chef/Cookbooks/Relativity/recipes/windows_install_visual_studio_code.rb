log 'Starting Visual Studio Code install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Visual Studio Code
include_recipe 'vs_code::default'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Visual Studio Code install'
