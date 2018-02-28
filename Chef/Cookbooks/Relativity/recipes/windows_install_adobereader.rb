custom_log 'custom_log' do msg 'Starting Adobe Reader Install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end


install_file_destination = "#{node['software']['adobe_reader']['destination_folder']}\\#{node['software']['adobe_reader']['file_name']}"
node.default['adobereader']['source'] = install_file_destination
node.default['adobereader']['filename'] = node['software']['adobe_reader']['file_name']

adobereader_is_installed = File.exist?(node['software']['adobe_reader']['executable'])

if adobereader_is_installed
    custom_log 'custom_log' do msg 'Adobe Reader already Installed' end
else
    # Copy Adobe Reader zip to VM
    copy_file_to_vm_from_host 'copy_adobe_reader_zip' do
        file_source node['software']['adobe_reader']['source']
        file_destination install_file_destination
        file_destination_folder node['software']['adobe_reader']['destination_folder']
    end

    # Extract Contents of adobe zip
    seven_zip_archive "extract adobe zip" do
        source    install_file_destination
        path      node['software']['adobe_reader']['destination_folder']
        overwrite true
        timeout   300
    end

    batch 'install Adobe Reader Packages' do
        code "#{node['software']['adobe_reader']['destination_folder']}/setup.exe"
        action :nothing
    end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished giving background processes priority\n\n\n" end