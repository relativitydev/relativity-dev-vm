custom_log 'custom_log' do msg 'Starting IIS Reset' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

powershell_script 'IIS_Reset' do
  code <<-EOH
    IISReset
  EOH
end

custom_log 'custom_log' do msg 'Sleeping for 5 minutes' end
sleep(300)

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished IIS Reset\n\n\n" end
