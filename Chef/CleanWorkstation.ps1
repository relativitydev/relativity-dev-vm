Steps to clean up Workstation
* Run this steps manually one after another

# Remove Vagrant Box Image
vagrant box remove "DevVmBaseImage" --force

# Uninstall vagrant-berkshelf plugin
vagrant plugin uninstall vagrant-berkshelf 

# Uninstall vagrant-reload plugin
vagrant plugin uninstall vagrant-reload

# Uninstall Vagrant
choco uninstall vagrant --force

# Uninstall Chocolatey
Remove-Item -Recurse -Force "$env:ChocolateyInstall" 
[System.Text.RegularExpressions.Regex]::Replace([Microsoft.Win32.Registry]::CurrentUser.OpenSubKey('Environment').GetValue('PATH', '', [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames).ToString(), [System.Text.RegularExpressions.Regex]::Escape("$env:ChocolateyInstall\bin") + '(?>;)?', '', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase) | % {[System.Environment]::SetEnvironmentVariable('PATH', $_, 'User')}
[System.Text.RegularExpressions.Regex]::Replace([Microsoft.Win32.Registry]::LocalMachine.OpenSubKey('SYSTEM\CurrentControlSet\Control\Session Manager\Environment\').GetValue('PATH', '', [Microsoft.Win32.RegistryValueOptions]::DoNotExpandEnvironmentNames).ToString(), [System.Text.RegularExpressions.Regex]::Escape("$env:ChocolateyInstall\bin") + '(?>;)?', '', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase) | % {[System.Environment]::SetEnvironmentVariable('PATH', $_, 'Machine')}

if ($env:ChocolateyBinRoot -ne '' -and $env:ChocolateyBinRoot -ne $null) { Remove-Item -Recurse -Force "$env:ChocolateyBinRoot"  }
if ($env:ChocolateyToolsRoot -ne '' -and $env:ChocolateyToolsRoot -ne $null) { Remove-Item -Recurse -Force "$env:ChocolateyToolsRoot"  }
[System.Environment]::SetEnvironmentVariable("ChocolateyBinRoot", $null, 'User')
[System.Environment]::SetEnvironmentVariable("ChocolateyToolsLocation", $null, 'User')

# Remove Virtual Switch
Remove-VMSwitch "RelativityDevVmSwitch" -Force

# Uninstall Hyper-V
Disable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -NoRestart

# Uninstall Chef Developmenet Kit
#------------> Do it manually from Programs and Features
#------------> Restart Machine

