custom_log 'custom_log' do msg 'Starting giving background processes priority' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Gives background processes priority
registry_key 'HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\PriorityControl' do
  values [{ name: 'Win32PrioritySeparation', type: :dword, data: 18 }]
  action :create
end

custom_log 'custom_log' do msg 'Finished setting up priority for background processes' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished giving background processes priority' end
  