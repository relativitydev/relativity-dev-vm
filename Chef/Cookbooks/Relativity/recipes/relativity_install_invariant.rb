log 'Starting Invariant install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

install_exe_local = node['invariant']['installer_file_location']
response_file = node['invariant']['response_file']
install_rpc_exe_local = node['invariant']['install_rpc_exe_local']
nist_zip_local = node['invariant']['nist_package_zip_file_location']

#todo
# unless node['invariant']['nist_package'] == ''
#   remote_file nist_zip_local do
#     source node['invariant']['nist_package']
#     checksum node['invariant']['nist_package_sha256']
#   end
# end

# remote_file install_exe_local do
#   source node['invariant']['install_exe_path']
#   checksum node['invariant']['install_exe_sha256']
# end

# # Copy the NIST Package file from the cookbook to the Chef cache.
# nist_package_installer_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'NISTPackage.zip'))
# cookbook_file nist_package_installer_path do
#   source 'NISTPackage.zip'
# end

# # Copy the invariant install file from the cookbook to the Chef cache.
# invariant_installer_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'Invariant.exe'))
# cookbook_file invariant_installer_path do
#   source 'Invariant.exe'
# end

# update response file
template node['invariant']['response_file'] do
  source 'InvariantResponse.txt.erb'
end

powershell_script 'install_invariant' do
  code <<-EOH
    $process = Start-Process -FilePath '#{install_exe_local}' -ArgumentList @('-Log #{node['invariant']['install_directory']}\\install_log.txt', '-ResponseFilePath=#{response_file}') -Wait -WindowStyle Hidden -PassThru
    exit $process.ExitCode
  EOH
  timeout node['timeout']['default']
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Invariant install'
