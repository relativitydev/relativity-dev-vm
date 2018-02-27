custom_log 'custom_log' do msg 'Starting Microsoft Office Install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

install_file_destination = "#{node['software']['MSOffice']['destination_folder']}\\#{node['software']['MSOffice']['file_name']}"
node.default['msoffice']['source'] = install_file_destination
node.default['seven_zip']['home'] = 'C:\\Program Files\\7-Zip'

# Copy JungUm install file to VM
copy_file_to_vm_from_host 'copy_msoffice_iso' do
    file_source node['software']['MSOffice']['source']
    file_destination install_file_destination
    file_destination_folder node['software']['MSOffice']['destination_folder']
end

include_recipe 'msoffice::install'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished giving background processes priority\n\n\n" end