default['timeout']['default'] = 3600

default['windows']['hostname'] = node['machinename'] 
default['windows']['new_computer_name'] = 'RelativityDevVm'
default['windows']['user']['admin']['login'] = 'Administrator'
default['windows']['user']['admin']['password'] = 'Password1!'

default['file']['installers']['default_destination_folder'] = 'C:\\Chef_Install'
default['file']['log']['default_destination_folder'] = 'C:\\Chef_Logs'
default['file']['log']['name'] = 'log.txt'

default['sql']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm_Install_Files\\SQL_Server_2016_Developer_Edition'
default['sql']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Sql"
default['sql']['install']['file_name'] = 'Sql.iso'
default['sql']['user']['sa']['login'] = 'sa'
default['sql']['user']['sa']['password'] = 'Password1!'
default['sql']['user']['eddsdbo']['password'] = 'Password1!'
default['sql']['instance_name'] = 'MSSQLSERVER'
default['sql']['directories']['backup'] = 'c:\bak'
default['sql']['directories']['logs'] = 'c:\ldf'
default['sql']['directories']['data'] = 'c:\mdf'
default['sql']['directories']['fulltext'] = 'c:\ndf'

default['service_bus']['defect_windows_update']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm_Install_Files\\Service_Bus_Defect_Windows_Update'
default['service_bus']['defect_windows_update']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Service_Bus_Defect_Windows_Update"
default['service_bus']['defect_windows_update']['install']['file_name'] = 'AppServer-KB3086798-x64-EN.exe'
default['service_bus']['run_as_account'] = default['windows']['user']['admin']['login']
default['service_bus']['run_as_account_password'] = default['windows']['user']['admin']['password']
default['service_bus']['services']['service_bus_gateway'] = 'Service Bus Gateway'
default['service_bus']['services']['service_bus_message_broker'] = 'Service Bus Message Broker'
default['service_bus']['services']['service_bus_resource_provider'] = 'Service Bus Resource Provider'
default['service_bus']['services']['service_bus_vss'] = 'Service Bus VSS'
default['service_bus']['services']['service_bus_FabricHostSvc'] = 'FabricHostSvc'

default['relativity']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm_Install_Files\\Relativity'
default['relativity']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Relativity"
default['relativity']['install']['file_name'] = 'Relativity.exe'
default['relativity']['install']['response_file_destination_location'] = "#{default['relativity']['install']['destination_folder']}\\RelativityResponse.txt"
default['relativity']['services_url'] = "http://" + node['fqdn'] +"/Relativity.Services"
default['relativity']['admin']['login'] = 'relativity.admin@kcura.com'
default['relativity']['admin']['password'] = 'Test1234!'
default['relativity']['processing']['source']['location'] = "\\\\#{default['windows']['hostname']}\\ProcessingSourceLocation"

default['invariant']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm_Install_Files\\Invariant'
default['invariant']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Invariant"
default['invariant']['install']['file_name'] = 'Invariant.exe'
default['invariant']['install']['response_file_destination_location'] = "#{default['invariant']['install']['destination_folder']}\\InvariantResponse.txt"

default['sample_data_population']['workspace_name'] = 'Sample Workspace'
default['sample_data_population']['relativity_admin_account']['login'] = default['relativity']['admin']['login']
default['sample_data_population']['relativity_admin_account']['password'] = default['relativity']['admin']['password']
default['sample_data_population']['number_of_documents'] = '11'
default['sample_data_population']['import_images_with_Documents'] = '$TRUE'
default['sample_data_population']['import_production_images_with_documents'] = '$TRUE'
