custom_log 'custom_log' do msg 'Starting IsOnlineInstance/Telemetry Instance Setting Update' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update IsOnlineInstance
custom_log 'custom_log' do msg 'Updating IsOnlineInstance Instance Setting.' end

powershell_script 'update_instance_setting - IsOnlineInstance' do
  code <<-EOH
  #{node['powershell_module']['import_module']}
  Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name IsOnlineInstance -Section kCura.LicenseManager -NewValue "False"  -ErrorAction Stop
  EOH
end

custom_log 'custom_log' do msg 'Updated IsOnlineInstance Instance Setting.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished IsOnlineInstance/Telemetry Instance Setting Update\n\n\n" end
