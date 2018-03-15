Clear-Host

function Write-Host-Custom ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Magenta
}

function Write-Host-Custom-Red ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Red
}

function Write-Host-Custom-Green ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Green
}

# Retrieve values from DevVm_Automation_Config.yaml file
[string] $devVmAutomationConfigFilePath = "C:\DevVm_Automation_Config.json"
[string] $json = Get-Content -Path $devVmAutomationConfigFilePath
$jsonContents = $json | ConvertFrom-Json

[string] $global:relativityVersionFolder = $jsonContents.relativityVersionFolder
Write-Host "global:relativityVersionFolder = $($global:relativityVersionFolder)"
[string] $global:invariantVersionFolder = $jsonContents.invariantVersionFolder
Write-Host "global:invariantVersionFolder = $($global:invariantVersionFolder)"
[string] $global:devVmInstallFolder = $jsonContents.devVmInstallFolder
Write-Host "global:devVmInstallFolder = $($global:devVmInstallFolder)"
[string] $global:devVmRelativityInvariantCompatibilityTextFile = $jsonContents.devVmRelativityInvariantCompatibilityTextFile
Write-Host "global:devVmRelativityInvariantCompatibilityTextFile = $($global:devVmRelativityInvariantCompatibilityTextFile)"
[string] $global:devVmCreatedTextFile = $jsonContents.devVmCreatedTextFile
Write-Host "global:devVmCreatedTextFile = $($global:devVmCreatedTextFile)"
[string] $global:devVmAutomationLogsFolder = $jsonContents.devVmAutomationLogsFolder
Write-Host "global:devVmAutomationLogsFolder = $($global:devVmAutomationLogsFolder)"
[string] $global:devVmNetworkStorageLocation = $jsonContents.devVmNetworkStorageLocation
Write-Host "global:devVmNetworkStorageLocation = $($global:devVmNetworkStorageLocation)"

[System.Int32]$global:maxRetry = 3
[System.Int32]$global:count = 1
[string] $global:regexForRelativityVersion = "\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"
[string] $global:devVmAutomationLogFilePrefix = "DevVm_Automation_Log"
[string] $global:devVmAutomationLogFileResetText = "empty"
$global:allRelativityVersions = New-Object System.Collections.ArrayList
$global:devVmVersionsCreated = New-Object System.Collections.ArrayList
$global:devVmVersionsToCreate = New-Object System.Collections.ArrayList
[string] $global:vmName = "RelativityDevVm"
[string] $global:vmExportPath = "C:\DevVmExport"
[Boolean] $global:foundCompatibleInvariantVersion = $false
[string] $global:invariantVersion = ""
[string] $global:devVmCreationResultFileName = "result_file.txt"
[Boolean] $global:devVmCreationWasSuccess = $false

function Reset-Logs-Environment-Variable() {
  Write-Host-Custom-Green "Resetting Logs Environment variable."

  Write-Host-Custom "env:DevVmAutomationLogFilePath=$($env:DevVmAutomationLogFilePath)"
  if (-Not (Test-Path env:DevVmAutomationLogFilePath)) {
    throw "ERROR: Environment variable for Logs doesn't exist."
  }
  else {
    Write-Host-Custom "Environment variable for Logs exists. Setting it to empty string."
    $env:DevVmAutomationLogFilePath = $global:devVmAutomationLogFileResetText
  }
  Write-Host-Custom "env:DevVmAutomationLogFilePath=$($env:DevVmAutomationLogFilePath)"

  Write-Host-Custom "Done resetting Logs Environment variable."
  Write-Host-Custom ""
}

function Create-Log-File () {
  Write-Host-Custom-Green "Creating new Log file."

  # Create DevVM Logs folder if it not already exists
  if (Test-Path $global:devVmAutomationLogsFolder) {    
    Write-Host-Custom "DevVM Automation Logs folder exists."
  }
  else {
    Write-Host-Custom "DevVM Automation Logs folder doesn't exists."
    New-Item -Path $global:devVmAutomationLogsFolder -ItemType directory
    Write-Host-Custom "Created DevVM Automation Logs folder. [$($global:devVmAutomationLogsFolder)]"
  }

  # Generate new Log file name
  [string] $newLogFileName = "$($global:devVmAutomationLogFilePrefix)_$(((get-date).ToUniversalTime()).ToString("yyyyMMddThhmmssZ")).txt"
  $env:DevVmAutomationLogFilePath = "$($global:devVmAutomationLogsFolder)\$($newLogFileName)"
  Write-Host-Custom "Log file: $($env:DevVmAutomationLogFilePath)"
  
  # Create Logs file
  New-Item $env:DevVmAutomationLogFilePath -ItemType file

  Write-Host-Custom "Created new Log file."
  Write-Host-Custom ""
}

