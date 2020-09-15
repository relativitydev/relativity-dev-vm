Steps to setup up Workstation
* Run this steps manually one after another

# Install Chocolatey
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

# Install Git
choco install git --version 2.14.1

# Clone DevVm Repository
cd "C:\"
git clone https://github.com/relativitydev/relativity-dev-vm.git

# Install Chef Development Kit Version 2.4.17
. { iwr -useb https://omnitruck.chef.io/install.ps1 } | iex; install -project chefdk -channel stable -version 2.4.17

# Enable the Hyper-V Windows feature
Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All -NoRestart

#------------> Restart Machine

# Load Hyper-V PowerShell module
Import-Module ServerManager
Add-WindowsFeature RSAT-Hyper-V-Tools -IncludeAllSubFeature
    
# Setup Hyper-V Virtual Switch
[string] $virtualSwitchName = "RelativityDevVmSwitch"
[Boolean] $switchFound = $false
[string] $ethernetAdapterName = "Ethernet"
  
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
  # Create virtual switch
  New-VMSwitch -Name $virtualSwitchName -NetAdapterName $ethernetAdapterName -AllowManagementOS $True -Notes "Provide public network access to DevVM"
  Write-Host "'$($virtualSwitchName)' virtual switch created!"
}

# Install Vagrant
choco install vagrant --version 2.0.1 --yes

#------------> Close and open powershell

# Install Vagrant Reboot plugin
vagrant plugin install vagrant-reload

# Install Vagrant berkshelf plugin
vagrant plugin install vagrant-berkshelf 

# Add DevVm base machine as Vagrant box image
Add-DevVm-Base-Machine-As-Vagrant-Box-Image# Add Vagrant box image
vagrant box add "DevVmBaseImage" $Env:devvm_box_image_network_path
  