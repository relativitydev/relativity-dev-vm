custom_log 'custom_log' do msg 'Starting PowerShell Module Full Processing Setup for Default Resource Pool' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Set up Processing
custom_log 'custom_log' do msg 'Setting up Processing' end

powershell_script 'full_processing_setup' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    New-ProcessingFullSetupAndUpdateDefaultResourcePool -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']}  -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Set up Processing' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Full Processing Setup for Default Resource Pool\n\n\n" end