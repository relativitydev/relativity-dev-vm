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

# Retrieve values from DevVm_Automation_Config.json file
[string] $devVmAutomationConfigFilePath = "C:\DevVm_Automation_Config.json"
[string] $json = Get-Content -Path $devVmAutomationConfigFilePath
$jsonContents = $json | ConvertFrom-Json

[string] $global:releaseSqlServer = $jsonContents.releaseSqlServer
Write-Host "global:releaseSqlServer = $($global:releaseSqlServer)"
[string] $global:releaseSqlServerDatabase = $jsonContents.releaseSqlServerDatabase
Write-Host "global:releaseSqlServerDatabase = $($global:releaseSqlServerDatabase)"
[string] $global:releaseSqlServerLogin = $jsonContents.releaseSqlServerLogin
Write-Host "global:releaseSqlServerLogin = $($global:releaseSqlServerLogin)"
[string] $global:releaseSqlServerPassword = $jsonContents.releaseSqlServerPassword
Write-Host "global:releaseSqlServerPassword = $($global:releaseSqlServerPassword)"
[string] $global:relativityVersionFolders = $jsonContents.relativityVersionFolders
Write-Host "global:relativityVersionFolders = $($global:relativityVersionFolders)"
[string] $global:devVmInstallFolder = $jsonContents.devVmInstallFolder
Write-Host "global:devVmInstallFolder = $($global:devVmInstallFolder)"
[string] $global:devVmAutomationLogsFolder = $jsonContents.devVmAutomationLogsFolder
Write-Host "global:devVmAutomationLogsFolder = $($global:devVmAutomationLogsFolder)"
[string] $global:devVmNetworkStorageLocation = $jsonContents.devVmNetworkStorageLocation
Write-Host "global:devVmNetworkStorageLocation = $($global:devVmNetworkStorageLocation)"

# Display parsed Relativity and Invariant folder arrays
Write-Host ""
$global:relativityVersionFoldersList = $global:relativityVersionFolders.Split(";")
Write-Host "global:relativityVersionFoldersList = $($global:relativityVersionFoldersList)"
Write-Host ""

[string] $global:currentRelativityVersionCreating = ""
[System.Int32]$global:maxRetry = 3
[System.Int32]$global:count = 1
[string] $global:regexForRelativityVersion = "\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"
[string] $global:devVmAutomationLogFilePrefix = "DevVm_Automation_Log"
[string] $global:devVmAutomationLogFileResetText = "empty"
$global:allRelativityVersionsReleased = New-Object System.Collections.ArrayList
$global:devVmVersionsCreated = New-Object System.Collections.ArrayList
$global:devVmVersionsToCreate = New-Object System.Collections.ArrayList
[string] $global:vmName = "RelativityDevVm"
[string] $global:vmExportPath = "D:\DevVmExport"
[Int32] $global:invariantVersionSqlRecordCount = 0
[string] $global:devVmCreationResultFileName = "result_file.txt"
[Boolean] $global:devVmCreationWasSuccess = $false
[string] $global:compressedFileExtension = "zip"
[string] $global:relativityInvariantVersionNumberFileName = "relativity_invariant_version.txt"
[string] $global:testSingleRelativityVersion = "10.3.142.22" # Leave it blank when in Automated Production mode
[string] $global:invariantVersion = "5.3.135.1" # Leave it blank when in Automated Production mode
[Boolean] $global:foundCompatibleInvariantVersion = $true # Set to $false when in Automated Production mode
[Boolean] $global:sendSlackMessage = $true # Set to $false when you do not want to send a slack message
 
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

  $global:relativityVersionFoldersList | ForEach-Object {
    $currentRelativityVersionFolder = $_

    $allChildFolders = Get-ChildItem -Path $currentRelativityVersionFolder
    Write-Heading-Message-To-Screen "List of Child items in folder [$($currentRelativityVersionFolder)]:"
    $allChildFolders | ForEach-Object {
      $folderName = ($_.Name).Trim()
      Write-Message-To-Screen "$($folderName)"
      if ($folderName -match $global:regexForRelativityVersion) {
        [void] $global:allRelativityVersionsReleased.Add($folderName)      
      }
    }
    Write-Empty-Line-To-Screen
  }

  Write-Heading-Message-To-Screen "List of Relativity versions identified:"
  $global:allRelativityVersionsReleased | ForEach-Object {
    Write-Message-To-Screen "$($_)"
  }
  Write-Empty-Line-To-Screen
}
  
