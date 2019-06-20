
# Copy the necessary files from the cookbook to the Chef cache
files_to_copy = {
  'DevVmPsModules.dll',
  'Helpers.dll',
}

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