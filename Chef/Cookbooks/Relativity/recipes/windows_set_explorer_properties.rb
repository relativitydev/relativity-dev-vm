custom_log 'custom_log' do msg 'Starting setting Explorer properties' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

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

custom_log 'custom_log' do msg 'Finished setting up windows explorer properties' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished setting Explorer properties\n\n\n" end
