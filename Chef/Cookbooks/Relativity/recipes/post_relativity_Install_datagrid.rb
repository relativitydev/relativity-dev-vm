custom_log 'custom_log' do msg 'Starting Install Data Grid' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Java should have been installed in the Windows_Install_Software recipe

# Add Java Home Environment Variables
env 'KCURA_JAVA_HOME' do
  value node['software']['java_runtime']['home']
end

env 'JAVA_HOME' do
  value node['software']['java_runtime']['home']
end

# Add Java Home to the Windows Path
windows_path node['software']['java_runtime']['home'] do
  action :add
end


# Install Elastic Search
include_recipe 'Relativity::post_relativity_Install_datagrid_elasticsearch_setup'

# Prepare DataGrid Workspace
include_recipe 'Relativity::post_relativity_Install_datagrid_prepare_workspace'

# Configure Environment for Data Grid
include_recipe 'Relativity::post_relativity_Install_datagrid_configure_environment'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Install Data Grid\n\n\n" end