function Retrieve-DevVms-Created() {
  # Retrieve all DevVM images
  $allDevVmImagesCreated = Get-ChildItem -Path $global:devVmNetworkStorageLocation | Sort-Object -Property LastWriteTime
    
  Write-Heading-Message-To-Screen "List of Child items in folder [$($global:devVmNetworkStorageLocation)]:"

  $allDevVmImagesCreated | ForEach-Object {
    [string] $imageName = ($_.Name).Trim()
    Write-Message-To-Screen "$($imageName)"
    [string] $versionWithExtension = $imageName -replace "$($global:vmName)-", ""
    [string] $version = $versionWithExtension -replace ".$($global:compressedFileExtension)", ""
    if ($version -match $global:regexForRelativityVersion) {
      [void] $global:devVmVersionsCreated.Add($version)      
    }
  }

  Write-Empty-Line-To-Screen

  Write-Heading-Message-To-Screen "List of DevVm's created:"
  $global:devVmVersionsCreated | ForEach-Object {
    Write-Message-To-Screen "$($_)"
  }
  Write-Empty-Line-To-Screen
}

function Identify-DevVms-To-Create() {
  # First Add the ones which aren't already created 
  foreach ($currentRelativityVersion1 in $global:allRelativityVersionsReleased) {
    if (-Not $global:devVmVersionsCreated.Contains($currentRelativityVersion1)) {
      [void] $global:devVmVersionsToCreate.Add($currentRelativityVersion1)      
    }
  }

  # # Then Add the ones which were already created, old ones first
  # foreach ($currentRelativityVersion2 in $global:devVmVersionsCreated) {
  #   [void] $global:devVmVersionsToCreate.Add($currentRelativityVersion2)      
  # }

  Write-Heading-Message-To-Screen "List of DevVm's To Create:"
  $global:devVmVersionsToCreate | ForEach-Object {
    Write-Message-To-Screen "$($_)"
  }
  Write-Empty-Line-To-Screen
}

