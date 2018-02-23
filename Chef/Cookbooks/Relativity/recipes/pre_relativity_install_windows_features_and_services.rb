custom_log 'custom_log' do msg 'Starting Windows Features and Services install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Features:

# - .NET Framework 3.5 Features => NET-Framework-Features
# 	- .NET Framework 3.5 (INCLUDES .net 2.0 AND 3.0) => NET-Framework-Core
# 	- HTTP Activation => NET-HTTP-Activation
# 	- Non-HTTP Activation => NET-Non-HTTp-Activ
# - .NET Framework 4.5 Features => NET-Framework-45-Features
# 	- .NET Framework 4.5 => NET-Framework-45-Core
# 	- APS.NET 4.5 => NET-Framework-45-ASPNET
# 	- WCF Services => NET-WCF-Services45
# 		- HTTP Activation =>  NET-WCF-HTTP-Activation45
# 		- Named Pipe Activation => NET-WCF-Pipe-Activation45
# 		- TCP Activation => NET-WCF-TCP-Activation45
# 		- TCP Port Sharing => NET-WCF-TCP-PortSharing45

# Services:

# - Web Server => Web-WebServer
# 	- Common HTTP Features => Web-Common-Http
# 		- Default Document => Web-Default-Doc
# 		- Directory Browsing => Web-Dir-Browsing
# 		- HTTP Errors => Web-Http-Errors
# 		- Static Content => Web-Static-Content
# 		- HTTP Redirection => Web-Http-Redirect
# 	- Health and Diagnostics => Web-Health
# 		- HTTP Logging => Web-Http-Logging
# 		- Request Monitor => Web-Request-Monitor
# 		- Tracing => Web-Http-Tracing
# 	- Performance => Web-Performance
# 		- Static Content Compression => Web-Stat-Compression
# 	- Security => Web-Security
# 		- Request Filtering => Web-Filtering
# 		- Basic Authentication => Web-Basic-Auth
# 		- Windows Authentication => Web-Windows-Auth
# 	- Application Development => Web-App-Dev
# 		- .NET Extensibility 3.5 => Web-Net-Ext
# 		- .NET Extensibility 4.5 => Web-Net-Ext45
# 		- ASP.NET 3.5 => Web-Asp-Net
# 		- ASP.NET 4.5 => Web-Asp-Net45
# 		- ISAPI Extensions => Web-ISAPI-Ext
# 		- ISAPI Filters => Web-ISAPI-Filter
# 		- WebSocket Protocol => Web-WebSockets
#   - Management Tools => Web-Mgmt-Tools
#     - IIS Management Console => Web-Mgmt-Console
#     - IIS 6 Management Compatibility => Web-Mgmt-Compat
#       - IIS 6 Metabase Compatibility => Web-Metabase
#       - IIS 6 Management Console => Web-Lgcy-Mgmt-Console
#       - IIS 6 Scripting Tools => Web-Lgcy-Scripting
#       - IIS 6 WMI Compatibility => Web-WMI
#     - IIS Management Scripts and Tools => Web-Scripting-Tools
#     - Management Service => Web-Mgmt-Service

# Install Windows Features and Services
features = %w(NET-Framework-Features NET-Framework-Core NET-HTTP-Activation NET-Non-HTTp-Activ NET-Framework-45-Features NET-Framework-45-Core NET-Framework-45-ASPNET NET-WCF-Services45 NET-WCF-HTTP-Activation45 NET-WCF-Pipe-Activation45 NET-WCF-TCP-Activation45 NET-WCF-TCP-PortSharing45 Web-WebServer Web-Common-Http Web-Default-Doc Web-Dir-Browsing Web-Http-Errors Web-Static-Content Web-Http-Redirect Web-Health Web-Http-Logging Web-Request-Monitor Web-Http-Tracing Web-Performance Web-Stat-Compression Web-Security Web-Filtering Web-Basic-Auth Web-Windows-Auth Web-App-Dev Web-Net-Ext Web-Net-Ext45 Web-Asp-Net Web-Asp-Net45 Web-ISAPI-Ext Web-ISAPI-Filter Web-WebSockets Web-Mgmt-Tools Web-Mgmt-Console Web-Mgmt-Compat Web-Metabase Web-Lgcy-Mgmt-Console Web-Lgcy-Scripting Web-WMI Web-Scripting-Tools Web-Mgmt-Service)

features.each do |feature|
  custom_log 'custom_log' do msg "Installing Windows Feature - #{feature}" end
  dsc_resource "windowsfeature_#{feature}" do
    resource :windowsfeature
    property :ensure, 'Present'
    property :name, feature
  end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Windows Features and Services install\n\n\n" end
