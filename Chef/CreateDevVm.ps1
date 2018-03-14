[Boolean] $global:exportVm = $false
[string] $global:vmName = "RelativityDevVm"
[string] $global:vmExportPath = "C:\DevVmExport"
[string] $global:vmCheckpointName = "RelativityDevVm Created"
[string] $global:devVmCreationResultFileName = "result_file.txt"
[Boolean] $global:devVmCreationWasSuccess = $false

function Write-Message-To-Screen ([string] $writeMessage) {
  Write-Host  "-----> [$(Get-Date -Format g)] $($writeMessage)" -ForegroundColor Magenta
}

function Remove-File-If-It-Exists ([string] $filePath) {
  If (Test-Path $filePath) {
    Remove-Item -path $filePath -Force
    Write-Message-To-Screen  "File[$($filePath)] exists and deleted."
  }
  else {
    Write-Message-To-Screen  "File[$($filePath)] doesn't exist. Skipped Deletion."
  }
}

function Remove-Directory-If-It-Exists ([string] $directoryPath) {
  If (Test-Path $directoryPath) {
    Remove-Item -path $directoryPath -Force -Recurse
    Write-Message-To-Screen  "Directory[$($directoryPath)] exists and deleted."
  }
  else {
    Write-Message-To-Screen  "Directory[$($directoryPath)] doesn't exist. Skipped Deletion."
  }
}

function Create-DevVm() {
  try {
    Write-Message-To-Screen  "Creating VM"
    vagrant up
    Write-Message-To-Screen  "Created VM"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when creating VM."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Stop-DevVm() {
  try {
    Write-Message-To-Screen  "Stopping VM"
    Stop-VM -Name $global:vmName
    Write-Message-To-Screen  "Stopped VM"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when stopping VM."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Create-DevVm-Checkpoint() {
  try {
    Write-Message-To-Screen  "Creating VM Checkpoint"
    Checkpoint-VM -Name $global:vmName -SnapshotName $global:vmCheckpointName
    Write-Message-To-Screen  "Created VM Checkpoint"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when creating VM checkpoint."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Export-DevVm() {
  try {
    Write-Message-To-Screen  "Exporting VM"
    
    # Delete Export folder if it already exists
    Remove-Directory-If-It-Exists $global:vmExportPath  

    # Export VM
    Export-VM -Name $global:vmName -Path $global:vmExportPath

    Write-Message-To-Screen  "Exported VM"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when exporting VM."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Compress-DevVm() {
  try {
    Write-Message-To-Screen  "Compressing Exported VM to 7Zip"
    
    [string] $folderToCompressPath = "$($global:vmExportPath)\$($global:vmName)"
    [string] $zipFilePath = "$($global:vmExportPath)\$($global:vmName).7z"

    Install-Module -NugetPackageId 7Zip4Powershell -PackageVersion 1.8.0
    Compress-7Zip -CompressionLevel Fast -Path $folderToCompressPath -ArchiveFileName $zipFilePath
    #todo: remove fast compression

    Write-Message-To-Screen  "Compressed Exported VM to 7Zip"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when compressing Exported VM to 7Zip."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Start-DevVm() {
  try {
    Write-Message-To-Screen  "Starting VM"
    Start-VM -Name $global:vmName
    Write-Message-To-Screen  "Started VM"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when starting VM."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Delete-DevVm() {
  try {
    Write-Message-To-Screen  "Deleting VM"
    vagrant destroy $global:vmName -f
    Write-Message-To-Screen  "Deleted VM"
  }
  Catch [Exception] {
    Write-Message-To-Screen "An error occured when deleting VM."
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

function Check-DevVm-Result-Text-File-For-Success() {
  Write-Message-To-Screen "Checking if DevVM creation was success."
  
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

function Delete-DevVm-Creation-Result-File() {
  Write-Message-To-Screen "Deleting DevVM Creation Result File."
  
  [string] $result_file_path = "$PSScriptroot\$($global:devVmCreationResultFileName)"
  Remove-File-If-It-Exists $result_file_path
  
  Write-Message-To-Screen "Deleted DevVM Creation Result File. [$($result_file_path)]"
  Write-Empty-Line-To-Screen
}

function New-DevVm() {
  try {
    Write-Message-To-Screen  "Attempt #$($global:count)"    
    
    # Delete Results file if it already exists
    Delete-DevVm-Creation-Result-File

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
  finally {
    if ($global:exportVm) {
      Start-DevVm
      Delete-DevVm
    }
    else {
      Write-Message-To-Screen "Skipped VM Deletion!"
    }    
  }
}

function Start-DevVm-Process() {
  $stopWatch = [System.Diagnostics.Stopwatch]::StartNew() 

  # while ($global:count -le $global:maxRetry) {
  try {    
    # Create New DevVm
    New-DevVm
  }
  Catch [Exception] {
    $global:count++
    Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
  }
  finally {
    Write-Message-To-Screen  "Total time: $($stopWatch.Elapsed.TotalMinutes) minutes"
    $stopWatch.Stop() 
  }
  # }
}

Start-DevVm-Process
