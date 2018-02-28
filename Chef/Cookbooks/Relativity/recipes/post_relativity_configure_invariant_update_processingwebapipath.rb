custom_log 'custom_log' do msg 'Starting update ProcessingWebAPIPath' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

# Run update ProcessingWebAPIPath
custom_log 'custom_log' do msg 'Setting CookieSecure Instance Setting to False.' end
powershell_script 'relativity_http_setup' do
    code <<-EOH
    Import-Module "#{sqlps_module_path}"
    Invoke-Sqlcmd -Query "
        UPDATE
            [EDDS].[eddsdbo].[InstanceSetting]
        SET
            [Value] = 'http://#{node['windows']['new_computer_name']}/RelativityWebAPI/'
        WHERE
            [Section] = 'Relativity.Core' AND
            [Name] = 'ProcessingWebAPIPath'
    "
    EOH
end
custom_log 'custom_log' do msg 'Updated ProcessingWebAPIPath.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished CookieSecure Instance Setting Update\n\n\n" end
