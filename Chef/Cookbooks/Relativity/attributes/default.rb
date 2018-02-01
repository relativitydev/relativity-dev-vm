class ResponseFileValue
    attr_accessor :name, :value
    
    def initialize params = {}
      params.each { |key, value| send "#{key}=", value }
    end
   end

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
default['relativity']['response_file']['file_name'] = 'RelativityResponse.txt'
default['relativity']['response_file']['source_folder'] = default['relativity']['install']['source_folder']
default['relativity']['response_file']['destination_folder'] = default['relativity']['install']['destination_folder']
default['relativity']['response_file']['parsed_values'] = ''
default['relativity']['response_file']['replacement_values'] =
    [
        ResponseFileValue.new({
            # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
            name: "INSTALLPRIMARYDATABASE", 
            value: "1"}),
        ResponseFileValue.new({
            # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
            name: "INSTALLDISTRIBUTEDDATABASE", 
            value: "0"}),
        ResponseFileValue.new({
            # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
            name: "INSTALLAGENTS", 
            value: "1"}),
        ResponseFileValue.new({
            # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
            name: "INSTALLWEB", 
            value: "1"}),        
        ResponseFileValue.new({
            # 0 for: Do not install, do not upgrade, or uninstall if currently installed. 1 for: Install or upgrade.
            name: "INSTALLSERVICEBUS", 
            value: "1"}),
        ResponseFileValue.new({
            # Target directory for local installation files.
            name: "INSTALLDIR", 
            value: "C:\\Program Files\\kCura Corporation\\Relativity\\"}),
        ResponseFileValue.new({
            # The Primary SQL Server Instance Name.
            name: "PRIMARYSQLINSTANCE", 
            value: "#{default['windows']['hostname']}"}),
        ResponseFileValue.new({
            # The password for the EDDSDBO account on the SQL Primary SQL Instance
            name: "EDDSDBOPASSWORD", 
            value: "#{default['sql']['user']['eddsdbo']['password']}"}),       
        ResponseFileValue.new({
            # Domain (or Workgroup) and Username of the Relativity Service Account Windows login.
            name: "SERVICEUSERNAME", 
            value: "#{default['windows']['user']['admin']['login']}"}),
        ResponseFileValue.new({
            # Password for the SERVICEUSERNAME.
            name: "SERVICEPASSWORD", 
            value: "#{default['windows']['user']['admin']['password']}"}),
        ResponseFileValue.new({
            # Whether or not to use WinAuth to connect to the SQL Server.
            name: "USEWINAUTH", 
            value: "1"}),
        ResponseFileValue.new({
            # The name of a SQL Server login. This property is only needed if USEWINAUTH is set to 0.
            name: "SQLUSERNAME", 
            value: ""}),        
        ResponseFileValue.new({
            # The password for the SQLUSERNAME. This property is only needed if USEWINAUTH is set to 0
            name: "SQLPASSWORD", 
            value: ""}),
        ResponseFileValue.new({
            # Whether or not to keep current connection strings in config files on upgrade.
            name: "KEEPCONNECTIONSTRINGS", 
            value: "1"}),
        ResponseFileValue.new({
            # Target UNC path for the default file repository.
            name: "DEFAULTFILEREPOSITORY", 
            value: "\\\\#{default['windows']['hostname']}\\Fileshare"}),
        ResponseFileValue.new({
            # Target UNC path for the EDDS File Share.
            name: "EDDSFILESHARE", 
            value: "\\\\#{default['windows']['hostname']}\\Fileshare\EDDS"}),      
        ResponseFileValue.new({
            # Target UNC path for the viewer cache location.
            name: "CACHELOCATION", 
            value: "\\\\#{default['windows']['hostname']}\\ViewerCache"}),
        ResponseFileValue.new({
            # Target UNC path for the dtSearch Indexes to be stored.
            name: "DTSEARCHINDEXPATH", 
            value: "\\\\#{default['windows']['hostname']}\\dtSearchIndexes"}),
        ResponseFileValue.new({
            # The name of the Relativity instance.
            name: "RELATIVITYINSTANCENAME", 
            value: "#{default['windows']['hostname']}"}),
        ResponseFileValue.new({
            # The Distributed SQL Server Instance Name.
            name: "DISTRIBUTEDSQLINSTANCE", 
            value: ""}),
        ResponseFileValue.new({
            # Target directory for the database backup (.bak) files.
            name: "DATABASEBACKUPDIR", 
            value: "C:\\Backup"}),
        ResponseFileValue.new({
            # Target directory for the database log (.ldf) files.
            name: "LDFDIR", 
            value: "C:\\Logs"}),
        ResponseFileValue.new({
            # Target directory for the database data (.mdf) files.
            name: "MDFDIR", 
            value: "C:\\Data"}),        
        ResponseFileValue.new({
            # Target directory for the database full text index (.ndf) files.
            name: "FULLTEXTDIR", 
            value: "C:\\FullText"}),
        ResponseFileValue.new({
            # Whether or not to create the default agents.
            name: "DEFAULTAGENTS", 
            value: "1"}),
        ResponseFileValue.new({
            # Whether or not to enable win auth for your Relativity environment.
            name: "ENABLEWINAUTH", 
            value: "0"})
    ]

