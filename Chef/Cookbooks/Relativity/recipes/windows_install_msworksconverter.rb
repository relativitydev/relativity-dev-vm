custom_log 'custom_log' do msg 'Starting Microsoft Works Converter Install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end


install_file_destination = "#{node['software']['MSWorksConverter']['destination_folder']}\\#{node['software']['MSWorksConverter']['file_name']}"

msworks_is_installed = File.exist?(node['software']['MSWorksConverter']['executable'])

if msworks_is_installed
    custom_log 'custom_log' do msg 'MS Works Converter already Installed' end
else
    #Copy MS Works Converter install file to VM
    copy_file_to_vm_from_host 'ms_works_exe' do
        file_source node['software']['MSWorksConverter']['source']
        file_destination install_file_destination
        file_destination_folder node['software']['MSWorksConverter']['destination_folder']
    end

    powershell_script 'Install Microsoft Works Converter' do
        code "#{install_file_destination}  /passive /norestart"
    end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished giving background processes priority\n\n\n" end