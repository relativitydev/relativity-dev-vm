custom_log 'custom_log' do msg 'Starting RelativityInstanceUrl Instance Setting Update' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update RelativityInstanceUrl
custom_log 'custom_log' do msg 'Updating RelativityInstanceUrl Instance Setting.' end

instance_name = "#{node['windows']['new_computer_name']}"
instance_name_upper_case = instance_name.upcase

powershell_script 'update_instance_setting - RelativityInstanceUrl' do
  code <<-EOH
  #{node['powershell_module']['import_module']}
  Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['eddsdbo']['password']} -Name RelativityInstanceURL -Section Relativity.Core -NewValue "http://#{instance_name_upper_case}/Relativity"
  EOH
end

custom_log 'custom_log' do msg 'Updated RelativityInstanceUrl Instance Setting.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished RelativityInstanceUrl Instance Setting Update\n\n\n" end
