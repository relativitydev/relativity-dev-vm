custom_log 'custom_log' do msg 'Starting IsOnlineInstance/Telemetry Instance Setting Update' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

# Update IsOnlineInstance
custom_log 'custom_log' do msg 'Updating IsOnlineInstance Instance Setting.' end
powershell_script 'update_instance_setting - IsOnlineInstance' do
  code <<-EOH
  Import-Module "#{sqlps_module_path}"
  Invoke-Sqlcmd -Query "
      UPDATE
          [EDDS].[eddsdbo].[InstanceSetting]
      SET
          [Value] = 'False'
      WHERE
          [Section] = 'kCura.LicenseManager' AND
          [Name] = 'IsOnlineInstance'
  "
  EOH
end
custom_log 'custom_log' do msg 'Updated IsOnlineInstance Instance Setting.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished IsOnlineInstance/Telemetry Instance Setting Update\n\n\n" end
