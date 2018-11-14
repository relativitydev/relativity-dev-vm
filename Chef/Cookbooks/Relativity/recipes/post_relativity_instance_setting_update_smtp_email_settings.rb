custom_log 'custom_log' do msg 'Starting SMTPServer/Email Instance Setting Update' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

#########################################################################################################
#NOTE: When you add any new STMP settings added to this recipe, don't forget to add to the clean up SMTP recipe.
#########################################################################################################

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

# Update SMTPServer
custom_log 'custom_log' do msg 'Updating SMTPServer Instance Setting.' end
powershell_script 'update_instance_setting - SMTPServer' do
  code <<-EOH
  Import-Module "#{sqlps_module_path}"
  Invoke-Sqlcmd -Query "
      UPDATE
          [EDDS].[eddsdbo].[InstanceSetting]
      SET
          [Value] = '#{node['smtp_server']}'
      WHERE
          [Section] = 'kCura.Notification' AND
          [Name] = 'SMTPServer'
  "
  EOH
end
custom_log 'custom_log' do msg 'Updated SMTPServer Instance Setting.' end

# Update SMTPPort
custom_log 'custom_log' do msg 'Updating SMTPPort Instance Setting.' end
powershell_script 'update_instance_setting - SMTPPort' do
  code <<-EOH
  Import-Module "#{sqlps_module_path}"
  Invoke-Sqlcmd -Query "
      UPDATE
          [EDDS].[eddsdbo].[InstanceSetting]
      SET
          [Value] = '#{node['smtp_port']}'
      WHERE
          [Section] = 'kCura.Notification' AND
          [Name] = 'SMTPPort'
  "
  EOH
end
custom_log 'custom_log' do msg 'Updated SMTPPort Instance Setting.' end

# Update EmailFrom
custom_log 'custom_log' do msg 'Updating EmailFrom Instance Setting.' end
powershell_script 'update_instance_setting - EmailFrom' do
  code <<-EOH
  Import-Module "#{sqlps_module_path}"
  Invoke-Sqlcmd -Query "
      UPDATE
          [EDDS].[eddsdbo].[InstanceSetting]
      SET
          [Value] = '#{node['email_from']}'
      WHERE
          [Section] = 'kCura.Notification' AND
          [Name] = 'EmailFrom'
  "
  EOH
end
custom_log 'custom_log' do msg 'Updated EmailFrom Instance Setting.' end

# Update EmailTo
custom_log 'custom_log' do msg 'Updating EmailTo Instance Setting.' end
powershell_script 'update_instance_setting - EmailTo' do
  code <<-EOH
  Import-Module "#{sqlps_module_path}"
  Invoke-Sqlcmd -Query "
      UPDATE
          [EDDS].[eddsdbo].[InstanceSetting]
      SET
          [Value] = '#{node['email_to']}'
      WHERE
          [Section] = 'kCura.Notification' AND
          [Name] = 'EmailTo'
  "
  EOH
end
custom_log 'custom_log' do msg 'Updated EmailTo Instance Setting.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished SMTPServer/Email Instance Setting Update\n\n\n" end
