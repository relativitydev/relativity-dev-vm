custom_log 'custom_log' do msg 'Starting Updating Java Environment Variables' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update Java Environment Variables
custom_log 'custom_log' do msg 'Updating Java Environment Variables' end

powershell_script 'update_java_environment_variables' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Reset-JavaEnvironmentVariables  -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Updated Java Environment Variables' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Updating Java Environment Variables\n\n\n" end