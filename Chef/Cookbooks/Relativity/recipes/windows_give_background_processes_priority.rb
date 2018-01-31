log 'Starting giving background processes priority'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Gives background processes priority
registry_key 'HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\PriorityControl' do
  values [{ name: 'Win32PrioritySeparation', type: :dword, data: 18 }]
  action :create
end

log_message "log_message" do  message "Finished setting up priority for background processes" end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished giving background processes priority'