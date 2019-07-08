[Boolean] $global:exportVm = $true
[string] $global:vmName = "RelativityDevVm"
[string] $global:vmExportPath = "C:\DevVmExport"
[string] $global:vmDocumentationOnline = "Relativity Dev VM documentation can be found at this link - https://github.com/relativitydev/relativity-dev-vm/blob/master/Documentation/PDF/Relativity%20Dev%20VM%20-%20Pre-built%20VM%20-%20Documentation.pdf"
[string] $global:vmCheckpointName = "$($global:vmName) Created"
[string] $global:devVmCreationResultFileName = "result_file.txt"
[Boolean] $global:devVmCreationWasSuccess = $false
[string] $global:compressedFileExtension = "zip"

[string] $devVmAutomationConfigFilePath = "C:\DevVm_Automation_Config.json"
[string] $json = Get-Content -Path $devVmAutomationConfigFilePath
$jsonContents = $json | ConvertFrom-Json
[string] $global:devVmInstallFolder = $jsonContents.devVmInstallFolder
[string] $global:relativityInvariantVersionNumberFileName = "relativity_invariant_version.txt"
$global:vmProcessorsOnExport = 2
$global:vmMemoryOnExport = 8000MB

function Write-Empty-Line-To-Screen () {
  Write-Host ""
}

function Write-Host-Custom ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Magenta
}

function Write-Host-Custom-Red ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Red
}

function Write-Host-Custom-Green ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Green
}

function Write-Message-To-Screen ([string] $writeMessage) {
  [string] $formattedMessage = "-----> [$(Get-Date -Format g)] $($writeMessage)"
  Write-Host-Custom $formattedMessage
}

function Write-Heading-Message-To-Screen ([string] $writeMessage) {
  [string] $formattedMessage = "-----> [$(Get-Date -Format g)] $($writeMessage)"
  Write-Host-Custom-Green $formattedMessage
}

function Write-Error-Message-To-Screen ([string] $writeMessage) {
  [string] $formattedMessage = "-----> [$(Get-Date -Format g)] $($writeMessage)"
  Write-Host-Custom-Red $formattedMessage
}

