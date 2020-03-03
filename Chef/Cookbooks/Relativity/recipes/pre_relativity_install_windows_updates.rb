custom_log 'custom_log' do msg 'Starting Windows Updates' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    powershell_script 'Install_Bootstrapper_And_Update_Windows' do
      code <<-EOH
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        Invoke-WebRequest -UseBasicParsing -Uri https://boxstarter.org/bootstrapper.ps1 | Invoke-Expression; Get-Boxstarter -Force
        Import-Module Boxstarter.WinConfig
        Enable-MicrosoftUpdate
        Restart-Service -Name "Windows Update"
        Install-WindowsUpdate -AcceptEula -SuppressReboots
        Write-Output "Updates complete."
        EOH
    end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Finished Windows Updates\n\n\n" end
    