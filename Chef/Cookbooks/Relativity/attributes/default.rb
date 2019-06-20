default['timeout']['default'] = 3600

default['windows']['hostname'] = node['machinename']
default['windows']['new_computer_name'] = 'RelativityDevVm'
default['windows']['user']['admin']['login'] = 'Administrator'
default['windows']['user']['admin']['password'] = 'Test1234!'

default['file']['installers']['default_destination_folder'] = 'C:\\Chef_Install'
default['file']['log']['default_destination_folder'] = 'C:\\vagrant\\Logs'
default['file']['log']['name'] = 'log.txt'
default['file']['result']['name'] = 'result_file.txt'
default['file']['result']['destination_folder'] = 'C:\\vagrant'

default['software']['jungum']['file_name'] = 'JungUm.msi'
default['software']['jungum']['source'] = "\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\Jungum\\#{default['software']['jungum']['file_name']}"
default['software']['jungum']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\JungUm"
default['software']['jungum']['executable'] = 'c:/Program Files (x86)/Samsung/JungUmGlobal/JungUm Global Viewer/JUGW_V.exe'
default['software']['lotus_notes']['file_name'] = 'LotusNotes.msi'
default['software']['lotus_notes']['source'] = "\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\LotusNotes\\#{default['software']['lotus_notes']['file_name']}"
default['software']['lotus_notes']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\LotusNotes"
default['software']['lotus_notes']['executable'] = 'c:/Program Files (x86)/IBM/Lotus/Notes/notes.exe'
default['software']['MSOffice']['file_name'] = 'SW_DVD5_Office_Professional_Plus_2010_W32_English_MLF_X16-52536.ISO'
default['software']['MSOffice']['source'] = "\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\MSOffice\\#{default['software']['MSOffice']['file_name']}"
default['software']['MSOffice']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\MSOffice"
default['software']['MSWorksConverter']['file_name'] = 'WorksConv.exe'
default['software']['MSWorksConverter']['source'] = "\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\MSWorksConverter\\#{default['software']['MSWorksConverter']['file_name']}"
default['software']['MSWorksConverter']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\MSWorksConverter"
default['software']['MSWorksConverter']['executable'] = 'C:\Program Files (x86)\Common Files\Microsoft Shared\TextConv\WksConv\Wkconv.exe'
default['software']['java_runtime']['home'] = "C:\\Program Files\\Java\\jre1.8.0_161"
default['software']['elastic_search']['file_name'] = 'datagrid-2.3.3.79-install.zip'
default['software']['elastic_search']['source'] = "\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\ElasticSearch\\#{default['software']['elastic_search']['file_name']}"
default['software']['elastic_search']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\ElasticSearch"
default['software']['elastic_search']['config_file_location'] = "C:\\RelativityDataGrid\\elasticsearch-main\\config\\elasticsearch.yml"
default['software']['elastic_search']['bin_folder'] = "C:\\RelativityDataGrid\\elasticsearch-main\\bin"
default['software']['elastic_search']['bat_filename'] = "kservice.bat"
default['software']['elastic_search']['max_memory_mb'] = "512"
default['software']['elastic_search']['number_of_chards'] = "2"
default['software']['elastic_search']['number_of_replicas'] = "0"

default['sql']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\SQL_Server_2016_Developer_Edition'
default['sql']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Sql"
default['sql']['install']['file_name'] = 'Sql.iso'
default['sql']['user']['sa']['login'] = 'sa'
default['sql']['user']['sa']['password'] = 'Test1234!'
default['sql']['user']['eddsdbo']['login'] = 'eddsdbo'
default['sql']['user']['eddsdbo']['password'] = 'Test1234!'
default['sql']['instance_name']['primary'] = 'MSSQLSERVER'
default['sql']['instance_name']['distributed'] = "MSSQLDIST"
default['sql']['instances'] = [default['sql']['instance_name']['primary'], default['sql']['instance_name']['distributed']]
default['sql']['directories']['backup'] = 'c:\bak'
default['sql']['directories']['logs'] = 'c:\ldf'
default['sql']['directories']['data'] = 'c:\mdf'
default['sql']['directories']['fulltext'] = 'c:\ndf'

