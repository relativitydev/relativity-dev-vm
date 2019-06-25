custom_log 'custom_log' do msg 'Starting PowerShell Module setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy Relativity API Dlls to Chef Cache Location
relativity_api_files_to_copy = [
  "kCura.dll",
  "kCura.Relativity.Client.dll",
  "Newtonsoft.Json.dll",
  "Relativity.dll",
  "Relativity.API.dll",
  "Relativity.Kepler.dll",
  "Relativity.OAuth2Client.Interfaces.dll",
  "Relativity.Services.DataContracts.dll",
  "Relativity.Services.Interfaces.dll",
  "Relativity.Services.ServiceProxy.dll",
  "Relativity.Services.Interfaces.Private.dll",
]

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

# Copy PowerShell Module related files from the Cookbook Location to Chef Cache Location
powershell_module_related_files_to_copy = [
  'DevVmPsModules.dll',
  'Helpers.dll',
  'DbContextHelper.dll',
  "kCura.Relativity.DataReaderClient.dll",
  "kCura.Relativity.ImportAPI.dll",
  "kCura.WinEDDS.dll",
  "Resources"
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