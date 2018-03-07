# Variables
[string] $virtualSwitchName = "RelativityDevVmSwitch"

function Write-Message-To-Screen ([string] $writeMessage) {
  Write-Host  "-----> [$(Get-Date -Format g)] $($writeMessage)" -ForegroundColor Blue
}

function Install-Chef-Development-Kit() {
  Write-Message-To-Screen "Installing Chef Development Kit Version 2.4.17"

  . { iwr -useb https://omnitruck.chef.io/install.ps1 } | iex; install -project chefdk -channel stable -version 2.4.17

  Write-Message-To-Screen "Installed Chef Development Kit Version 2.4.17 \n"
}

function Enable-HyperV-Windows-Feature() {
  Write-Message-To-Screen "Enabling the Hyper-V Windows feature"

  Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All
  
  Write-Message-To-Screen "Enabled the Hyper-V Windows feature \n"
}

function Load-HyperV-PowerShell-Module() {
  Write-Message-To-Screen "Loading Hyper-V PowerShell module"

  Import-Module ServerManager
  Add-WindowsFeature RSAT-Hyper-V-Tools -IncludeAllSubFeature

  Write-Message-To-Screen "Loaded Hyper-V PowerShell module \n"
}

function Setup-HyperV-Virtual-Switch() {
  Write-Message-To-Screen "Setting up Hyper-V Virtual Switch"

  [Boolean] $switchFound = $false
  
  # Query for virutal switches
  $virtualSwitches = Get-VMSwitch -SwitchType Internal

  # Loop through queried virtual switches
  $virtualSwitches | ForEach-Object { 
    if ($_.Name -eq $virtualSwitchName) {
      Write-Host "'$($virtualSwitchName)' virtual switch found!"
      $switchFound = $true
    }
    else {
      Write-Host "'$($virtualSwitchName)' virtual switch NOT found!"
      $switchFound = $false
    }
  }

  # Create virtual switch if not found
  if (-not $switchFound) {
    # Ask for Ethernet Adapter name
    $ethernetAdapterName = Read-Host -Prompt 'Please enter the name of the Ethernet Adapter which has Internet connection'
    
    # Create virtual switch
    New-VMSwitch -Name $virtualSwitchName -NetAdapterName $ethernetAdapterName -AllowManagementOS $True -Notes "Provide public network access to DevVM"
    Write-Host "'$($virtualSwitchName)' virtual switch created!"
  }

  Write-Message-To-Screen "Setup complete for Hyper-V Virtual Switch \n"
}

function Install-Chocolatey() {
  Write-Message-To-Screen "Installing Chocolatey"

  Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
  
  Write-Message-To-Screen "Installed Chocolatey \n"
}

function Install-Vagrant() {
  Write-Message-To-Screen "Installing Vagrant version 2.0.1"

  choco install vagrant --version 2.0.1
  
  Write-Message-To-Screen "Installed Vagrant version 2.0.1 \n"
}

function Install-Vagrant-Reboot-Plugin() {
  Write-Message-To-Screen "Installing Vagrant Reboot plugin"

  vagrant plugin install vagrant-reload
  
  Write-Message-To-Screen "Installed Vagrant Reboot plugin \n"
}

function Install-Vagrant-Berkshelf-Plugin() {
  Write-Message-To-Screen "Installing Vagrant Berkshelf plugin"

  vagrant plugin install vagrant-berkshelf 
  
  Write-Message-To-Screen "Installed Vagrant Berkshelf plugin \n"
}

function Add-DevVm-Base-Machine-As-Vagrant-Box-Image() {
  Write-Message-To-Screen "Adding DevVM base machine as Vagrant box image"

  # Ask for Ethernet Adapter name
  $boxFilePath = Read-Host -Prompt 'Please enter the path for DevVM base image box file'

  # Add Vagrant box image
  vagrant box add "DevVmBaseImage" $boxFilePath
  
  Write-Message-To-Screen "Adding DevVM base machine as Vagrant box image"
}

function Start-Machine-Setup() {
  $stopWatch = [System.Diagnostics.Stopwatch]::StartNew() 

  try {
    # Install Chef Development Kit Version 2.4.17
    Install-Chef-Development-Kit

    # Enable the Hyper-V Windows feature
    Enable-HyperV-Windows-Feature

    # Load Hyper-V PowerShell module
    Load-HyperV-PowerShell-Module
    
    # Setup Hyper-V Virtual Switch
    Setup-HyperV-Virtual-Switch

    # Install Chocolatey
    Install-Chocolatey

    # Install Vagrant
    Install-Vagrant

    # Install Vagrant Reboot plugin
    Install-Vagrant-Reboot-Plugin

    # Install Vagrant berkshelf plugin
    Install-Vagrant-Berkshelf-Plugin

    # Add DevVm base machine as Vagrant box image
    Add-DevVm-Base-Machine-As-Vagrant-Box-Image
  }
  Catch [Exception] {
    Write-Host "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Host "-----> Exception Message: $($_.Exception.Message)"
  }
  finally {
    Write-Host  "-----> [$(Get-Date -Format g)] Total time: $($stopWatch.Elapsed.TotalMinutes) minutes" -ForegroundColor Blue
    $stopWatch.Stop() 
  }
}

Start-Machine-Setup
