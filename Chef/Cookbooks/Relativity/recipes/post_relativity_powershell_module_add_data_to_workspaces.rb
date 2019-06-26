custom_log 'custom_log' do msg 'Starting PowerShell Module Add Data to Workspaces' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Generate Import Powershell module code
powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\DevVmPsModules.dll'))
IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -Verbose -ErrorAction Stop".freeze

# Add Workspaces
custom_log 'custom_log' do msg 'Adding Data to Workspaces' end

powershell_script 'add_data_to_workspaces' do
  code <<-EOH
    #{IMPORT_MODULE}
    New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -WorkspaceName '#{node['sample_workspace_name']}' -FileType #{node['sample_data_population']['document_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -WorkspaceName '#{node['sample_workspace_name']}' -FileType #{node['sample_data_population']['image_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    
	  New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -WorkspaceName '#{node['sample_data_grid_workspace_name']}' -FileType #{node['sample_data_population']['document_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    New-Documents -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -WorkspaceName '#{node['sample_data_grid_workspace_name']}' -FileType #{node['sample_data_population']['image_type']} -FileCount #{node['sample_data_population']['number_of_documents']} -ResourceFilePath #{node['sample_data_population']['resource_path']}
    EOH
end

custom_log 'custom_log' do msg 'Added Data to Workspaces' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Add Workspaces Data to Workspaces\n\n\n" end