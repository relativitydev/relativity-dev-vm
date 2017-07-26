log 'Starting copying install files'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create directories for Install files
directory_chef_install = 'C:/Chef_Install'
directory_chef_install_invariant = 'C:/Chef_Install/Invariant'
directory_chef_install_relativity = 'C:/Chef_Install/Relativity'
directory_chef_install_sql = 'C:/Chef_Install/Sql'

directories = [directory_chef_install, directory_chef_install_invariant, directory_chef_install_relativity, directory_chef_install_sql]

# Create and Share directories
directories.each do |dir|
  # create directory
  directory dir do
    rights :full_control, 'Everyone'
    recursive true
    action :create
  end

  # share directory
  powershell_script 'directory_chef_install' do
    code <<-EOH
  New-SmbShare -Name 'Backup' -Path '#{dir}' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Backup'"
    action :run
  end
end

log 'Copying SQL iso file'
cookbook_file node['sql']['installer_iso_file_location'] do
  source 'Sql.iso'
end

log 'Copying Relativity exe file'
cookbook_file node['relativity']['installer_file_location'] do
  source 'Relativity.exe'
end

log 'Copying NISTPackage zip file'
cookbook_file node['invariant']['nist_package_zip_file_location'] do
  source 'NISTPackage.zip'
end

log 'Copying Invariant exe file'
cookbook_file node['invariant']['installer_file_location'] do
  source 'Invariant.exe'
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished copying install files'
