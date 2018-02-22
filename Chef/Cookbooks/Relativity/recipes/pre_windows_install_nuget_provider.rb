custom_log 'custom_log' do msg 'Starting Nuget provider install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Install Nuget provider
powershell_script 'install_nuget_provider' do
  code 'Install-PackageProvider -Name NuGet -Force'
  not_if '(Get-PackageProvider).Name -Contains "NuGet"'
end

custom_log 'custom_log' do msg 'Finished setting up nuget provider' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished Nuget provider install' end
