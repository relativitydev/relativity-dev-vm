[Boolean] $global:exportVm = $false
[string] $global:vmName = "RelativityDevVm"
[string] $global:vmExportPath = "C:\DevVmExport"
[string] $global:vmCheckpointName = "RelativityDevVm Created"
[string] $global:devVmCreationResultFileName = "result_file.txt"
[Boolean] $global:devVmCreationWasSuccess = $false

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
    $env:DevVmCreationErrorStatus = "true"
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
    $env:DevVmCreationErrorStatus = "true"
    Write-Error-Message-To-Screen "An error occured when deleting a folder."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Create-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Creating VM"

    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot

    vagrant up
    
    Write-Message-To-Screen  "Created VM"
  }
  Catch [Exception] {
    $env:DevVmCreationErrorStatus = "true"
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
    $env:DevVmCreationErrorStatus = "true"
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
    $env:DevVmCreationErrorStatus = "true"
    Write-Error-Message-To-Screen "An error occured when creating VM checkpoint."
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
    $env:DevVmCreationErrorStatus = "true"
    Write-Error-Message-To-Screen "An error occured when exporting VM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Compress-DevVm() {
  try {
    Write-Heading-Message-To-Screen  "Compressing Exported VM to 7Zip"
    
    [string] $folderToCompressPath = "$($global:vmExportPath)\$($global:vmName)"
    [string] $zipFilePath = "$($global:vmExportPath)\$($global:vmName).7z"

    # Remove previous zip file
    Delete-File-If-It-Exists $zipFilePath

    Install-Module -NugetPackageId 7Zip4Powershell -PackageVersion 1.8.0
    Compress-7Zip -CompressionLevel Fast -Path $folderToCompressPath -ArchiveFileName $zipFilePath
    #todo: remove fast compression

    Write-Message-To-Screen  "Compressed Exported VM to 7Zip"
  }
  Catch [Exception] {
    $env:DevVmCreationErrorStatus = "true"
    Write-Error-Message-To-Screen "An error occured when compressing Exported VM to 7Zip."
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
    $env:DevVmCreationErrorStatus = "true"
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
    $env:DevVmCreationErrorStatus = "true"
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
    $env:DevVmCreationErrorStatus = "true"
    Write-Error-Message-To-Screen "An error occured when checking DevVM Result file for Success."
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
    $env:DevVmCreationErrorStatus = "true"
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
    $env:DevVmCreationErrorStatus = "true"
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
        Create-DevVm-Checkpoint
        Export-DevVm
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
    $env:DevVmCreationErrorStatus = "true"
    Write-Error-Message-To-Screen "An error occured when creating DevVM."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
  finally {
    if ($global:exportVm) {
      Start-DevVm
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
    $env:DevVmCreationErrorStatus = "false"

    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot

    # Create New DevVm
    New-DevVm
  }
  Catch [Exception] {
    $env:DevVmCreationErrorStatus = "true"
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
