log 'Starting Services'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

for service in node['services']
    start_service service[:name] do
      item_type service[:type]
      serviceBusRelated service[:serviceBus]
      location service[:location]
    end
  end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Starting Services'