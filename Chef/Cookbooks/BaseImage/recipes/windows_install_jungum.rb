custom_log 'custom_log' do msg 'Starting JungUm Install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

jungum_is_installed = File.exist?(node['software']['jungum']['executable'])

if jungum_is_installed
  custom_log 'custom_log' do msg 'JungUM Viewer is already installed. Skipping...' end
else

    install_file_destination = "#{node['software']['jungum']['destination_folder']}\\#{node['software']['jungum']['file_name']}"

    # Copy JungUm install file to VM
    copy_file_to_vm_from_host 'copy_junum_install_file' do
        file_source node['software']['jungum']['source']
        file_destination install_file_destination
        file_destination_folder node['software']['jungum']['destination_folder']
    end

    windows_package 'install_jungUM' do
        source install_file_destination
        options '/quiet /q'
    end

    file install_file_destination do
        action	:delete
    end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished giving background processes priority\n\n\n" end
