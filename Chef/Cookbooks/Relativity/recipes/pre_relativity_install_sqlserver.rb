log 'Starting Sql Server install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

ps_modules = %w(xSQLServer Carbon)

ps_modules.each do |ps_module|
  powershell_script "install_#{ps_module}_module" do
    code "Install-Module #{ps_module} -Force"
    not_if "(Get-Module -ListAvailable).Name -Contains \"#{ps_module}\""
  end
end

#todo
# directories = %w(C:/chef/sql C:/relativity/BCPPath)

# directories.each do |directory|
#   directory directory do
#     recursive true
#   end
# end

# powershell_script 'install_fileshare_BCPPath' do
#   code "Install-FileShare -Name BCPPath -Path C:/relativity/BCPPath -FullAccess 'Everyone'"
#   not_if "(Get-FileShare).Name -Contains 'BCPPath'"
# end

# extract_iso 'sql_iso' do
#   iso_source node['sql']['iso_path']
#   target 'C:/chef/sql/'
#   not_if { ::File.exist?('C:/chef/sql/setup.exe') }
# end

relativity_pscredential = ps_credential(node['windows']['user']['admin']['login'], node['windows']['user']['admin']['password'])

# Install Microsoft SQL Server
dsc_resource 'install_sql_server' do
  resource :xsqlserversetup
  property :features, 'SQLENGINE,FULLTEXT'
  property :instancename, node['sql']['instance_name']
  property :securitymode, 'SQL'
  property :sourcepath, 'C:/Chef_Install/Sql'
  property :updateenabled, 'False'
  property :agtsvcaccount, relativity_pscredential
  property :ftsvcaccount, relativity_pscredential
  property :sapwd, relativity_pscredential
  property :setupcredential, relativity_pscredential
  property :sqlsvcaccount, relativity_pscredential
  property :sqlsysadminaccounts, [node['windows']['user']['admin']['login']]
  timeout 3600
end

# Enable SQL TCP and Named Pipes remote connections
protocols = %w(Tcp Np)

protocols.each do |protocol|
  powershell_script "enable_protocol_#{protocol}_#{node['sql']['instance_name']}" do
    code <<-EOH
      # Import-Module sqlps
      [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo")
      [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.SqlWmiManagement")
      $wmi = New-Object ('Microsoft.SqlServer.Management.Smo.Wmi.ManagedComputer')
      $uri = "ManagedComputer[@Name='#{node['hostname'].upcase}']/ ServerInstance[@Name='#{node['sql']['instance_name']}']/ServerProtocol[@Name='#{protocol}']"
      $protocol = $wmi.GetSmoObject($uri)
      $protocol.IsEnabled = $true
      $protocol.Alter()
      EOH
  end
end

powershell_script "restart_MSSQL$#{node['sql']['instance_name']}" do
  action :nothing
  code <<-EOH
    Restart-Service '#{node['sql']['instance_name']}' -Force
    EOH
end

node['sql']['directories'].each do |_key, path|
  directory path do
    recursive true
  end
end

service 'SQLBrowser' do
  action [:enable, :start]
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Sql Server install'
