custom_log 'custom_log' do msg 'Starting PowerShell Module Full Processing Setup for Default Resource Pool' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Generate Import Powershell module code
powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\VisualStudioDlls\DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze

# Set up Processing
custom_log 'custom_log' do msg 'Setting up Processing' end

powershell_script 'full_processing_setup' do
  code <<-EOH
    #{IMPORT_MODULE}
    New-ProcessingFullSetupAndUpdateDefaultResourcePool -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']}
    EOH
end

custom_log 'custom_log' do msg 'Set up Processing' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Full Processing Setup for Default Resource Pool\n\n\n" end