log 'Starting CookieSecure Instance Setting Update'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create a path to the SQL file in the Chef cache.
sql_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'sql_relativity_instance_setting_update_cookie_secure.sql'))

# Copy the SQL file from the cookbook to the Chef cache.
cookbook_file sql_script_path do
  source 'sql_relativity_instance_setting_update_cookie_secure.sql'
end

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

# Run the SQL file only if CookieSecure Instance Setting is nots already 'False'
log 'Setting CookieSecure Instance Setting to False.'
powershell_script 'relativity_http_setup' do
  code <<-EOH
    Import-Module "#{sqlps_module_path}"
    Invoke-Sqlcmd -InputFile #{sql_script_path}
  EOH
end
log 'Updated CookieSecure InstanceSetting.'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished CookieSecure Instance Setting Update'
