custom_log 'custom_log' do msg 'Starting Shutting off Windows Firewall' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    # Shut off Windows Firewall
    custom_log 'custom_log' do msg 'Shutting off Windows Firewall' end
    
    powershell_script 'shut_off_windows_firewall' do
      code <<-EOH
        Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False
        EOH
    end
    
    custom_log 'custom_log' do msg 'Shut off Windows Firewall' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Finished Shutting off Windows Firewall\n\n\n" end