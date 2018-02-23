custom_log 'custom_log' do msg 'Starting copying install files' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy SQL Install File
copy_file_to_vm_from_host 'copy_sql_install_file' do
  file_source "#{node['sql']['install']['source_folder']}\\#{node['sql']['install']['file_name']}"
  file_destination "#{node['sql']['install']['destination_folder']}\\#{node['sql']['install']['file_name']}"
  file_destination_folder node['sql']['install']['destination_folder']
end

# Copy Relativity Install File
copy_file_to_vm_from_host 'copy_relativity_install_file' do
  file_source "#{node['relativity']['install']['source_folder']}\\#{node['relativity']['install']['file_name']}"
  file_destination "#{node['relativity']['install']['destination_folder']}\\#{node['relativity']['install']['file_name']}"
  file_destination_folder node['relativity']['install']['destination_folder']
end

# Copy Invariant Install File
copy_file_to_vm_from_host 'copy_invariant_install_file' do
  file_source "#{node['invariant']['install']['source_folder']}\\#{node['invariant']['install']['file_name']}"
  file_destination "#{node['invariant']['install']['destination_folder']}\\#{node['invariant']['install']['file_name']}"
  file_destination_folder node['invariant']['install']['destination_folder']
end

# Copy Invariant Install File
copy_file_to_vm_from_host 'copy_service_bus_defect_windows_update_install_file' do
  file_source "#{node['service_bus']['defect_windows_update']['install']['source_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
  file_destination "#{node['service_bus']['defect_windows_update']['install']['destination_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
  file_destination_folder node['service_bus']['defect_windows_update']['install']['destination_folder']
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished copying install files\n\n\n" end
