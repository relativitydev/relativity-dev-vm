default['timeout']['default'] = 3600

default['windows']['hostname'] = node['fqdn']
default['windows']['new_computer_name'] = 'RelativityDevVm'
default['windows']['user']['admin']['login'] = 'Administrator'
default['windows']['user']['admin']['password'] = 'Password1!'

default['sql']['installer_iso_file_location'] = 'C:/Chef_Install/Sql/Sql.iso'
default['sql']['user']['sa']['login'] = 'sa'
default['sql']['user']['sa']['password'] = 'Password1!'
default['sql']['user']['eddsdbo']['password'] = 'Password1!'
default['sql']['instance_name'] = 'MSSQLSERVER'
default['sql']['directories']['backup'] = 'c:\bak'
default['sql']['directories']['logs'] = 'c:\ldf'
default['sql']['directories']['data'] = 'c:\mdf'
default['sql']['directories']['fulltext'] = 'c:\ndf'

default['service_bus']['run_as_account'] = default['windows']['user']['admin']['login']
default['service_bus']['run_as_account_password'] = default['windows']['user']['admin']['password']
default['service_bus']['services']['service_bus_gateway'] = 'Service Bus Gateway'
default['service_bus']['services']['service_bus_message_broker'] = 'Service Bus Message Broker'
default['service_bus']['services']['service_bus_resource_provider'] = 'Service Bus Resource Provider'
default['service_bus']['services']['service_bus_vss'] = 'Service Bus VSS'
default['service_bus']['services']['service_bus_FabricHostSvc'] = 'FabricHostSvc'

default['relativity']['install_directory'] = 'C:/Chef_Install/Relativity'
default['relativity']['installer_file_location'] = 'C:/Chef_Install/Relativity/Relativity.exe'
default['relativity']['response_file_location'] = 'C:/Chef_Install/Relativity/RelativityResponse.txt'
default['relativity']['admin']['login'] = 'relativity.admin@kcura.com'
default['relativity']['admin']['password'] = 'Test1234!'
default['relativity']['processing']['source']['location'] = "\\\\#{default['windows']['hostname']}\\ProcessingSourceLocation"

default['invariant']['install_directory'] = 'C:/Chef_Install/Invariant'
default['invariant']['installer_file_location'] = 'C:/Chef_Install/Invariant/Invariant.exe'
default['invariant']['install_rpc_exe_local'] = 'C:/Chef_Install/Invariant/RPC.exe'
default['invariant']['nist_package_zip_file_location'] = 'C:/Chef_Install/Invariant/NISTPackage.zip'
default['invariant']['response_file'] = 'C:/Chef_Install/Invariant/InvariantResponse.txt'

default['sample_data_population']['workspace_name'] = 'Sample Workspace'
default['sample_data_population']['relativity_admin_account']['login'] = default['relativity']['admin']['login']
default['sample_data_population']['relativity_admin_account']['password'] = default['relativity']['admin']['password']
default['sample_data_population']['number_of_documents'] = '11'
default['sample_data_population']['import_images_with_Documents'] = '$TRUE'
default['sample_data_population']['import_production_images_with_documents'] = '$TRUE'
