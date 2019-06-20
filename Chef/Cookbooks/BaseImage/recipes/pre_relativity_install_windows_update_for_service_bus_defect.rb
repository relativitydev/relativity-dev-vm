custom_log 'custom_log' do msg 'Starting Windows Update install for Service Bus defect' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy Service Bus Defect Windows Update Install File
copy_file_to_vm_from_host 'copy_service_bus_defect_windows_update_install_file' do
  file_source "#{node['service_bus']['defect_windows_update']['install']['source_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
  file_destination "#{node['service_bus']['defect_windows_update']['install']['destination_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
  file_destination_folder node['service_bus']['defect_windows_update']['install']['destination_folder']
end

service_bus_defect_windows_udpate_installer_source_location = "#{node['service_bus']['defect_windows_update']['install']['source_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
service_bus_defect_windows_udpate_installer_destination_location = "#{node['service_bus']['defect_windows_update']['install']['destination_folder']}/#{node['service_bus']['defect_windows_update']['install']['file_name']}"

# Copy the service bus windows update file to install directory
cookbook_file service_bus_defect_windows_udpate_installer_destination_location do
  source service_bus_defect_windows_udpate_installer_source_location
end

# Install windows update
windows_package 'Install Windows Update for Service Bus defect' do
  action :install
  installer_type :installshield
  source service_bus_defect_windows_udpate_installer_destination_location
  options '/quiet /q /norestart'
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Windows Update install for Service Bus defect\n\n\n" end
