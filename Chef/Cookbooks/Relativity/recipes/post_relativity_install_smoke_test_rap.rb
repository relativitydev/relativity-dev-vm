custom_log 'custom_log' do msg 'Starting Installing Smoke Test Rap' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    # Generate Import Powershell module code
    powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
    IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze

    rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\Relativity_Smoke_Test_2.1.0.1.rap'))
        
    # Delete Errors from Errors Tab
    custom_log 'custom_log' do msg 'Installing Smoke Test Rap' end
    
    powershell_script 'install_smoke_test_rap_and_create_agents' do
      code <<-EOH
        #{IMPORT_MODULE}
        Add-ApplicationFromRapFile -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -WorkspaceName "#{node['sample_data_grid_workspace_name']}" -FilePath #{rap_file_path}
        Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -AgentNames "#{node["smoke_test_agent"]["analysis"]}"
        Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -AgentNames "#{node["smoke_test_agent"]["runner"]}"
        EOH
    end
    
    custom_log 'custom_log' do msg 'Installed Smoke Test Rap' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Finished Installing Smoke Test Rap\n\n\n" end