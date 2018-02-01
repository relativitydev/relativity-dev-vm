log 'Starting disabling Windows Firewall'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Disable Windows Firewall for all profiles
powershell_script 'disable_firewall_profiles' do
  code 'Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False'
  only_if { powershell_out!('(Get-NetFirewallProfile -Profile Domain,Public,Private).Enabled').stdout.include?('True') }
end

log_message "log_message" do  message "Finished disabling firewall" end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished disabling Windows Firewall'
