custom_log 'custom_log' do msg 'Starting PowerShell Module Resize Databases' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Generate Import Powershell module code
powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze

# Add Agents
custom_log 'custom_log' do msg 'Resizing Databases' end

powershell_script 'resize_databases' do
  code <<-EOH
    #{IMPORT_MODULE}
    Resize-Databases -RelativityInstanceName #{node['windows']['new_computer_name']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']}
    EOH
end

custom_log 'custom_log' do msg 'Resized Databases' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module  Resize Databases\n\n\n" end