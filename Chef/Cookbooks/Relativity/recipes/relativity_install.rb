log 'Starting Relativity install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

relativity_install_file = "#{node['relativity']['install']['destination_folder']}\\#{node['relativity']['install']['file_name']}"
relativity_response_file = node['relativity']['install']['response_file_destination_location']

# update response file
template relativity_response_file do
  source 'RelativityResponse.txt.erb'
end

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
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Relativity install'
