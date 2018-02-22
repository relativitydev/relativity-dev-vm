custom_log 'custom_log' do msg 'Starting Invariant install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

invariant_install_file = "#{node['invariant']['install']['destination_folder']}\\#{node['invariant']['install']['file_name']}"
invariant_response_file = node['invariant']['install']['response_file_destination_location']

powershell_script 'install_invariant' do
  code <<-EOH
    $process = Start-Process -FilePath '#{invariant_install_file}' -ArgumentList @('-Log #{node['invariant']['install']['destination_folder']}\\install_log.txt', '-ResponseFilePath=#{invariant_response_file}') -Wait -WindowStyle Hidden -PassThru
    exit $process.ExitCode
  EOH
  timeout node['timeout']['default']
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished Invariant install' end