function Write-To-Log-File ([string] $writeMessage) {
  Add-Content -path $env:DevVmAutomationLogFilePath -Value "$($writeMessage)"
}

function Write-Message-To-Screen ([string] $writeMessage) {
  [string] $formattedMessage = "-----> [$(Get-Date -Format g)] $($writeMessage)"
  Write-Host-Custom $formattedMessage
  Write-To-Log-File $formattedMessage
}

function Write-Heading-Message-To-Screen ([string] $writeMessage) {
  [string] $formattedMessage = "-----> [$(Get-Date -Format g)] $($writeMessage)"
  Write-Host-Custom-Green $formattedMessage
  Write-To-Log-File $formattedMessage
}

function Write-Error-Message-To-Screen ([string] $writeMessage) {
  [string] $formattedMessage = "-----> [$(Get-Date -Format g)] $($writeMessage)"
  Write-Host-Custom-Red $formattedMessage
  Write-To-Log-File $formattedMessage
}

function Write-Empty-Line-To-Screen () {
  Write-Host ""
  Write-To-Log-File ""
}

function Delete-File-If-It-Exists ([string] $filePath) {
  If (Test-Path $filePath) {
    Remove-Item -path $filePath -Force
    Write-Message-To-Screen  "File exists and deleted. [$($filePath)]"
  }
  else {
    Write-Message-To-Screen  "File doesn't exist. Skipped Deletion. [$($filePath)]"
  }
}

function Copy-File-Overwrite-If-It-Exists ([string] $sourceFilePath, [string] $destinationFilePath) {
  Write-Message-To-Screen "Copying File. [Source: $($sourceFilePath), Destination: $($destinationFilePath)]"
  Copy-Item -Path $sourceFilePath -Destination $destinationFilePath -Force
  Write-Message-To-Screen "Copied File. [Source: $($sourceFilePath), Destination: $($destinationFilePath)]"
}

function Retrieve-All-Relativity-Versions-Released() {
  # Retrieve all child folders
  $allChildFolders = Get-ChildItem -Path $global:relativityVersionFolder

  Write-Heading-Message-To-Screen "List of Child folders:"
  
  $allChildFolders | ForEach-Object {
    $folderName = ($_.Name).Trim()
    Write-Message-To-Screen "$($folderName)"
    if ($folderName -match $global:regexForRelativityVersion) {
      [void] $global:allRelativityVersions.Add($folderName)      
    }
  }
  Write-Empty-Line-To-Screen

  Write-Heading-Message-To-Screen "List of Relativity versions identified:"
  $global:allRelativityVersions | ForEach-Object {
    Write-Message-To-Screen "$($_)"
  }
  Write-Empty-Line-To-Screen
}

function Retrieve-DevVms-Created() {
  # Read text file
  foreach ($line in Get-Content $global:devVmCreatedTextFile) {
    [string] $lineTrimmed = $line.Trim()
    if ($lineTrimmed -match $global:regexForRelativityVersion) {
      [void] $global:devVmVersionsCreated.Add($lineTrimmed)
    }
  }  

  Write-Heading-Message-To-Screen "List of DevVm's created:"
  $global:devVmVersionsCreated | ForEach-Object {
    Write-Message-To-Screen "$($_)"
  }
  Write-Empty-Line-To-Screen
}

function Identify-DevVms-To-Create() {
  foreach ($currentRelativityVersion in $global:allRelativityVersions) {
    if (-Not $global:devVmVersionsCreated.Contains($currentRelativityVersion)) {
      [void] $global:devVmVersionsToCreate.Add($currentRelativityVersion)      
    }
  }
  Write-Heading-Message-To-Screen "List of DevVm's To Create:"
  $global:devVmVersionsToCreate | ForEach-Object {
    Write-Message-To-Screen "$($_)"
  }
  Write-Empty-Line-To-Screen
}