function Delete-File-If-It-Exists ([string] $filePath) {
  try {
    If (Test-Path $filePath) {
      Remove-Item -path $filePath -Force
      Write-Message-To-Screen  "File[$($filePath)] exists and deleted."
    }
    else {
      Write-Message-To-Screen  "File[$($filePath)] doesn't exist. Skipped Deletion."
    }
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when deleting a file."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Delete-Folder-If-It-Exists ([string] $directoryPath) {
  try {
    If (Test-Path $directoryPath) {
      Remove-Item -path $directoryPath -Force -Recurse
      Write-Message-To-Screen  "Directory[$($directoryPath)] exists and deleted."
    }
    else {
      Write-Message-To-Screen  "Directory[$($directoryPath)] doesn't exist. Skipped Deletion."
    }
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when deleting a folder."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Create-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Creating VM"

    # Make sure we are in the folder where the running script exists
    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot

    vagrant up
    
    Write-Message-To-Screen  "Created VM"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when creating VM. (Vagrant Up)"
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Stop-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Stopping VM"
    Stop-VM -Name $global:vmName
    Write-Message-To-Screen  "Stopped VM"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when stopping VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Create-DevVm-Checkpoint() {
  try {
    Write-Heading-Message-To-Screen  "Creating VM Checkpoint"
    Checkpoint-VM -Name $global:vmName -SnapshotName $global:vmCheckpointName
    Write-Message-To-Screen  "Created VM Checkpoint"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when creating VM checkpoint."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Rename-DevVm() {
  try{
    Write-Heading-Message-To-Screen  "Renaming VM"
    $relativityAndInvariantVersions = Get-Content -Path "$($global:devVmInstallFolder)\Relativity\$($global:relativityInvariantVersionNumberFileName)"
    $indexOfComma = $relativityAndInvariantVersions.IndexOf(',')
    $indexOfColon = $relativityAndInvariantVersions.IndexOf(':')
    $relativityVersion = $relativityAndInvariantVersions.Substring(($indexOfColon + 2), ($indexOfComma - ($indexOfColon +2)))
    $relativityVersion = $relativityVersion.Replace(" ", "")
    [string] $newName = "$($global:vmName)-$($relativityVersion)"
    Rename-Vm -Name $global:vmName -NewName $newName
    $global:vmName = $newName
    Write-Message-To-Screen  "Renamed VM"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when renaming the VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Create-DevVm-Documentation-Text-File() {
  try {
    Write-Heading-Message-To-Screen  "Creating DevVM Documentation text file"

    [string] $documentation_text_file_path = "$($global:vmExportPath)\$($global:vmName)\DevVm_Documentation.txt"

    # Delete DevVM Documentation text file if it already exists
    Delete-File-If-It-Exists $documentation_text_file_path

    # Create DevVM Documentation text file
    New-Item $documentation_text_file_path -type file -Force
    Set-Content -Path $documentation_text_file_path -Value $global:vmDocumentationOnline -Force

    Write-Message-To-Screen  "Created DevVM Documentation text file"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when creating DevVM Documentation text file"
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Downgrade-DevVm-Resources() {
  try{
    Set-VMProcessor $global:vmName -Count $global:vmProcessorsOnExport
    Set-VMMemory $global:vmName -StartupBytes $global:vmMemoryOnExport
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when dowgrading the VM resources."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Export-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Exporting VM"
    
    # Delete Export folder if it already exists
    Delete-Folder-If-It-Exists $global:vmExportPath  

    # Export VM
    Export-VM -Name $global:vmName -Path $global:vmExportPath

    Write-Message-To-Screen  "Exported VM"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when exporting VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Compress-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Converting Exported VM to a Zip file"
    
    [string] $folderToCompressPath = "$($global:vmExportPath)\$($global:vmName)"
    [string] $zipFilePath = "$($global:vmExportPath)\$($global:vmName).$($global:compressedFileExtension)"

    # Make sure we are in the folder where the running script exists
    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot

    # Remove previous zip file
    Delete-File-If-It-Exists $zipFilePath

    # Create new zip file
    Import-Module .\Cookbooks\Relativity\files\default\DevVmPsModules.dll
    Add-ZipFolder -SourceFolderPath $folderToCompressPath -DestinationZipFilePath $zipFilePath

    Write-Message-To-Screen  "Converted Exported VM to a Zip file"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when converting Exported VM to a Zip file"
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Start-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Starting VM"
    Start-VM -Name $global:vmName
    Write-Message-To-Screen  "Started VM"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when starting VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Delete-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Deleting VM"
    vagrant destroy $global:vmName -f
    Write-Message-To-Screen  "Deleted VM"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when deleting VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Check-DevVm-Result-Text-File-For-Success() {
  try {
    Write-Heading-Message-To-Screen "Checking if DevVM creation was success."
  
    $global:devVmCreationWasSuccess = $false  
    [string] $result_file_path = "$PSScriptroot\$($global:devVmCreationResultFileName)"
    if (Test-Path $result_file_path) {
      # Read text file
      [string] $content = [IO.File]::ReadAllText($result_file_path)
      [string] $contentTrimmed = ($content.Trim())
      Write-Message-To-Screen "DevVM result file content: $($contentTrimmed)"

      if (($content.Trim()) -eq "success") {
        Write-Message-To-Screen "DevVM creation was success."
        $global:devVmCreationWasSuccess = $true    
      }
    }
    else {
      $global:devVmCreationWasSuccess = $false  
      Write-Error-Message-To-Screen "DevVM Result file doesn't exist."
    }
  
    Write-Message-To-Screen "Checked if DevVM creation was success."
    Write-Empty-Line-To-Screen
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when checking DevVM Result file for Success."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Delete-DevVm-Export-Folder() {
  try {
    Write-Heading-Message-To-Screen "Deleting DevVM Export folder."
  
    Delete-Folder-If-It-Exists $global:vmExportPath
  
    Write-Message-To-Screen "Deleted DevVM Export folder. [$($global:vmExportPath)]"
    Write-Empty-Line-To-Screen
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when deleting DevVM Export folder."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Delete-DevVm-Creation-Result-File() {
  try {
    Write-Heading-Message-To-Screen "Deleting DevVM Creation Result File."
  
    [string] $result_file_path = "$PSScriptroot\$($global:devVmCreationResultFileName)"
    Delete-File-If-It-Exists $result_file_path
  
    Write-Message-To-Screen "Deleted DevVM Creation Result File. [$($result_file_path)]"
    Write-Empty-Line-To-Screen
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when deleting DevVM Result file."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Delete-DevVm-If-It-Exists() {
  Write-Heading-Message-To-Screen "Checking for any previous DevVM instances."

  try {
    $devVmExists = Get-Vm -Name $global:vmName -ErrorAction SilentlyContinue  
    if ($devVmExists) {  
      Write-Message-To-Screen "Previous DevVM exists. Deleting it."
      Start-DevVm
      Delete-DevVm 
      Write-Message-To-Screen "Previous DevVM deleted."
    }  
    else {  
      Write-Message-To-Screen "Previous DevVM doesn't exist." 
    } 
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when deleting previous VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }

  Write-Message-To-Screen "Checked for any previous DevVM instances"
}

function New-DevVm() {
  Write-Heading-Message-To-Screen "Creating new DevVM."

  try {
    # Delete Export folder if it already exists
    Delete-DevVm-Export-Folder  
		
    # Delete Results file if it already exists
    Delete-DevVm-Creation-Result-File

    # Delete VM if it already exists
    Delete-DevVm-If-It-Exists

    # Create DevVM
    Create-DevVm

    # Check for DevVM creation result
    Check-DevVm-Result-Text-File-For-Success

    # If DevVM creation was success, export it
    if ($global:devVmCreationWasSuccess -eq $true) {    
      if ($global:exportVm) {
        Stop-DevVm
        Downgrade-DevVm-Resources
        Create-DevVm-Checkpoint
        Rename-DevVm
        Export-DevVm
        Create-DevVm-Documentation-Text-File
        Compress-DevVm
      }
      else {
        Write-Message-To-Screen "Skipped VM Export!"
      }    
    }
    else {
      Write-Error-Message-To-Screen "DevVM creation FAILED!"
    }
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when creating DevVM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
  finally {
    if ($global:exportVm -And $global:devVmCreationWasSuccess) {
      Delete-DevVm
    }
    else {
      Write-Message-To-Screen "Skipped VM Deletion!"
    }  
    Write-Message-To-Screen "Finished running new DevVM creation process."
  }
}

function Start-DevVm-Process() {
  $stopWatch = [System.Diagnostics.Stopwatch]::StartNew() 
  try {
    # Make sure we are in the folder where the running script exists
    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot

    # Create New DevVm
    New-DevVm
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured in Start-DevVm-Process function."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
  finally {
    Write-Message-To-Screen  "Total time: $($stopWatch.Elapsed.TotalMinutes) minutes"
    $stopWatch.Stop() 
  }
}

Start-DevVm-Process
