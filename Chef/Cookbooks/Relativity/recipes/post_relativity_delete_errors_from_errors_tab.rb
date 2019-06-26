custom_log 'custom_log' do msg 'Starting Deleting Errors from Errors Tab' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Generate Import Powershell module code
powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze

# Delete Errors from Errors Tab
custom_log 'custom_log' do msg 'Deleting Errors from Errors Tab' end

powershell_script 'delete_errors' do
  code <<-EOH
    #{IMPORT_MODULE}
    Remove-RelativityErrors -RelativityInstanceName #{node['windows']['new_computer_name']} -SqlAdminUserName #{node['sql']['user']['sa']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']}
    EOH
end

custom_log 'custom_log' do msg 'Deleted Errors from Errors Tab' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Deleting Errors from Errors Tab\n\n\n" end