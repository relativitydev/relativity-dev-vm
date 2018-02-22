custom_log 'custom_log' do msg 'Starting DeveloperMode Instance Setting Update' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Create a path to the SQL file in the Chef cache.
sql_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'sql_relativity_instance_setting_update_developer_mode.sql'))

# Copy the SQL file from the cookbook to the Chef cache.
cookbook_file sql_script_path do
  source 'sql_relativity_instance_setting_update_developer_mode.sql'
end

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

# Run the SQL file only if CookieSecure Instance Setting is not already 'False'
custom_log 'custom_log' do msg 'Setting DeveloperMode InstanceSetting to True' end
powershell_script 'relativity_developer_mode_setup' do
  code <<-EOH
    Import-Module "#{sqlps_module_path}"
    Invoke-Sqlcmd -InputFile #{sql_script_path}
  EOH
end
custom_log 'custom_log' do msg 'Updated DeveloperMode InstanceSetting.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished DeveloperMode Instance Setting Update' end
