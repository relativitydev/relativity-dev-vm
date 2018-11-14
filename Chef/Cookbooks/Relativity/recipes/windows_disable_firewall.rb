custom_log 'custom_log' do msg 'Starting disabling Windows Firewall' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Disable Windows Firewall for all profiles
powershell_script 'disable_firewall_profiles' do
  code 'Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False'
  only_if { powershell_out!('(Get-NetFirewallProfile -Profile Domain,Public,Private).Enabled').stdout.include?('True') }
end

custom_log 'custom_log' do msg 'Windows Firewall disabled.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished disabling Windows Firewall\n\n\n" end
