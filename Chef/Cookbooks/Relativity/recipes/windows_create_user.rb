log 'Starting creation of RelativityService windows user'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create RelativityService user
user node['windows']['user']['relativity_service_account']['login'] do
  password node['windows']['user']['relativity_service_account']['password']
end


# Add RelativityService user to Administrators group
group 'Administrators' do
  action :modify
  members node['windows']['user']['relativity_service_account']['login']
  append true
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished creation of RelativityService windows user'
