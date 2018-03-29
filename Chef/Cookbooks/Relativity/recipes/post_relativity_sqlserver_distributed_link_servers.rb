custom_log 'custom_log' do msg 'Link distributed SQL Servers' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

hostname = node['windows']['hostname']

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

for instance in node['sql']['instances']
    if instance == node['sql']['instance_name']['primary']
        connected_instance = hostname
    else
        connected_instance = "#{hostname}\\#{instance}"
    end

    for instance_entry in node['sql']['instances']
        if instance_entry == node['sql']['instance_name']['primary']
            linked_server = hostname
        else
            linked_server = "#{hostname}\\#{instance_entry}"
        end
            
        powershell_script 'Add_linked_server' do
            code <<-EOH
            Import-Module "#{sqlps_module_path}"
            Invoke-Sqlcmd -username '#{node['sql']['user']['sa']['login']}' -password '#{node['sql']['user']['sa']['password']}' -serverinstance '#{connected_instance}' -Query "
            
            if not exists(select * from sys.servers where name = '#{linked_server}')
                exec sp_addlinkedserver '#{linked_server}'
                exec sp_serveroption '#{linked_server}', 'data access', 'true'
            GO
            "
            EOH
        end

    end       

end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Linking SQL Servers\n\n\n" end
