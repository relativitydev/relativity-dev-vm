custom_log 'custom_log' do msg 'Starting PowerShell Module setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy Relativity API Dlls to Chef Cache Location
relativity_api_files_to_copy = [
  "kCura.dll",
  "kCura.Relativity.Client.dll",
  "Relativity.dll",
  "Relativity.API.dll",
  "Relativity.OAuth2Client.Interfaces.dll",
  "Relativity.Services.Interfaces.dll",
  "Relativity.Services.ServiceProxy.dll",
  "Relativity.Services.Interfaces.Private.dll",
  "Relativity.dll"
]
relativity_api_library_files_to_copy = [
  "FreeImageNET.dll", # Required for Import API
  "FreeImage.dll", # Required for Import API
  "oi.dll", # Required for Import API
  "Polly.dll", # Required for Import API
  "Relativity.Logging.dll", # Required for Import API
  "Relativity.Logging.Interfaces.dll", # Required for Import API
]

# Copy ServiceHost files
relativity_api_files_to_copy.each do |source_file_name|
  custom_log 'custom_log' do msg "Copying Relativity API DLL to Chef Cache Location - #{source_file_name}" end

  source_file_full_path = "#{node['powershell_module']['relativity_api_dlls_location']}\\#{source_file_name}"
  destination_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], source_file_name))

  powershell_script 'copying_relativity_api_dll' do
    code <<-EOH
      Copy-Item "#{source_file_full_path}" -Destination "#{destination_file_full_path}"
      EOH
  end
  custom_log 'custom_log' do msg "Copied Relativity API DLL to Chef Cache Location - #{source_file_name}" end
end

# Copy other Library files
relativity_api_library_files_to_copy.each do |source_file_name|
  custom_log 'custom_log' do msg "Copying Relativity API Library DLL to Chef Cache Location - #{source_file_name}" end

  source_file_full_path = "#{node['powershell_module']['relativity_api_dlls_library_location']}\\#{source_file_name}"
  destination_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], source_file_name))

  powershell_script 'copying_relativity_api_library_dll' do
    code <<-EOH
      Copy-Item "#{source_file_full_path}" -Destination "#{destination_file_full_path}"
      EOH
  end
  custom_log 'custom_log' do msg "Copied Relativity API Library DLL to Chef Cache Location - #{source_file_name}" end
end

# Copy oi folder since that is needed for TAPI
source_folder_full_path = "#{node['powershell_module']['relativity_api_dlls_library_location']}\\oi\\"
destination_folder_full_path = win_friendly_path(Chef::Config[:file_cache_path])

powershell_script 'copying_relativity_oi_folder' do
  code <<-EOH
    Copy-Item "#{source_folder_full_path}" -Destination "#{destination_folder_full_path}"  -Force -Recurse
    EOH
end

# Copy PowerShell Module related files from the Cookbook Location to Chef Cache Location
powershell_module_related_files_to_copy = [
  'DevVmPsModules.dll',
  'Helpers.dll',
  'Serilog.dll', # Required for ILogService
  'Serilog.Sinks.Console.dll', # Required for ILogService
  'Serilog.Sinks.Debug.dll', # Required for ILogService
  'Serilog.Sinks.File.dll', # Required for ILogService
  'DbContextHelper.dll',
  'Relativity.Imaging.Services.Interfaces.dll',
  'FaspManager.dll', # Required for Import API
  'Relativity.DataExchange.Client.SDK.dll', # Required for Import API
  'Relativity.DataTransfer.MessageService.dll', # Required for Import API
  'Relativity.Transfer.Client.dll', # Required for Import API
  'Relativity.Transfer.Client.Core.dll', # Required for Import API
  'Relativity.Transfer.Client.Aspera.dll', # Required for Import API
  'Relativity.Transfer.Client.FileShare.dll', # Required for Import API
  'Relativity.Transfer.Client.Http.dll', # Required for Import API
  "Relativity.Kepler.dll", # Required for Object Manager, Default 2.7.0.0
  "Relativity.Services.DataContracts.dll", # Required for Object Manager, Default 13.2.0.0
  "Newtonsoft.Json.dll" # Don't pull this from Relativity, use the dll from the files\default folder
]

powershell_module_related_files_to_copy.each do |source_file_name|
  custom_log 'custom_log' do msg "Copying PowerShell Module related file from Cookbook Location to Chef Cache Location - #{source_file_name}" end

  destination_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], source_file_name))

  cookbook_file destination_file_full_path do
    source source_file_name
  end

  custom_log 'custom_log' do msg "Copied PowerShell Module related file from Cookbook Location to Chef Cache Location - #{source_file_name}" end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module setup\n\n\n" end