function Check-If-Only-One-Invariant-Sql-Record-Exists ([string] $relativityVersion) {
  Write-Heading-Message-To-Screen "Checking if only 1 invariant sql record exists."

  if ($relativityVersion -eq '') {
    throw "relativityVersion passed is empty."
  }
  
  $global:invariantVersionSqlRecordCount = 0
  try {
    # Create and open a database connection
    $sqlConnection = new-object System.Data.SqlClient.SqlConnection "server=$($global:releaseSqlServer);database=$($global:releaseSqlServerDatabase);user id=$($global:releaseSqlServerLogin);password=$($global:releaseSqlServerPassword);trusted_connection=true;"
    $sqlConnection.Open()

    # Create a command object
    $sqlCommand = $sqlConnection.CreateCommand()
    $sqlCommand.CommandText = 
    "
      SELECT 
        COUNT(0)
      FROM 
          [$($global:releaseSqlServer)].[$($global:releaseSqlServerDatabase)].[dbo].[Release]
      WHERE
        [ReleaseNumber] = '$($relativityVersion)'"

    # Execute the Command
    $global:invariantVersionSqlRecordCount = $sqlCommand.ExecuteScalar()
    Write-Message-To-Screen "global:invariantVersionSqlRecordCount: $($global:invariantVersionSqlRecordCount)"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when checking if only 1 invariant sql record exists."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
  finally {
    # Close the database connection
    $sqlConnection.Close()
  }
  Write-Message-To-Screen "Checked if only 1 invariant sql record exists."
}

function Retrieve-Invariant-Version-From-Sql-Server ([string] $relativityVersion) {
  Write-Heading-Message-To-Screen "Retrieving Invarint version from SQL server."
    
  if ($relativityVersion -eq '') {
    throw "relativityVersion passed is empty."
  }

  $global:invariantVersion = ""
  $global:foundCompatibleInvariantVersion = $false
  try {
    # Create and open a database connection
    $sqlConnection = new-object System.Data.SqlClient.SqlConnection "server=$($global:releaseSqlServer); database=$($global:releaseSqlServerDatabase); user id=$($global:releaseSqlServerLogin); password=$($global:releaseSqlServerPassword); trusted_connection=true; "
    $sqlConnection.Open()

    # Create a command object
    $sqlCommand = $sqlConnection.CreateCommand()
    $sqlCommand.CommandText = 
    "SELECT 
      TOP (1) [InvariantReleaseNumber]
    FROM 
      [$($global:releaseSqlServer)].[$($global:releaseSqlServerDatabase)].[dbo].[Release]
    WHERE
      [ReleaseNumber] = '$($relativityVersion)'"

    $global:invariantVersion = $sqlCommand.ExecuteScalar()
    $global:foundCompatibleInvariantVersion = $true
    Write-Message-To-Screen "global:invariantVersion: $($global:invariantVersion)"
    Write-Message-To-Screen "global:foundCompatibleInvariantVersion: $($global:foundCompatibleInvariantVersion)"
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when retrieving Invarint version from SQL server."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
  finally {
    # Close the database connection
    $sqlConnection.Close()
  } 
  Write-Message-To-Screen "Retrieved Invariant version from SQL server."
}

function Find-Invariant-Version([string] $relativityVersion) {
  Write-Heading-Message-To-Screen "Look for Invariant version for Relativity version."
  
  if ($relativityVersion -eq '') {
    throw "relativityVersion passed is empty."
  }

  try {
    Check-If-Only-One-Invariant-Sql-Record-Exists $relativityVersion
    if ($global:invariantVersionSqlRecordCount -eq 0) {
      throw "No Invariant version exists in the Sql database."
    }
    if ($global:invariantVersionSqlRecordCount -gt 1) {
      throw "More than one Invariant version exists in the Sql database."
    }

    Retrieve-Invariant-Version-From-Sql-Server $relativityVersion

    if ($global:foundCompatibleInvariantVersion) {
      Write-Message-To-Screen "Identified Invariant Version: $($global:invariantVersion)"
    }
    else {
      Write-Message-To-Screen "Could not Identify Invariant Version."
    }
  }
  Catch [Exception] {
    Write-Error-Message-To-Screen "An error occured when looking for Invariant version for Relativity version."
    Write-Error-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Error-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
  
  Write-Message-To-Screen "Finished looking for Invariant version for Relativity version."
}

function Copy-Relativity-Installer-And-Response-Files([string] $relativityVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying Relativity Installer and Response files."

  [string] $sourceRelativityFile = ""
  [string] $sourceRelativityResponseFile = ""

  $global:relativityVersionFoldersList | ForEach-Object {
    [string] $currentRelativityVersionFolder = $_
    $relativityVersionToCopySplit = $relativityVersionToCopy.Split(".")
    [string] $majorRelativityVersion = "$($relativityVersionToCopySplit[0]).$($relativityVersionToCopySplit[1])"

    if ($currentRelativityVersionFolder.Contains($majorRelativityVersion)) {
      $global:currentRelativityVersionCreating = "$($currentRelativityVersionFolder)" # This vairable will be used when copying invariant install file
      $sourceRelativityFile = "$($currentRelativityVersionFolder)\$($relativityVersionToCopy)\RelativityInstallation\GOLD $($relativityVersionToCopy) Relativity.exe"
      $sourceRelativityResponseFile = "$($currentRelativityVersionFolder)\$($relativityVersionToCopy)\RelativityInstallation\RelativityResponse.txt"
    }
  }

  [string] $destinationRelativityFile = "$($global:devVmInstallFolder)\Relativity\Relativity.exe"
  Delete-File-If-It-Exists $destinationRelativityFile
  Copy-File-Overwrite-If-It-Exists $sourceRelativityFile $destinationRelativityFile

  [string] $destinationRelativityResponseFile = "$($global:devVmInstallFolder)\Relativity\RelativityResponse.txt"
  Delete-File-If-It-Exists $destinationRelativityResponseFile
  Copy-File-Overwrite-If-It-Exists $sourceRelativityResponseFile $destinationRelativityResponseFile

  Write-Message-To-Screen "Copied Relativity Installer and Response files."
  Write-Empty-Line-To-Screen
}

function Copy-Invariant-Installer-And-Response-Files([string] $invariantVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying Invariant Installer and Response files."
  
  [string] $sourceInvariantFile = ""
  [string] $sourceInvariantResponseFile = ""

  $global:relativityVersionFoldersList | ForEach-Object {
    [string] $currentRelativityVersionFolder = $_
    $global:currentRelativityVersionCreatingSplit = $global:currentRelativityVersionCreating.Split(".")
    [string] $majorRelativityVersion = "$($global:currentRelativityVersionCreatingSplit[0]).$($global:currentRelativityVersionCreatingSplit[1])"

    if ($currentRelativityVersionFolder.Contains($majorRelativityVersion)) {
      $global:currentRelativityVersionCreating = "$($currentRelativityVersionFolder)"
      $sourceInvariantFile = "$($currentRelativityVersionFolder)\Invariant\Invariant $($invariantVersionToCopy)\GOLD $($invariantVersionToCopy) Invariant.exe"
      $sourceInvariantResponseFile = "$($currentRelativityVersionFolder)\Invariant\Invariant $($invariantVersionToCopy)\InvariantResponse.txt"
    }
  }

  [string] $destinationInvariantFile = "$($global:devVmInstallFolder)\Invariant\Invariant.exe"
  Delete-File-If-It-Exists $destinationInvariantFile
  Copy-File-Overwrite-If-It-Exists $sourceInvariantFile $destinationInvariantFile
  
  [string] $destinationInvariantResponseFile = "$($global:devVmInstallFolder)\Invariant\InvariantResponse.txt"
  Delete-File-If-It-Exists $destinationInvariantResponseFile
  Copy-File-Overwrite-If-It-Exists $sourceInvariantResponseFile $destinationInvariantResponseFile

  Write-Message-To-Screen "Copied Invariant Installer and Response files."
  Write-Empty-Line-To-Screen
}

function Copy-Relativity-And-Invariant-Version-Numbers-To-Text-File([string] $relativityVersionToCopy, [string] $invariantVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying Relativity and Invariant Version Numbers to Text File."

  # Copy Relativity and Invariant version number 
  [string] $relativityInvairantVersionNumberFileNameWithPath = "$($global:devVmInstallFolder)\Relativity\$($global:relativityInvariantVersionNumberFileName)"
  Delete-File-If-It-Exists $relativityInvairantVersionNumberFileNameWithPath
  If (Test-Path $relativityInvairantVersionNumberFileNameWithPath) {
    Remove-Item -path $relativityInvairantVersionNumberFileNameWithPath -Force
  }
  New-Item $relativityInvairantVersionNumberFileNameWithPath -type file -Force
  [string] $fileContent = "Relativity_Version: $($relativityVersionToCopy), Invariant_Version: $($invariantVersionToCopy)"
  Set-Content -Path $relativityInvairantVersionNumberFileNameWithPath -Value $fileContent -Force

  Write-Message-To-Screen "Copied Relativity and Invariant Version Numbers to Text File. [$($relativityInvairantVersionNumberFileNameWithPath)]"
  Write-Empty-Line-To-Screen
}

function Run-DevVm-Creation-Script([string] $relativityVersionToCreate) {
  Write-Heading-Message-To-Screen "Running DevVm creation script."

  # Make sure we are in the folder where the running script exists
  Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
  Set-Location $PSScriptroot
  
  # Run DevVm Script
  &"$PSScriptroot\CreateDevVm.ps1"

  Write-Message-To-Screen "Ran DevVm creation script."
  Write-Empty-Line-To-Screen
}

function Copy-DevVm-Zip-To-Network-Storage([string] $relativityVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying DevVm created [$($relativityVersionToCopy)] to Network storage."
   
  [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
  [string] $majorRelativityVersion = "$($relativityVersion.Major).$($relativityVersion.Minor)"

  [string] $sourceZipFilePath = "$($global:vmExportPath)\$($global:vmName).$($global:compressedFileExtension)"
  [string] $destinationFilePath = "$($global:devVmNetworkStorageLocation)\$($majorRelativityVersion)\$($global:vmName).$($global:compressedFileExtension)"
  
  if (Test-Path $sourceZipFilePath) {
    Copy-File-Overwrite-If-It-Exists $sourceZipFilePath $destinationFilePath  
  }
  else {
    Write-Message-To-Screen "File[$($sourceZipFilePath)] doesn't exist. Skipped copying to Network storage."
  }  

  Write-Message-To-Screen "Copied DevVm created [$($relativityVersionToCopy)] to Network storage."
  Write-Empty-Line-To-Screen
}

function Send-Slack-Success-Message([string] $relativityVersionToCopy){
  if($global:sendSlackMessage -eq $true){
    Write-Heading-Message-To-Screen "Sending Slack Success Message"
    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
    [string] $majorRelativityVersion = "$($relativityVersion.Major).$($relativityVersion.Minor)"
    [string] $destinationFilePath = "$($global:devVmNetworkStorageLocation)\$($majorRelativityVersion)\$($global:vmName).$($global:compressedFileExtension)"
    $BodyJSON = @{
      "text" = "New DevVm ($($relativityVersionToCreate)) is available at $($destinationFilePath)"
    } | ConvertTo-Json

    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri "https://hooks.slack.com/services/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" -ContentType application/json
    Write-Message-To-Screen "Sent Slack Success Message"
  }
}

function Send-Slack-Success-Message-Follow-Up-Tasks([string] $relativityVersionToCopy){
  if($global:sendSlackMessage -eq $true){
    Write-Heading-Message-To-Screen "Sending Slack Success Message - Follow Up Tasks"
    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
    $BodyJSON = @{
      "text" = "REMINDER: 1. Please add Relativity Version ($($relativityVersionToCreate)) to Solution Snapshot Database. 2. Inform DevCon team to update Compatibility info for the Applications. 3. Publish NuGet packages."
    } | ConvertTo-Json

    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri "https://hooks.slack.com/services/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" -ContentType application/json
    Write-Message-To-Screen "Sent Slack Success Message - Follow Up Tasks"
  }
}

function Send-Slack-Failure-Message([string] $relativityVersionToCopy){
  if($global:sendSlackMessage -eq $true){
    Write-Heading-Message-To-Screen "Sending Slack Failure Message"
    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
    $BodyJSON = @{
      "text" = "Failed to create Relativity DevVm ($($relativityVersionToCreate))"
    } | ConvertTo-Json

    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri "https://hooks.slack.com/services/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" -ContentType application/json
    Write-Message-To-Screen "Sent Slack Failure Message"
  }
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

function Create-DevVm([string] $relativityVersionToCreate) {
  Write-Heading-Message-To-Screen "Creating DevVm. [$($relativityVersionToCreate)]"

  $global:devVmCreationWasSuccess = $false
  Write-Message-To-Screen "Total attempts: $($global:maxRetry)"

  Do {
    try {
      Write-Heading-Message-To-Screen "Attempt #$($global:count)"
    
      # Find Invariant version
      # Find-Invariant-Version $relativityVersionToCreate

      if ($global:foundCompatibleInvariantVersion) {
        Copy-Relativity-Installer-And-Response-Files $relativityVersionToCreate
        Copy-Invariant-Installer-And-Response-Files $global:invariantVersion
        Copy-Relativity-And-Invariant-Version-Numbers-To-Text-File $relativityVersionToCreate $global:invariantVersion
        Run-DevVm-Creation-Script
        Write-Message-To-Screen "Created DevVm. [$($relativityVersionToCreate)]"

        Check-DevVm-Result-Text-File-For-Success
       
        if ($global:devVmCreationWasSuccess -eq $true) {
          # Copy Zip file to network drive with the version number in name
          Copy-DevVm-Zip-To-Network-Storage $relativityVersionToCreate
          # Send Slack Message that upload to the network storage succeeded
          Send-Slack-Success-Message $relativityVersionToCreate
          # Send Slack Message to the Tools Slack channel to remind about the follow up tasks
          Send-Slack-Success-Message-Follow-Up-Tasks $relativityVersionToCreate
        }
        else {
          $global:count++
          Write-Message-To-Screen "DevVm creation failed. Skipped copying zip file to network storage."
        }
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
    Write-Message-To-Screen "global:devVmCreationWasSuccess: $($global:devVmCreationWasSuccess)"
    Write-Message-To-Screen "global:count: $($global:count)"
    Write-Message-To-Screen "global:maxRetry: $($global:maxRetry)"
  }  while (($global:devVmCreationWasSuccess -eq $false) -And ($global:count -le $global:maxRetry))

  if ($global:devVmCreationWasSuccess -eq $false) {
    Write-Error-Message-To-Screen "DevVM creation failed. Attempted $($global:count - 1) times."
    Send-Slack-Failure-Message $relativityVersionToCreate
  }

  Write-Empty-Line-To-Screen
}

function Create-DevVms() {
  if ($global:devVmVersionsToCreate.Count -gt 0) {
    $global:devVmVersionsToCreate | ForEach-Object {
      [string] $relativityVersionToCreate = $_

      Write-Message-To-Screen "#################### DevVm - [$($relativityVersionToCreate)] ####################"
    
      # Check for testing mode
      if ($global:testSingleRelativityVersion -eq "") {
        # In Production mode
        # Create DevVM as a Zip file
        Create-DevVm $relativityVersionToCreate
      }
      else {
        # In Testing mode
        if ($relativityVersionToCreate -eq $global:testSingleRelativityVersion) {
          # Create DevVM as a Zip file
          Create-DevVm $relativityVersionToCreate
        }
        else {
          Write-Message-To-Screen "In Testing mode - skipped DevVM creation."
        }
      }

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

  # Make sure we are in the folder where the running script exists
  Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
  Set-Location $PSScriptroot
}

Initialize
Delete-DevVm-Creation-Result-File
Retrieve-All-Relativity-Versions-Released
Retrieve-DevVms-Created
Identify-DevVms-To-Create
Create-DevVms
