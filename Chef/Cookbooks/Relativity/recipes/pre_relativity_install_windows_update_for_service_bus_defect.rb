log 'Starting Windows Update install for Service Bus defect'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

#todo
# cookbook_file 'C:/Chef_Install/ServiceBusWindowsUpdate/AppServer-KB3086798-x64-EN.exe' do
#   source 'AppServer-KB3086798-x64-EN.exe'
# end

windows_package 'Install Windows Update for Service Bus defect' do
  action :install
  installer_type :installshield
  source 'C:/Chef_Install/ServiceBusWindowsUpdate/AppServer-KB3086798-x64-EN.exe'
  options '/quiet /q /norestart'
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Windows Update install for Service Bus defect'
