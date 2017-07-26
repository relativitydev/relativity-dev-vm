log 'Starting Relativity install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

relativity_installer_file_location = node['relativity']['installer_file_location']
relativity_response_file = node['relativity']['response_file_location']

# update response file
template node['relativity']['response_file_location'] do
  source 'RelativityResponse.txt.erb'
end

# Import Service Bus module
IMPORT_MODULE = 'Import-Module "C:/Program Files/Service Bus/1.1/ServiceBus/ServiceBus.psd1" -ErrorAction Stop'.freeze

powershell_script 'install_relativity' do
  code <<-EOH
    #{IMPORT_MODULE}
    Start-SBFarm
    $process = Start-Process -FilePath '#{relativity_installer_file_location}' -ArgumentList @(\"-Log #{node['relativity']['install_directory']}\\install_log.txt\", \"-ResponseFilePath=#{relativity_response_file}\") -Wait -WindowStyle Hidden -PassThru
    exit $process.ExitCode
  EOH
  timeout node['timeout']['default']
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Relativity install'