function Find-Invariant-Version([string] $relativityVersion) {
  Write-Heading-Message-To-Screen "Look for Invariant version for Relativity version."
  
  $global:invariantVersion = ""

  # Read text file
  foreach ($line in Get-Content $global:devVmRelativityInvariantCompatibilityTextFile) {
    [string] $lineTrimmed = $line.Trim()
    $lineTrimmedStringSplitArray = $lineTrimmed.Split("=")
    [string] $currentLineRelativityVersion = $lineTrimmedStringSplitArray[0].Trim()
    [string] $currentLineInvariantVersion = $lineTrimmedStringSplitArray[1].Trim()

    if ($currentLineRelativityVersion -eq $relativityVersion) {
      $global:invariantVersion = $currentLineInvariantVersion
      $global:foundCompatibleInvariantVersion = $true
    }
  }  

  if ($global:foundCompatibleInvariantVersion) {
    Write-Message-To-Screen "Identified Invariant Version: $($global:invariantVersion)"
  }
  else {
    Write-Message-To-Screen "Could not Identify Invariant Version."
  }

  Write-Message-To-Screen "Finished looking for Invariant version for Relativity version."
}

function Copy-Relativity-Installer-And-Response-Files([string] $relativityVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying Relativity Installer and Response files."

  [string] $sourceRelativityFile = "$($global:relativityVersionFolder)\$($relativityVersionToCopy)\RelativityInstallation\GOLD $($relativityVersionToCopy) Relativity.exe"
  [string] $destinationRelativityFile = "$($global:devVmInstallFolder)\Relativity\Relativity.exe"
  Delete-File-If-It-Exists $destinationRelativityFile
  Copy-File-Overwrite-If-It-Exists $sourceRelativityFile $destinationRelativityFile

  [string] $sourceRelativityResponseFile = "$($global:relativityVersionFolder)\$($relativityVersionToCopy)\RelativityInstallation\RelativityResponse.txt"
  [string] $destinationRelativityResponseFile = "$($global:devVmInstallFolder)\Relativity\RelativityResponse.txt"
  Delete-File-If-It-Exists $destinationRelativityResponseFile
  Copy-File-Overwrite-If-It-Exists $sourceRelativityResponseFile $destinationRelativityResponseFile
  
  #todo: Testing only
  Add-Content -Path $destinationRelativityResponseFile -Value "`n"
  Add-Content -Path $destinationRelativityResponseFile -Value "SKIPSSECRETSTOREREQUIREMENT=1"

  Write-Message-To-Screen "Copied Relativity Installer and Response files."
  Write-Empty-Line-To-Screen
}

function Copy-Invariant-Installer-And-Response-Files([string] $invariantVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying Invariant Installer and Response files."
  
  [string] $sourceInvariantFile = "$($global:invariantVersionFolder)\Invariant $($invariantVersionToCopy)\GOLD $($invariantVersionToCopy) Invariant.exe"
  [string] $destinationInvariantFile = "$($global:devVmInstallFolder)\Invariant\Invariant.exe"
  Delete-File-If-It-Exists $destinationInvariantFile
  Copy-File-Overwrite-If-It-Exists $sourceInvariantFile $destinationInvariantFile
  
  [string] $sourceInvariantResponseFile = "$($global:invariantVersionFolder)\Invariant $($invariantVersionToCopy)\InvariantResponse.txt"
  [string] $destinationInvariantResponseFile = "$($global:devVmInstallFolder)\Invariant\InvariantResponse.txt"
  Delete-File-If-It-Exists $destinationInvariantResponseFile
  Copy-File-Overwrite-If-It-Exists $sourceInvariantResponseFile $destinationInvariantResponseFile

  Write-Message-To-Screen "Copied Invariant Installer and Response files."
  Write-Empty-Line-To-Screen
}

function Run-DevVm-Creation-Script([string] $relativityVersionToCreate) {
  Write-Heading-Message-To-Screen "Running DevVm creation script."

  # Run DevVm Script
  &"$PSScriptroot\CreateDevVm.ps1"

  Write-Message-To-Screen "Ran DevVm creation script."
  Write-Empty-Line-To-Screen
}

function Copy-DevVm-7Zip-To_Network-Storage([string] $relativityVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying DevVm created [$($relativityVersionToCopy)] to Network storage."

  [string] $sourceZipFilePath = "$($global:vmExportPath)\$($global:vmName).7z"
  [string] $destinationFilePath = "$($global:devVmNetworkStorageLocation)\$($global:vmName)-$($relativityVersionToCopy).7z"
  
  if (Test-Path $sourceZipFilePath) {
    Copy-File-Overwrite-If-It-Exists $sourceZipFilePath $destinationFilePath  
  }
  else {
    Write-Message-To-Screen "File[$($sourceZipFilePath)] doesn't exist. Skipped copying to Network storage."
  }  

  Write-Message-To-Screen "Copied DevVm created [$($relativityVersionToCopy)] to Network storage."
  Write-Empty-Line-To-Screen
}

