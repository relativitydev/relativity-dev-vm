custom_log 'custom_log' do msg 'Starting PowerShell Module Add Agent Server to Default Resource Pool' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Add Agent Server
custom_log 'custom_log' do msg 'Adding Agent Server' end

powershell_script 'add_agent_server' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Add-AgentServerToDefaultResourcePool -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']}
    EOH
end

custom_log 'custom_log' do msg 'Added Agent Server' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Add Agent Server to Default Resource Pool\n\n\n" end