log 'Starting Nuget provider install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Nuget provider
powershell_script 'install_nuget_provider' do
  code 'Install-PackageProvider -Name NuGet -Force'
  not_if '(Get-PackageProvider).Name -Contains "NuGet"'
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Nuget provider install'
