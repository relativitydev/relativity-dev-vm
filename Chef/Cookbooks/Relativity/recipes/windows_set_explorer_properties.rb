log 'Starting setting Explorer properties'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

powershell_script 'set windows explorer properties' do
  code <<-EOH
    $key = "HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced"

    Write-Host "1. Enabling showing hidden files"
    Set-ItemProperty $key Hidden 1

    Write-Host "2. Disabling hiding extensions for known files"
    Set-ItemProperty $key HideFileExt 0

    Write-Host "3. Disabling showing hidden operation system files"
    Set-ItemProperty $key ShowSuperHidden 0

    # Write-Host "Restarting explorer shell to apply registry changes"
    # Stop-Process -processname explorer -Force

    Write-Host "Enabling never group taskbar items option"
    Set-ItemProperty $key TaskbarGlomLevel 2

    # Write-Host "Restarting explorer shell to apply registry changes"
    # Stop-Process -processname explorer -Force
  EOH
  timeout node['timeout']['default']
end

log_message "log_message" do  message "Finished setting up windows explorer properties" end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished setting Explorer properties'
