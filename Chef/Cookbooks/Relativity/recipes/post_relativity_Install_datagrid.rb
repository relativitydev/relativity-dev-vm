custom_log 'custom_log' do msg 'Starting Install Data Grid' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Java should have been installed in the Windows_Install_Software recipe and env variables set in datagrid-presetup

# Install Elastic Search
include_recipe 'Relativity::post_relativity_Install_datagrid_elasticsearch_setup'

# # Configure Environment for Data Grid
include_recipe 'Relativity::post_relativity_Install_datagrid_configure_environment'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Install Data Grid\n\n\n" end