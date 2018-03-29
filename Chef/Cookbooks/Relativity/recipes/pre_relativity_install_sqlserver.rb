custom_log 'custom_log' do msg 'Starting Sql Server install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Extract Contents of iso file
seven_zip_archive "extract sql iso" do
  source    "#{node['sql']['install']['destination_folder']}\\#{node['sql']['install']['file_name']}"
  path      node['sql']['install']['destination_folder']
  overwrite true
  timeout   300
end

# Install xSQLServer powershell module
powershell_script 'install_xSQLServer_module' do
  code 'Install-Module -Name xSQLServer -RequiredVersion 7.1.0.0 -Force'
  not_if '(Get-Module -ListAvailable).Name -Contains \"xSQLServer\"'
end

# Install Carbon powershell module
powershell_script 'install_Carbon_module' do
  code 'Install-Module -Name Carbon -RequiredVersion 2.5.0 -Force'
  not_if '(Get-Module -ListAvailable).Name -Contains \"Carbon\"'
end

relativity_pscredential = ps_credential(node['windows']['user']['admin']['login'], node['windows']['user']['admin']['password'])

node['sql']['instances'].each do |instance|

  # Install Microsoft SQL Server
  dsc_resource 'install_sql_server' do
    resource :xsqlserversetup
    property :features, 'SQLENGINE,FULLTEXT'
    property :instancename, instance
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
    powershell_script "enable_protocol_#{protocol}_#{instance}" do
      code <<-EOH
        # Import-Module sqlps
        [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.Smo")
        [reflection.assembly]::LoadWithPartialName("Microsoft.SqlServer.SqlWmiManagement")
        $wmi = New-Object ('Microsoft.SqlServer.Management.Smo.Wmi.ManagedComputer')
        $uri = "ManagedComputer[@Name='#{node['windows']['hostname'].upcase}']/ ServerInstance[@Name='#{instance}']/ServerProtocol[@Name='#{protocol}']"
        $protocol = $wmi.GetSmoObject($uri)
        $protocol.IsEnabled = $true
        $protocol.Alter()
        EOH
    end
  end
end

node['sql']['directories'].each do |_key, path|
  directory path do
    recursive true
  end
end

# We are not manually starting sql server instance services here because distributed instances don't have service names that are straight forward.
# Be sure to do a full machine restart after this point to access the sql instances.

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Sql Server install\n\n\n" end
