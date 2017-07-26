log 'Starting Default recipe'
start_time = DateTime.now
log "cookbook_start_time(#{cookbook_name}): #{start_time}"

# Setup Windows
include_recipe 'Relativity::default_windows'

# Setup Pre-Relativity
include_recipe 'Relativity::default_pre_relativity'

# Setup Relativity
include_recipe 'Relativity::default_relativity'

# Setup Post Relativity
include_recipe 'Relativity::default_post_relativity'

# Configure Invariant
include_recipe 'Relativity::post_relativity_configure_invariant'

end_time = DateTime.now
log "cookbook_end_Time(#{cookbook_name}): #{end_time}"
log "cookbook_duration(#{cookbook_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Default recipe'
