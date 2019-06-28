custom_log 'custom_log' do msg 'Starting PowerShell Module Sample' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Create Agent
custom_log 'custom_log' do msg 'Creating new Agent' end

powershell_script 'create_new_agent' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Add-AgentByName -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -AgentNames "Data Grid Audit Manager"
    EOH
end

custom_log 'custom_log' do msg 'Created new agent' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished PowerShell Module Sample\n\n\n" end