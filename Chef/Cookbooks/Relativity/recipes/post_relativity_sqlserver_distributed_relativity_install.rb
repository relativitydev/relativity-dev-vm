custom_log 'custom_log' do msg 'Running Distributed Sql Server relativity install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

counter = 0

for instance in node['sql']['instances'] do

  if instance != node['sql']['instance_name']['primary']
    bat_filename = "RelativityDistributedInstall_#{counter.to_s}.bat"
    distributed_sql_install_bat_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], bat_filename))

    # Copy the powershell script template file from the cookbook to the Chef cache
    template distributed_sql_install_bat_path do
      variables(
        'eddsdbo_password': node['sql']['user']['eddsdbo']['password'],
        'full_distributed_server_instance_name': "#{node['windows']['hostname']}\\#{instance}",
        'sa_username': node['sql']['user']['sa']['login'],
        'sa_password': node['sql']['user']['sa']['password']
      )
      source 'RelativityDistributedInstall.bat.erb'
    end

    # Run the powershell scripts to execute distributed sql server relativity install
    powershell_script 'Execute distributed Relativity Server Install' do
      code distributed_sql_install_bat_path
    end
  end

  counter = counter + 1
end
end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Distributed Sql Server relativity install\n\n\n" end