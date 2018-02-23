custom_log 'custom_log' do msg 'Starting Relativity install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

relativity_install_file = "#{node['relativity']['install']['destination_folder']}\\#{node['relativity']['install']['file_name']}"
relativity_response_file = node['relativity']['install']['response_file_destination_location']

# Import Service Bus module
IMPORT_MODULE = 'Import-Module "C:/Program Files/Service Bus/1.1/ServiceBus/ServiceBus.psd1" -ErrorAction Stop'.freeze

powershell_script 'install_relativity' do
  code <<-EOH
    #{IMPORT_MODULE}
    Start-SBFarm
    $process = Start-Process -FilePath '#{relativity_install_file}' -ArgumentList @(\"-Log #{node['relativity']['install']['destination_folder']}\\install_log.txt\", \"-ResponseFilePath=#{relativity_response_file}\") -Wait -WindowStyle Hidden -PassThru
    exit $process.ExitCode
  EOH
  timeout node['timeout']['default']
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Relativity install\n\n\n" end
