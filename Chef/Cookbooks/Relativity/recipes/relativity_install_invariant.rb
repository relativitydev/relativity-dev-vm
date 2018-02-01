log 'Starting Invariant install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

invariant_install_file = "#{node['invariant']['install']['destination_folder']}\\#{node['invariant']['install']['file_name']}"
invariant_response_file = node['invariant']['install']['response_file_destination_location']

# update response file
template invariant_response_file do
  source 'InvariantResponse.txt.erb'
end

powershell_script 'install_invariant' do
  code <<-EOH
    $process = Start-Process -FilePath '#{invariant_install_file}' -ArgumentList @('-Log #{node['invariant']['install']['destination_folder']}\\install_log.txt', '-ResponseFilePath=#{invariant_response_file}') -Wait -WindowStyle Hidden -PassThru
    exit $process.ExitCode
  EOH
  timeout node['timeout']['default']
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Invariant install'