default['service_bus']['defect_windows_update']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\Service_Bus_Defect_Windows_Update'
default['service_bus']['defect_windows_update']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Service_Bus_Defect_Windows_Update"
default['service_bus']['defect_windows_update']['install']['file_name'] = 'AppServer-KB3086798-x64-EN.exe'
default['service_bus']['run_as_account'] = default['windows']['user']['admin']['login']
default['service_bus']['run_as_account_password'] = default['windows']['user']['admin']['password']
default['service_bus']['services']['service_bus_gateway'] = 'Service Bus Gateway'
default['service_bus']['services']['service_bus_message_broker'] = 'Service Bus Message Broker'
default['service_bus']['services']['service_bus_resource_provider'] = 'Service Bus Resource Provider'
default['service_bus']['services']['service_bus_vss'] = 'Service Bus VSS'
default['service_bus']['services']['service_bus_FabricHostSvc'] = 'FabricHostSvc'

default['secret_store']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\SecretStore'
default['secret_store']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\SecretStore"
default['secret_store']['install']['file_name'] = 'Relativity.SecretStore.Installer.exe'
default['secret_store']['install']['client']['location'] = 'C:\\\"Program Files\"\\\"Relativity Secret Store\"\\Client\\secretstore.exe'
default['secret_store']['service']['port'] = '9090'
default['secret_store']['init_result'] = 'C:\\\"Program Files\"\\\"Relativity Secret Store\"\\Client\\init_output.txt'
default['secret_store']['unseal_key_identifier'] = 'UNSEALKEY'
default['secret_store']['unseal_key'] = 'C:\\Program Files\\Relativity Secret Store\\unseal.txt'
# default['secret_store']['unseal_key']['save_to_local_file_destination_folder'] = 'C:\\Relativity_Secret_Store_Unseal_Key'
# default['secret_store']['unseal_key']['save_to_local_file_destination_path'] = "#{default['secret_store']['unseal_key']['save_to_local_file_destination_folder']}\\relativity_secret_store_unseal_key.txt"

