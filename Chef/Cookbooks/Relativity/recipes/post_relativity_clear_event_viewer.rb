custom_log 'custom_log' do msg 'Starting Clearing Event Viewer' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    # Clear Event Viewer
    custom_log 'custom_log' do msg 'Clearing Event Viewer' end
    
    powershell_script 'clear_event_viewer' do
      code <<-EOH
        Clear-Eventlog -Log Application
        EOH
    end
    
    custom_log 'custom_log' do msg 'Cleared Event Viewer' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Clearing Event Viewer\n\n\n" end