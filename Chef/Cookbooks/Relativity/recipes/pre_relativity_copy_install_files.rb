log 'Starting copying install files'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create Default Destination folder if it not already exists
directory node['file']['installers']['default_destination_folder'] do
  action :create
end

# Copy SQL Install File
copy_file_to_vm_from_host "copy_sql_install_file" do
  file_source "#{node['sql']['install']['source_folder']}\\#{node['sql']['install']['file_name']}"
  file_destination "#{node['sql']['install']['destination_folder']}\\#{node['sql']['install']['file_name']}"
  file_destination_folder node['sql']['install']['destination_folder']
end

# Copy Relativity Install File
copy_file_to_vm_from_host "copy_relativity_install_file" do
  file_source "#{node['relativity']['install']['source_folder']}\\#{node['relativity']['install']['file_name']}"
  file_destination "#{node['relativity']['install']['destination_folder']}\\#{node['relativity']['install']['file_name']}"
  file_destination_folder node['relativity']['install']['destination_folder']
end

# Copy Invariant Install File
copy_file_to_vm_from_host "copy_invariant_install_file" do
  file_source "#{node['invariant']['install']['source_folder']}\\#{node['invariant']['install']['file_name']}"
  file_destination "#{node['invariant']['install']['destination_folder']}\\#{node['invariant']['install']['file_name']}"
  file_destination_folder node['invariant']['install']['destination_folder']
end

# Copy Invariant Install File
copy_file_to_vm_from_host "copy_service_bus_defect_windows_update_install_file" do
  file_source "#{node['service_bus']['defect_windows_update']['install']['source_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
  file_destination "#{node['service_bus']['defect_windows_update']['install']['destination_folder']}\\#{node['service_bus']['defect_windows_update']['install']['file_name']}"
  file_destination_folder node['service_bus']['defect_windows_update']['install']['destination_folder']
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished copying install files'
