log 'Starting Windows Update install for Service Bus defect'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

service_bus_defect_windows_udpate_installer_location = "#{node['service_bus']['defect_windows_update']['install']['destination_folder']}/#{node['service_bus']['defect_windows_update']['install']['file_name']}"

# Copy the service bus windows update file to install directory
cookbook_file service_bus_defect_windows_udpate_installer_location do
  source node['service_bus']['installer_file_name']
end

# Install windows update
windows_package 'Install Windows Update for Service Bus defect' do
  action :install
  installer_type :installshield
  source service_bus_defect_windows_udpate_installer_location
  options '/quiet /q /norestart'
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Windows Update install for Service Bus defect'
