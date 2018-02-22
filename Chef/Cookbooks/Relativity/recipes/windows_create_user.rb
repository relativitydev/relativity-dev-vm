custom_log 'custom_log' do msg 'Starting creation of RelativityService windows user' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

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
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished creation of RelativityService windows user' end
