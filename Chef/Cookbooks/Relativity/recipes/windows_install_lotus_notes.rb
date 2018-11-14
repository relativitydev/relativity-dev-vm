custom_log 'custom_log' do msg 'Starting Lotus Notes Install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

lotus_notes_is_installed = File.exist?(node['software']['lotus_notes']['executable'])

if lotus_notes_is_installed
    custom_log 'custom_log' do msg 'Lotus Notes is already installed. Skipping...' end
else

    install_file_destination = "#{node['software']['lotus_notes']['destination_folder']}\\#{node['software']['lotus_notes']['file_name']}"

    # Copy JungUm install file to VM
    copy_file_to_vm_from_host 'copy_junum_install_file' do
        file_source node['software']['lotus_notes']['source']
        file_destination install_file_destination
        file_destination_folder node['software']['lotus_notes']['destination_folder']
    end

    cookbook_file 'msvcr71.dll' do
        source 'msvcr71.dll'
        path 'C:/windows/sysWow64/msvcr71.dll'
        action :create_if_missing
      end

    windows_package 'install_lotus_notes' do
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
