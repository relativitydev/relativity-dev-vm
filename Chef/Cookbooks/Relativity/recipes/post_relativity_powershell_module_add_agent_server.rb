custom_log 'custom_log' do msg 'Starting PowerShell Module Add Agent Server to Default Resource Pool' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Generate Import Powershell module code
powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze

# Add Agent Server
custom_log 'custom_log' do msg 'Adding Agent Server' end

powershell_script 'add_agent_server' do
  code <<-EOH
    #{IMPORT_MODULE}
    Add-AgentServerToDefaultResourcePool -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']}
    EOH
end

custom_log 'custom_log' do msg 'Added Agent Server' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Add Agent Server to Default Resource Pool\n\n\n" end