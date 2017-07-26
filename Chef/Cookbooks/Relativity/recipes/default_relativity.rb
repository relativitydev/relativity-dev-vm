log 'Starting Relativity Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Relativity
include_recipe 'Relativity::relativity_install'

# Install Invariant
include_recipe 'Relativity::relativity_install_invariant'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Relativity Setup'
