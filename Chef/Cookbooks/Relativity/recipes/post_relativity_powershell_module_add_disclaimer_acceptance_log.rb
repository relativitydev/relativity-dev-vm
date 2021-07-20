custom_log 'custom_log' do msg 'Starting Installing Disclaimer Acceptance Log Rap' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\Disclaimer_Acceptance_Log (Ninebark 12-0 and Above).rap'))

# Install Disclaimer Acceptance Log Rap
custom_log 'custom_log' do msg 'Installing Disclaimer Acceptance Log Rap' end

powershell_script 'install_disclaimer_acceptance_log_rap' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Add-ApplicationFromRapFile -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName "#{node['sample_workspace_name']}" -FilePath #{rap_file_path}  -ErrorAction Stop
    Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -AgentNames "#{node["disclaimer_acceptance_agent"]}"  -ErrorAction Stop
    Add-DisclaimerRDOs -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName "#{node['sample_workspace_name']}"  -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Installed Disclaimer Acceptance Log Rap' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Installing Disclaimer Acceptance Log Rap\n\n\n" end