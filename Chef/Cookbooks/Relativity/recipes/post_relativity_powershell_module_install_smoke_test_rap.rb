custom_log 'custom_log' do msg 'Starting Installing Smoke Test Rap' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\RA_Smoke_Test_2.2.0.9.rap'))

# Install Smoke Test RAP
custom_log 'custom_log' do msg 'Installing Smoke Test Rap' end

powershell_script 'install_smoke_test_rap_and_create_agents' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Add-InstanceSetting -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -Name "#{node['smoke_test']['instance_setting']['name']}" -Section "#{node['smoke_test']['instance_setting']['section']}" -Description "#{node['smoke_test']['instance_setting']['description']}" -Value "#{node['smoke_test']['instance_setting']['value']}"
    Add-ApplicationFromRapFile -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName "#{node['sample_data_grid_workspace_name']}" -FilePath #{rap_file_path}
    Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -AgentNames "#{node["smoke_test_agent"]["analysis"]}"
    Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -AgentNames "#{node["smoke_test_agent"]["runner"]}"
    Watch-SmokeTestForTestToFinish -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -WorkspaceName "#{node['sample_data_grid_workspace_name']}" -TimeoutValueInMinutes 15
    EOH
end

custom_log 'custom_log' do msg 'Installed Smoke Test Rap' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Installing Smoke Test Rap\n\n\n" end