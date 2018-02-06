log 'Starting Default recipe'
start_time = DateTime.now
log "cookbook_start_time(#{cookbook_name}): #{start_time}"

###################################################
# Please refer to Vagrantfile or kitchen.yml files
###################################################

end_time = DateTime.now
log "cookbook_end_Time(#{cookbook_name}): #{end_time}"
log "cookbook_duration(#{cookbook_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Default recipe'