default['invariant']['install']['source_folder'] = '\\\\kcura.corp\\shares\\Development\\DevEx\\DevVm_Install_Files\\Invariant'
default['invariant']['install']['destination_folder'] = "#{default['file']['installers']['default_destination_folder']}\\Invariant"
default['invariant']['install']['file_name'] = 'Invariant.exe'
default['invariant']['install']['response_file_destination_location'] = "#{default['invariant']['install']['destination_folder']}\\InvariantResponse.txt"
default['invariant']['response_file']['file_name'] = 'InvariantResponse.txt'
default['invariant']['response_file']['source_folder'] = default['invariant']['install']['source_folder']
default['invariant']['response_file']['destination_folder'] = default['invariant']['install']['destination_folder']
default['invariant']['response_file']['parsed_values'] = ''
default['invariant']['response_file']['replacement_values'] =
[
    ResponseFileValue.new({
        # 0 for: Do not install or uninstall. 1 for: Install or upgrade.
        name: "INSTALLDATABASE", 
        value: "1"}),
    ResponseFileValue.new({
        # 0 for: Do not install or uninstall. 1 for: Install or upgrade.
        name: "INSTALLQUEUEMANAGER", 
        value: "1"}),
    ResponseFileValue.new({
        # 0 for: Do not install or uninstall. 1 for: Install or upgrade.
        name: "INSTALLWORKER", 
        value: "1"}),
    ResponseFileValue.new({
        # Target SQL server for Invariant install
        name: "SQLINSTANCE", 
        value: "#{default['windows']['hostname']}"}),
    ResponseFileValue.new({
        # Target SQL server port for Invariant install. This value only needs to be set it the default value (1433) is not being used.
        name: "SQLINSTANCEPORT", 
        value: ""}),
    ResponseFileValue.new({
        # SQL instance for Relativity install
        name: "RELATIVITYSQLINSTANCE", 
        value: "#{default['windows']['hostname']}"}),
    ResponseFileValue.new({
        # Target SQL server port for Relativity install. This value only needs to be set it the default value (1433) is not being used.
        name: "RELATIVITYSQLINSTANCEPORT", 
        value: ""}),
    ResponseFileValue.new({
        # Windows username to run queue manager service as
        name: "SERVICEUSERNAME", 
        value: "#{default['windows']['user']['admin']['login']}"}),
    ResponseFileValue.new({
        # Password for Windows username to run queue manager service as
        name: "SERVICEPASSWORD", 
        value: "#{default['windows']['user']['admin']['password']}"}),
    ResponseFileValue.new({
        # The EDDSDBO password for the target SQL instance
        name: "EDDSDBOPASSWORD", 
        value: "#{default['sql']['user']['eddsdbo']['password']}"}),
    ResponseFileValue.new({
        # Whether or not to use Windows authorization for SQL access
        name: "USEWINAUTH", 
        value: "1"}),
    ResponseFileValue.new({
        # The name of a SQL Server login. This property is only needed if USEWINAUTH is set to 0.
        name: "SQLUSERNAME", 
        value: ""}),
    ResponseFileValue.new({
        # The password for the SQLUSERNAME. This property is only needed if USEWINAUTH is set to 0.
        name: "SQLPASSWORD", 
        value: ""}),
    ResponseFileValue.new({
        # The file share for worker files
        name: "WORKERNETWORKPATH", 
        value: "\\\\#{default['windows']['hostname']}>\\InvariantNetworkShare\\"}),
    ResponseFileValue.new({
        # The URL for the Identity Server for user authentication when running the RPC
        name: "IDENTITYSERVERURL", 
        value: "http://#{default['windows']['hostname']}/Relativity/Identity"}),
    ResponseFileValue.new({
        # The file path for database data files
        name: "MDFDIR", 
        value: "C:\\Data"}),
    ResponseFileValue.new({
        # The file path for database log files
        name: "LDFDIR", 
        value: "C:\\Logs"}),
    ResponseFileValue.new({
        # The file share for dtSearch files
        name: "DTSEARCHINDEXPATH", 
        value: "\\\\#{default['windows']['hostname']}\\dtSearchIndexes"}),
    ResponseFileValue.new({
        # The file share for data files
        name: "DATAFILESNETWORKPATH", 
        value: "\\\\#{default['windows']['hostname']}\\fileshare"}),
    ResponseFileValue.new({
        # The nist package path to install (OPTIONAL)
        name: "NISTPACKAGEPATH", 
        value: ""}),
    ResponseFileValue.new({
        # The install path for database utilities (NISTUtility, DbUpdater, Deployment)
        name: "DATABASEINSTALLPATH", 
        value: "C:\\Program Files\\kCura Corporation\\Invariant\\Database\\"}),
    ResponseFileValue.new({
        # The install path for queue manager files
        name: "QUEUEMANAGERINSTALLPATH", 
        value: "C:\\Program Files\\kCura Corporation\\Invariant\\QueueManager\\"}),
    ResponseFileValue.new({
        # The install path for worker files
        name: "WORKERINSTALLPATH", 
        value: "C:\\Program Files\\kCura Corporation\\Invariant\\Worker\\"})
]

default['sample_workspace_name'] = 'Sample Workspace'

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

# Add resource files to this 2 dimensional Array to push them to Relativity. Make sure they exist in files/default
default['relativity_resource_files_to_push'] = [
    # 2 dimensional -> (resource file name located in files/default, Application Guid)
]
