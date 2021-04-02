custom_log 'custom_log' do msg 'Starting PowerShell Module Add Data to Workspaces' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Add Data to Workspaces
custom_log 'custom_log' do msg 'Adding Data to Workspaces' end

powershell_script 'add_data_to_workspaces' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -WorkspaceName 'Sample Workspace' -FileType #{node['sample_data_population']['document_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -WorkspaceName 'Sample Workspace' -FileType #{node['sample_data_population']['image_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    
	  New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -WorkspaceName '#{node['sample_data_grid_workspace_name']}' -FileType #{node['sample_data_population']['document_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -WorkspaceName '#{node['sample_data_grid_workspace_name']}' -FileType #{node['sample_data_population']['image_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    EOH
end

custom_log 'custom_log' do msg 'Added Data to Workspaces' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Add Workspaces Data to Workspaces\n\n\n" end