function Check-DevVm-Result-Text-File-For-Success() {
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

function Update-Text-File-With-DevVm-Version-Created([string] $relativityVersionCreated) {
  Write-Heading-Message-To-Screen "Writing DevVm version created [$($relativityVersionCreated)] to text file. [$($global:devVmCreatedTextFile)]"
  
  Write-Message-To-Screen "DevVm Creation Status: $($global:devVmCreationWasSuccess)"

  Check-DevVm-Result-Text-File-For-Success

  if ($global:devVmCreationWasSuccess -eq $true) {
    Add-Content -Path $global:devVmCreatedTextFile -Value "`r`n$($relativityVersionCreated)"
  }
  else {
    Write-Error-Message-To-Screen "DevVM creation FAILED!"
  }
  
  Write-Message-To-Screen "Written DevVm version created [$($relativityVersionCreated)] to text file. [$($global:devVmCreatedTextFile)]"
  Write-Empty-Line-To-Screen
}

function Create-DevVm([string] $relativityVersionToCreate) {
  Write-Heading-Message-To-Screen "Creating DevVm. [$($relativityVersionToCreate)]"

  $global:devVmCreationWasSuccess = $false
  $env:DevVmCreationErrorStatus = "false"
  Write-Message-To-Screen "Total attempts: $($global:maxRetry)"

  Do {
    try {
      Write-Heading-Message-To-Screen "Attempt #$($global:count)"
    
      # Find Invariant version
      Find-Invariant-Version $relativityVersionToCreate

      if ($global:foundCompatibleInvariantVersion) {
        # Copy-Relativity-Installer-And-Response-Files $relativityVersionToCreate
        # Copy-Invariant-Installer-And-Response-Files $global:invariantVersion
        Run-DevVm-Creation-Script
        Write-Message-To-Screen "Created DevVm. [$($relativityVersionToCreate)]"

        # Copy 7zip file to network drive with the version number in name
        Copy-DevVm-7Zip-To_Network-Storage $relativityVersionToCreate

        # Update text file with DevVm version created
        Update-Text-File-With-DevVm-Version-Created $relativityVersionToCreate
      }
      else {
        throw "Skipped DevVM Creation. Could not Identify Invariant Version for Relativity Version[$($relativityVersionToCreate)]"
      }
    }
    Catch [Exception] {
      $global:count++
      Write-Message-To-Screen "Exception: $($_.Exception.GetType().FullName)"
      Write-Message-To-Screen "Exception Message: $($_.Exception.Message)"
    }
    finally {
      Check-DevVm-Result-Text-File-For-Success
    }
    
    Write-Heading-Message-To-Screen "Retry variables:"
    Write-Message-To-Screen "env:DevVmCreationErrorStatus: $($env:DevVmCreationErrorStatus)"
    Write-Message-To-Screen "global:devVmCreationWasSuccess: $($global:devVmCreationWasSuccess)"
    Write-Message-To-Screen "global:count: $($global:count)"
    Write-Message-To-Screen "global:maxRetry: $($global:maxRetry)"
  }  while (($env:DevVmCreationErrorStatus -eq "true") -And ($global:devVmCreationWasSuccess -eq $false) -And ($global:count -le $global:maxRetry))

  if ($global:devVmCreationWasSuccess -eq $false) {
    Write-Error-Message-To-Screen "DevVM creation failed. Attempted $($global:count - 1) times."
  }

  Write-Empty-Line-To-Screen
}

function Create-DevVms() {
  if ($global:devVmVersionsToCreate.Count -gt 0) {
    $global:devVmVersionsToCreate | ForEach-Object {
      [string] $relativityVersionToCreate = $_

      Write-Message-To-Screen "#################### DevVm - [$($relativityVersionToCreate)] ####################"
    
      # Create DevVM as a 7Zip file
      Create-DevVm $relativityVersionToCreate

      Write-Message-To-Screen "#################### DevVm - [$($relativityVersionToCreate)] ####################"
      Write-Empty-Line-To-Screen
      Write-Empty-Line-To-Screen
      Write-Empty-Line-To-Screen
    }
  }
  else {
    Write-Heading-Message-To-Screen "All DevVM's already created. Skipped Creation."
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

function Initialize() {
  Reset-Logs-Environment-Variable
 
  # Create Logs file if it not already exists
  If ($env:DevVmAutomationLogFilePath -eq $global:devVmAutomationLogFileResetText) {
    Write-Host-Custom "DevVM Automation Logs file doesn't exist."
    Create-Log-File
  }
}

Initialize
Delete-DevVm-Creation-Result-File
Retrieve-All-Relativity-Versions-Released
Retrieve-DevVms-Created
Identify-DevVms-To-Create
Create-DevVms
