log 'Starting Service Bus install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

include_recipe 'webpi'

# Instal Service Bus
webpi_product 'ServiceBus_1_1_CU1' do
  accept_eula true
  action :install
end

cert_flag = '-CertificateAutoGenerationKey $mycert '
farm_connection_string = "Data Source=#{node['fqdn']};User Id=#{node['sql']['user']['sa']['login']};Password=#{node['sql']['user']['sa']['password']}"
farm_catalog_connection_string = "Data Source=#{node['fqdn']};Initial Catalog=SbManagementDB;User Id=#{node['sql']['user']['sa']['login']};Password=#{node['sql']['user']['sa']['password']}"

# Powershell module import line.
IMPORT_MODULE = 'Import-Module "C:/Program Files/Service Bus/1.1/ServiceBus/ServiceBus.psd1" -ErrorAction Stop'.freeze

sleep 30

# Create the initial farm, if needed
log 'Creating new service bus farm'
powershell_script 'new_sb_farm' do
  code <<-EOH
#{IMPORT_MODULE}
$mycert = ConvertTo-SecureString -String '#{node['sql']['user']['sa']['password']}' -Force -AsPlainText
New-SBFarm -SBFarmDBConnectionString '#{farm_connection_string}' -RunAsAccount '#{node['service_bus']['run_as_account']}' -AdminGroup 'BUILTIN\\Administrators' -MessageContainerDBConnectionString '#{farm_connection_string}' #{cert_flag}
    EOH
  not_if "#{IMPORT_MODULE}; (Get-SBFarm -SBFarmDBConnectionString '#{farm_catalog_connection_string}') -ne $null"
end
log 'Created new service bus farm'

sleep 30

# Add the host to the newly created farm
log 'Adding service bus host to farm'
powershell_script 'add_sb_host' do
  code <<-EOH
#{IMPORT_MODULE}
$mycert = ConvertTo-SecureString -String '#{node['sql']['user']['sa']['password']}' -Force -AsPlainText
$SBRunAsPassword = ConvertTo-SecureString -AsPlainText -Force -String '#{node['sql']['user']['sa']['password']}'
Add-SBHost -SBFarmDBConnectionString '#{farm_catalog_connection_string}' -CertificateAutoGenerationKey $mycert -RunAsPassword $SBRunAsPassword -EnableFirewallRules $true
EOH
  # Check if the current machine is listed in the Farm's hosts
  not_if "#{IMPORT_MODULE}; (Get-SBFarm).Hosts.Name -contains '#{node['fqdn']}'"
end
log 'Added service bus host to farm'

sleep 30

# Create a new namespace
log 'Creating new service bus namespace'
powershell_script 'new_sb_namespace' do
  code <<-EOH
#{IMPORT_MODULE}
New-SBNamespace -Name 'ServiceBusDefaultNamespace' -AddressingScheme 'Path' -ManageUsers '#{node['service_bus']['run_as_account']}'
EOH
  not_if "#{IMPORT_MODULE}; (Get-SBNamespace).Name -eq 'ServiceBusDefaultNamespace'"
end
log 'Created new service bus namespace'

sleep 30

# Update the DNS
log 'Setting service bus farm dns'
powershell_script 'set_sb_farm_dns' do
  code <<-EOH
#{IMPORT_MODULE}
Start-SBFarm
Stop-SBFarm
Set-SBFarm -FarmDns #{node['windows']['hostname']}
Update-SBHost
Start-SBFarm
EOH
  not_if "#{IMPORT_MODULE}; (Get-SBFarm).FarmDNS -eq '#{node['windows']['hostname']}'"
end
log 'Finished setting service bus farm dns'

sleep 30

log 'Setting correct service bus credentials'
powershell_script 'correct_sb_creds' do
  code <<-EOH
#{IMPORT_MODULE}
Stop-SBFarm
Set-SBFarm -RunAsAccount '#{node['service_bus']['run_as_account']}'
$RunAsPassword = ConvertTo-SecureString -AsPlainText -Force '#{node['service_bus']['run_as_account_password']}'
Update-SBHost -RunAsPassword $RunAsPassword
Start-SBFarm
    EOH
  not_if "#{IMPORT_MODULE}; (Get-SBFarm).RunAsAccount -eq '#{node['service_bus']['run_as_account']}'"
end
log 'Finished setting correct service bus credentials'

log 'sleeping for 2 mins for service bus services to start running'
sleep 120

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Service Bus install'
