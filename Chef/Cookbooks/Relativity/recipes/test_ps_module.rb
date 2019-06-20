# Copy Relativity API Dlls
relativity_files_to_copy = [
  "BCrypt.Net.dll",
  "FreeImageNET.dll",
  "Ionic.Zip.dll",
  "kCura.dll",
  "kCura.ImageValidator.dll",
  "kCura.OI.FileID.dll",
  "kCura.Relativity.Client.dll",
  "Newtonsoft.Json.dll",
  "oi.dll",
  "Polly.dll",
  "Relativity.API.dll",
  "Relativity.dll",
  "Relativity.Kepler.dll",
  "Relativity.Logging.dll",
  "Relativity.Logging.Interfaces.dll",
  "Relativity.OAuth2Client.dll",
  "Relativity.OAuth2Client.IdentityModel.dll",
  "Relativity.OAuth2Client.IdentityModel.Interfaces.dll",
  "Relativity.OAuth2Client.Interfaces.dll",
  "Relativity.Services.DataContracts.dll",
  "Relativity.Services.Interfaces.dll",
  "Relativity.Services.Interfaces.Private.dll",
  "Relativity.Services.ServiceProxy.dll",
  "Relativity.Telemetry.APM.dll",
  "Relativity.Telemetry.DataContracts.Shared.dll",
  "Relativity.Telemetry.MetricsCollection.dll"
]

relativity_files_to_copy.each do |file|
  custom_log 'custom_log' do msg "Copying file from cookbook to the Chef cache - #{file}" end
  source_file_path = "C:\\Program Files\\kCura Corporation\\Relativity\\ServiceHost\\#{file}"
  destination_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], file))
  powershell_script 'copying dlls' do
    code <<-EOH
      Copy-Item "#{source_file_path}" -Destination "#{destination_file_path}"
      EOH
  end
  custom_log 'custom_log' do msg "Copied file from cookbook to the Chef cache - #{file}" end
end

# Copy the necessary files from the cookbook to the Chef cache
files_to_copy = [
  'DevVmPsModules.dll',
  'Helpers.dll'
]

files_to_copy.each do |file|
  custom_log 'custom_log' do msg "Copying file from cookbook to the Chef cache - #{file}" end
  file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], file))
  cookbook_file file_path do
    source file
  end
  custom_log 'custom_log' do msg "Copied file from cookbook to the Chef cache - #{file}" end
end

# Powershell module import line.
powershell_module_dll_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_full_path}\" -ErrorAction Stop".freeze

# Create the initial farm, if needed
custom_log 'custom_log' do msg 'Creating new Agent' end
powershell_script 'create new agent' do
  code <<-EOH
    #{IMPORT_MODULE}
    Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -AgentNames "Data Grid Audit Manager"
    EOH
end
custom_log 'custom_log' do msg 'Created new agent' end