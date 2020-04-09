custom_log 'custom_log' do msg 'Starting Toggle to Enable Aero UI' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Toggle Aero UI
custom_log 'custom_log' do msg 'Toggling to Enable Aero UI.' end
powershell_script 'toggle_aero_ui' do
  code <<-EOH
    Invoke-Sqlcmd -Query "
        USE EDDS;
        INSERT INTO 
            [EDDS].[eddsdbo].[Toggle] 
        VALUES 
            ('kCura.EDDS.Web.UseRemoteNavHeaderToggle',1)
    "
    EOH
end
custom_log 'custom_log' do msg 'Toggled to Enable Aero UI.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Toggle to Enable Aero UI\n\n\n" end