default['relativity']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\Relativity'
default['relativity']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Relativity"
default['relativity']['install']['file_name'] = 'Relativity.exe'
default['relativity']['install']['response_file_destination_location'] = "#{default['relativity']['install']['destination_folder']}\\RelativityResponse.txt"
default['relativity']['services_url'] = "http://" + node['fqdn'] +"/Relativity.Services"
default['relativity']['admin']['login'] = 'relativity.admin@relativity.com'
default['relativity']['admin']['password'] = 'Test1234!'
default['relativity']['service_account']['login'] = ''
default['relativity']['service_account']['password'] = ''
default['relativity']['processing']['source']['location'] = "\\\\#{default['windows']['hostname']}\\ProcessingSourceLocation"
default['relativity']['response_file']['file_name_original'] = 'RelativityResponse_Original.txt'
default['relativity']['response_file']['file_name'] = 'RelativityResponse.txt'
default['relativity']['response_file']['source_folder'] = default['relativity']['install']['source_folder']
default['relativity']['response_file']['destination_folder'] = default['relativity']['install']['destination_folder']
default['relativity']['response_file']['parsed_values'] = Array.new
default['relativity']['response_file']['replacement_values'] =
[
    {
        # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
        name: "INSTALLPRIMARYDATABASE", 
        value: "1"
    },
    {
        # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
        name: "INSTALLDISTRIBUTEDDATABASE", 
        value: "0"
    },
    {
        # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
        name: "INSTALLAGENTS", 
        value: "1"
    },
    {
        # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
        name: "INSTALLWEB", 
        value: "1"
    },        
    {
        # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
        name: "INSTALLSERVICEBUS", 
        value: "1"
    },
    {
        # Target directory for local installation files.
        name: "INSTALLDIR", 
        value: "C:\\Program Files\\kCura Corporation\\Relativity\\"
    },
    {
        # The Primary SQL Server Instance Name.
        name: "PRIMARYSQLINSTANCE", 
        value: "#{default['windows']['hostname']}"
    },
    {
        # The password for the EDDSDBO account on the SQL Primary SQL Instance
        name: "EDDSDBOPASSWORD", 
        value: "#{default['sql']['user']['eddsdbo']['password']}"
    },       
    {
        # Domain (or Workgroup) and Username of the Relativity Service Account Windows login.
        name: "SERVICEUSERNAME", 
        value: "#{default['windows']['hostname']}\\#{default['windows']['user']['admin']['login']}"
    },
    {
        # Password for the SERVICEUSERNAME.
        name: "SERVICEPASSWORD", 
        value: "#{default['windows']['user']['admin']['password']}"
    },
    {
        # Whether or not to use WinAuth to connect to the SQL Server.
        name: "USEWINAUTH", 
        value: "1"
    },
    {
        # The name of a SQL Server login. This property is only needed if USEWINAUTH is set to 0.
        name: "SQLUSERNAME", 
        value: ""
    },        
    {
        # The password for the SQLUSERNAME. This property is only needed if USEWINAUTH is set to 0
        name: "SQLPASSWORD", 
        value: ""
    },
    {
        # Whether or not to keep current connection strings in config files on upgrade.
        name: "KEEPCONNECTIONSTRINGS", 
        value: "1"
    },
    {
        # Target UNC path for the default file repository.
        name: "DEFAULTFILEREPOSITORY", 
        value: "\\\\#{default['windows']['hostname']}\\Fileshare"
    },
    {
        # Target UNC path for the EDDS File Share.
        name: "EDDSFILESHARE", 
        value: "\\\\#{default['windows']['hostname']}\\Fileshare\\EDDS"
    },      
    {
        # Target UNC path for the viewer cache location.
        name: "CACHELOCATION", 
        value: "\\\\#{default['windows']['hostname']}\\ViewerCache"
    },
    {
        # Target UNC path for the dtSearch Indexes to be stored.
        name: "DTSEARCHINDEXPATH", 
        value: "\\\\#{default['windows']['hostname']}\\dtSearchIndexes"
    },
    {
        # The name of the Relativity instance.
        name: "RELATIVITYINSTANCENAME", 
        value: "#{default['windows']['hostname']}"
    },
    {
        # The name of the Relativity instance.
        name: "ADMIN_EMAIL", 
        value: ""
    },
    {
        # The name of the Relativity instance.
        name: "SERVICEACCOUNT_EMAIL", 
        value: "#{default['relativity']['service_account']['login']}"
    },
    {
        # The name of the Relativity instance.
        name: "ADMIN_PASSWORD", 
        value: ""
    },
    {
        # The name of the Relativity instance.
        name: "SERVICEACCOUNT_PASSWORD", 
        value: "#{default['relativity']['service_account']['password']}"
    },
    {
        # The Distributed SQL Server Instance Name.
        name: "DISTRIBUTEDSQLINSTANCE", 
        value: ""
    },
    {
        # Target directory for the database backup (.bak) files.
        name: "DATABASEBACKUPDIR", 
        value: "C:\\Backup"
    },
    {
        # Target directory for the database log (.ldf) files.
        name: "LDFDIR", 
        value: "C:\\Logs"
    },
    {
        # Target directory for the database data (.mdf) files.
        name: "MDFDIR", 
        value: "C:\\Data"
    },        
    {
        # Target directory for the database full text index (.ndf) files.
        name: "FULLTEXTDIR", 
        value: "C:\\FullText"
    },
    {
        # Whether or not to create the default agents.
        name: "DEFAULTAGENTS", 
        value: "1"
    },
    {
        # Whether or not to enable win auth for your Relativity environment.
        name: "ENABLEWINAUTH", 
        value: "0"
    },
    {
        # Used to configure Relativity with the messaging provider you are using.
        name: "SERVICEBUSPROVIDER", 
        value: "Windows"
    },
    {
        # Fully qualified domain name of your message bus.
        name: "SERVERFQDN", 
        value: "localhost"
    },
    {
        # Shared access key or admin password used to authenticate to the bus.
        name: "SHAREDACCESSKEY", 
        value: "guest"
    },
    {
        # Shared access key name or admin user used to authenticate to the bus.
        name: "SHAREDACCESSKEYNAME", 
        value: "guest"
    },
    {
        # Service namespace used when creating entities on the bus.
        name: "SERVICENAMESPACE", 
        value: "Relativity"
    },
    {
        # Enable TLS communication when connecting to the service bus.
        name: "TLSENABLED", 
        value: "1"
    }
]

