custom_log 'custom_log' do msg 'Starting PowerShell Module Add Workspaces' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Add Workspaces
custom_log 'custom_log' do msg 'Adding Workspaces' end

powershell_script 'add_workspaces' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    New-Workspace -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName 'Sample Workspace' -EnableDataGrid false
    
    New-Workspace -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName '#{node['sample_data_grid_workspace_name']}' -EnableDataGrid true
    EOH
end

custom_log 'custom_log' do msg 'Added Workspaces' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Add Workspaces\n\n\n" end