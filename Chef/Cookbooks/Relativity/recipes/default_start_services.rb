custom_log 'custom_log' do msg 'Starting Services' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

for service in node['services']
  start_service service[:name] do
    item_type service[:type]
    serviceBusRelated service[:serviceBus]
    location service[:location]
  end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Starting Services\n\n\n" end
