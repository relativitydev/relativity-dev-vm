custom_log 'custom_log' do msg 'Starting Prepare Data Grid Workspace' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

create_workspace "Create Data Grid Workspace" do
	workspace_name node['sample_data_grid_workspace_name']
	relativity_services_url node['relativity']['services_url']
	relativity_username node['sample_data_population']['relativity_admin_account']['login']
	relativity_password node['sample_data_population']['relativity_admin_account']['password']
	powershell_functions_script_path powershell_functions_script_path
end

enable_datagrid_in_workspace "Enable Data Grid in Sample Data Grid Workspace" do
	workspace_name node['sample_data_grid_workspace_name']
	relativity_services_url node['relativity']['services_url']
	relativity_username node['sample_data_population']['relativity_admin_account']['login']
	relativity_password node['sample_data_population']['relativity_admin_account']['password']
	powershell_functions_script_path powershell_functions_script_path
end

install_library_application "Install Data Grid Core" do
    relativity_services_url node['relativity']['services_url']
    workspace_name node['sample_data_grid_workspace_name']
	server_name node['windows']['hostname']
	relativity_username node['sample_data_population']['relativity_admin_account']['login']
	relativity_password node['sample_data_population']['relativity_admin_account']['password']
    application_guid '6A8C2341-6888-44DA-B1A4-5BDCE0D1A383'
    powershell_functions_script_path powershell_functions_script_path
end

install_library_application "Install Data Grid Text Migration" do
    relativity_services_url node['relativity']['services_url']
    workspace_name node['sample_data_grid_workspace_name']
	server_name node['windows']['hostname']
	relativity_username node['sample_data_population']['relativity_admin_account']['login']
	relativity_password node['sample_data_population']['relativity_admin_account']['password']
    application_guid '684B10BB-3B12-4BB1-83E9-A56A7D6CA67F'
    powershell_functions_script_path powershell_functions_script_path
end

enable_datagrid_on_workspace_field "Enable Data Grid on the Extracted Text field" do
	workspace_name node['sample_data_grid_workspace_name']
	field_name 'Extracted Text'
	sqlps_module_path ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')
	sql_server_name node['windows']['hostname']
	sql_server_username node['sql']['user']['sa']['login']
	sql_server_password node['sql']['user']['sa']['password']
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Prepare Data Grid Workspace\n\n\n" end