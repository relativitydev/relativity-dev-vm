custom_log 'custom_log' do msg 'Starting Updating Agent Off Hour End Time' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update Agent Off Hour End Time
custom_log 'custom_log' do msg 'Updating Agent Off Hour End Time' end

powershell_script 'update_agent_off_hour_end_time' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -Name AgentOffHourEndTime -Section kCura.EDDS.Agents -NewValue "23:59:59" -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Updated Agent Off Hour End Time' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Updating Agent Off Hour End Time\n\n\n" end