custom_log 'custom_log' do msg 'Starting PowerShell Module Add Workspaces' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Add Workspaces
custom_log 'custom_log' do msg 'Adding Workspaces' end

# Logging Powreshell Module input parameters for troubleshooting
custom_log 'custom_log' do msg "-ModuleLoadPath: #{node['powershell_module']['import_module']}" end
custom_log 'custom_log' do msg "-RelativityInstanceName: #{node['windows']['new_computer_name']}" end
custom_log 'custom_log' do msg "-RelativityAdminUserName: #{node['relativity']['admin']['login']}" end
custom_log 'custom_log' do msg "-RelativityAdminPassword: #{node['relativity']['admin']['password']}" end
custom_log 'custom_log' do msg "-SqlAdminUserName: #{node['sql']['user']['eddsdbo']['login']}" end
custom_log 'custom_log' do msg "-SqlAdminPassword: #{node['sql']['user']['sa']['password']}" end
custom_log 'custom_log' do msg "-WorkspaceName: #{node['sample_workspace_name']}" end
custom_log 'custom_log' do msg "-EnableDataGrid: false" end
custom_log 'custom_log' do msg "For Datagrid enabled Workspace: -WorkspaceName: #{node['sample_data_grid_workspace_name']}" end
custom_log 'custom_log' do msg "For Datagrid enabled Workspace: -EnableDataGrid: true" end

powershell_script 'add_workspaces' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    New-Workspace -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName '#{node['sample_workspace_name']}' -EnableDataGrid false -ErrorAction Stop

    New-Workspace -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName '#{node['sample_data_grid_workspace_name']}' -EnableDataGrid true -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Added Workspaces' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Add Workspaces\n\n\n" end