default['invariant']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm\\Development\\Chandra\\DevVm_Install_Files\\Invariant'
default['invariant']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Invariant"
default['invariant']['install']['file_name'] = 'Invariant.exe'
default['invariant']['install']['response_file_destination_location'] = "#{default['invariant']['install']['destination_folder']}\\InvariantResponse.txt"
default['invariant']['response_file']['file_name_original'] = 'InvariantResponse_Original.txt'
default['invariant']['response_file']['file_name'] = 'InvariantResponse.txt'
default['invariant']['response_file']['source_folder'] = default['invariant']['install']['source_folder']
default['invariant']['response_file']['destination_folder'] = default['invariant']['install']['destination_folder']
default['invariant']['response_file']['replacements']['QUEUEMANAGERINSTALLPATH'] = 'C:\\Program Files\\kCura Corporation\\Invariant\\QueueManager\\'
default['invariant']['response_file']['parsed_values'] = Array.new
default['invariant']['response_file']['replacement_values'] =
[
    {
        # 0 for: Do not install or uninstall. 1 for: Install or upgrade.
        name: "INSTALLQUEUEMANAGER", 
        value: "1"
    },
    {
        # 0 for: Do not install or uninstall. 1 for: Install or upgrade.
        name: "INSTALLWORKER", 
        value: "1"
    },
    {
        # Target SQL server for Invariant install
        name: "SQLINSTANCE", 
        value: "#{default['windows']['hostname']}"
    },
    {
        # Target SQL server port for Invariant install. This value only needs to be set it the default value (1433) is not being used.
        name: "SQLINSTANCEPORT", 
        value: ""
    },
    {
        # SQL instance for Relativity install
        name: "RELATIVITYSQLINSTANCE", 
        value: "#{default['windows']['hostname']}"
    },
    {
        # Target SQL server port for Relativity install. This value only needs to be set it the default value (1433) is not being used.
        name: "RELATIVITYSQLINSTANCEPORT", 
        value: ""
    },
    {
        # Windows username to run queue manager service as
        name: "SERVICEUSERNAME", 
        value: "#{default['windows']['hostname']}\\#{default['windows']['user']['admin']['login']}"
    },
    {
        # Password for Windows username to run queue manager service as
        name: "SERVICEPASSWORD", 
        value: "#{default['windows']['user']['admin']['password']}"
    },
    {
        # The EDDSDBO password for the target SQL instance
        name: "EDDSDBOPASSWORD", 
        value: "#{default['sql']['user']['eddsdbo']['password']}"
    },
    {
        # Whether or not to use Windows authorization for SQL access
        name: "USEWINAUTH", 
        value: "1"
    },
    {
        # The name of a SQL Server login. This property is only needed if USEWINAUTH is set to 0.
        name: "SQLUSERNAME", 
        value: ""
    },
    {
        # The password for the SQLUSERNAME. This property is only needed if USEWINAUTH is set to 0.
        name: "SQLPASSWORD", 
        value: ""
    },
    {
        # The file share for worker files
        name: "WORKERNETWORKPATH", 
        value: "\\\\#{default['windows']['hostname']}\\InvariantNetworkShare\\"
    },
    {
        # The URL for the Identity Server for user authentication when running the RPC
        name: "IDENTITYSERVERURL", 
        value: "http://#{default['windows']['hostname']}/Relativity/Identity"
    },
    {
        # The file path for database data files
        name: "QUEUEMANAGERINSTALLPATH", 
        value: "#{default['invariant']['response_file']['replacements']['QUEUEMANAGERINSTALLPATH']}"
    },
    {
        # The file path for database data files
        name: "MDFDIR", 
        value: "C:\\Data"
    },
    {
        # The file path for database log files
        name: "LDFDIR", 
        value: "C:\\Logs"
    },
    {
        # The file share for dtSearch files
        name: "DTSEARCHINDEXPATH", 
        value: "\\\\#{default['windows']['hostname']}\\dtSearchIndexes"
    },
    {
        # The file share for data files
        name: "DATAFILESNETWORKPATH", 
        value: "\\\\#{default['windows']['hostname']}\\fileshare"
    },
    {
        # The nist package path to install (OPTIONAL)
        name: "NISTPACKAGEPATH", 
        value: ""
    },
    {
        # The install path for worker files
        name: "WORKERINSTALLPATH", 
        value: "C:\\Program Files\\kCura Corporation\\Invariant\\Worker\\"
    }
]

