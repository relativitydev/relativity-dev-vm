custom_log 'custom_log' do msg 'Starting Enabling Viewer for RSMF' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update Java Environment Variables
custom_log 'custom_log' do msg 'Enabling Viewer for RSMF' end

powershell_script 'enable_viewer_for_rsmf' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Add-RsmfViewerOverride -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']}  -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Enabled Viewer for RSMF' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Enabling Viewer for RSMF\n\n\n" end