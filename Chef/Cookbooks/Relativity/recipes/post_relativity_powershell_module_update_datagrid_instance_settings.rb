custom_log 'custom_log' do msg 'Starting Updating DataGrid Instance Settings' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update Data Grid Instance Settings
custom_log 'custom_log' do msg 'Updating DataGrid Instance Settings' end

powershell_script 'update_datagrid_instance_settings' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    
    # This instance setting doesn't exist in 10.3 Goatsbeard release 9/27/2019 (Chandra)
    # Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name DataGridEndPoint -Section Relativity.DataGrid -NewValue " "  -ErrorAction Stop
    
    Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name AuditDataGridEndPoint -Section kCura.Audit -NewValue http://RelativityDevVm:9200  -ErrorAction Stop
    
    # This instance setting doesn't exist in 10.3 Goatsbeard release 9/27/2019 (Chandra)
    # Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name DataGridSearchIndex  -Section Relativity.DataGrid -NewValue 0  -ErrorAction Stop
    
    Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name ESIndexCreationSettings -Section kCura.Audit " "  -ErrorAction Stop
    
    Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name ESIndexPrefix -Section kCura.Audit -NewValue audit  -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Updated DataGrid Instance Settings' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Updating DataGrid Instance Settings\n\n\n" end