default['sample_workspace_name'] = 'Sample Workspace'
default['sample_data_grid_workspace_name'] = 'Sample Data Grid Workspace'

default['sample_data_population']['config_file_name'] = 'DataPopulateConfiguration.Json'
default['sample_data_population']['config_file_path'] = Chef::Config[:file_cache_path] #Dir.tmpdir
default['sample_data_population']['relativity_admin_account']['login'] = default['relativity']['admin']['login']
default['sample_data_population']['relativity_admin_account']['password'] = default['relativity']['admin']['password']
default['sample_data_population']['number_of_documents'] = '11'
default['sample_data_population']['import_images_with_Documents'] = '$TRUE'
default['sample_data_population']['import_production_images_with_documents'] = '$TRUE'

# Add RAP files to this Array to install them into the sample workspace. Make sure they exist in files/default
default['relativity_apps_to_install'] = [
    "Relativity_App_Smoke_Test.rap",
    "Relativity_App_Data_Sampler.rap",
    "Single File Upload 1.2.0.16 (for Relativity 9.4 - 9.5 - RelOne).rap"
    ]

# Add RAP application GUIDs to this Array to create the packaged agents. Make sure the RAP application is installed in the environment
default['relativity_apps_agents_to_install'] = {
    'Imaging' => "C9E4322E-6BD8-4A37-AE9E-C3C9BE31776B",
    'DocumentViewer' => "5725CAB5-EE63-4155-B227-C74CC9E26A76",
    'Production' => "51B19AB2-3D45-406C-A85E-F98C01B033EC",
    'Processing' => "ED0E23F9-DA60-4298-AF9A-AE6A9B6A9319",
    'SmokeTest' => "0125C8D4-8354-4D8F-B031-01E73C866C7C",
    'DataGridCore' => "6A8C2341-6888-44DA-B1A4-5BDCE0D1A383",
    'DataGridTextMigration' => "684B10BB-3B12-4BB1-83E9-A56A7D6CA67F"
}

# Add resource files to this 2 dimensional Array to push them to Relativity. Make sure they exist in files/default
default['relativity_resource_files_to_push'] = [
    # 2 dimensional -> (resource file name located in files/default, Application Guid)
]

default['services'] =
[
    {
        name: "kCura Service Host Manager", 
        type: "Service",
        serviceBus: "$False",
        location: ""
    },
    {
        name: "kCura EDDS Agent Manager", 
        type: "Service",
        serviceBus: "$False",
        location: ""
    },
    {
        name: "kCura EDDS Web Processing Manager", 
        type: "Service",
        serviceBus: "$False",
        location: ""
    },
    {
        name: "Service Bus Gateway", 
        type: "Service",
        serviceBus: "$True",
        location: ""
    },
    {
        name: "Service Bus Message Broker", 
        type: "Service",
        serviceBus: "$True",
        location: ""
    },
    {
        name: "Service Bus Resource Provider", 
        type: "Service",
        serviceBus: "$True",
        location: ""
    },
    {
        name: "Service Bus VSS", 
        type: "Service",
        serviceBus: "$True",
        location: ""
    },
    {
        name: "elasticsearch-service-x64", 
        type: "Service",
        serviceBus: "$False",
        location: ""
    },
    {
        name: "InvariantWorker", 
        type: "Process",
        serviceBus: "$False",
        location: "C:\\InvariantNetworkShare\\InvariantWorker.exe"
    }
]
