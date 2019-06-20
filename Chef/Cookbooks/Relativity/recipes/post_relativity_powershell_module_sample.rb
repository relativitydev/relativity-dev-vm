custom_log 'custom_log' do msg 'Starting PowerShell Module setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Generate Import Powershell module code
powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze

# Create Agent
custom_log 'custom_log' do msg 'Creating new Agent' end

powershell_script 'create_new_agent' do
  code <<-EOH
    #{IMPORT_MODULE}
    Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -AgentNames "Data Grid Audit Manager"
    EOH
end

custom_log 'custom_log' do msg 'Created new agent' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{en_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{endtime.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module setup\n\n\n" end