Clear-Host

function Write-Host-Custom ([string] $writeMessage) {
  Write-Host "$($writeMessage)`n" -ForegroundColor Magenta
}

function Write-Host-Custom-Red ([string] $writeMessage) {
  Write-Host "$($writeMessage)`n" -ForegroundColor Red
}

function Write-Host-Custom-Green ([string] $writeMessage) {
  Write-Host "$($writeMessage)`n" -ForegroundColor Green
}

# Retrieve values from DevVm_Automation_Config.json file
[string] $devVmAutomationConfigFilePath = "D:\DevVm_Automation_Config.json"
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
[string] $global:devVmLocalDriveStorageLocation = "T:\DevVmImages"
Write-Host "global:devVmLocalDriveStorageLocation = $($global:devVmLocalDriveStorageLocation)"

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
[string] $global:vmNameAfterCreation = ""
[string] $global:vmExportPath = "D:\DevVmExport"
[Int32] $global:invariantVersionSqlRecordCount = 0
[string] $global:devVmCreationResultFileName = "result_file.txt"
[Boolean] $global:devVmCreationWasSuccess = $false
[string] $global:compressedFileExtension = "zip"
[string] $global:relativityInvariantVersionNumberFileName = "relativity_invariant_version.txt"
[string] $global:testSingleRelativityVersion = "11.1.457.11" # Leave it blank when in Automated Production mode
[string] $global:invariantVersion = "6.1.457.7" # Leave it blank when in Automated Production mode
[string] $global:releaseName = "Junipher 11.1 Server" # Make sure to update this to the correct release name for the Relativity Version above, leave it blank when in Automated Production mode
[Boolean] $global:foundCompatibleInvariantVersion = $true # Set to $false when in Automated Production mode

# Define Toggle variables
[Boolean] $global:toggleSimulateDevVmCreation = $false # Set to $true when you want to simulate the DevVm creation to test the other parts of the automation script
[Boolean] $global:toggleSendSlackMessage = $true # Set to $false when you do not want to send a slack message
[Boolean] $global:toggleCopyToLocalNetworkStorage = $true # Set to $false when you do not want to copy the DevVm to the network storage
[Boolean] $global:toggleCopyToLocalDriveStorage = $true # Set to $false when you do not want to copy the DevVm to the local drive storage
[Boolean] $global:toggleUploadToAzureDevVmBlobStorage = $true # Set to $false when you do not want to copy the DevVm to the network storage
[Boolean] $global:toggleAddVersionToSolutionSnapshotDatabase = $true # Set to $false when you do not want to add the Relativity Version to the Solution Snapshot Database
[Boolean] $global:toggleSkipCopyingRelativityAndInvariantInstallerAndResponseFiles = $false # Set to $true when you want to create DevVM with pre-release Relativity Versions. Remember to manually copy the Relativity and Invariant installer and response files to the network storage
 
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

  # Make sure we are in the same folder where the current running script (AutomationScript.ps1) exists
  Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
  Set-Location $PSScriptroot
  
  # Run separate DevVm Creation PowerShell Script
  &"$PSScriptroot\CreateDevVm.ps1"

  Write-Message-To-Screen "Ran DevVm creation script."
  Write-Empty-Line-To-Screen
}

function Copy-DevVm-Zip-File-To-All-File-Storages([string] $relativityVersionToCopy) {
  Write-Heading-Message-To-Screen "Copying DevVm created [$($relativityVersionToCopy)] to File storage(s)."
   
  [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
  [string] $majorRelativityVersion = "$($relativityVersion.Major).$($relativityVersion.Minor)"
  
  # Copy to Network Storage
  if ($global:toggleCopyToLocalNetworkStorage -eq $true) {
    Write-Message-To-Screen "toggleCopyToLocalNetworkStorage is set to True"

    [string] $destinationFileParentFolderPathForNetworkStorage = "$($global:devVmNetworkStorageLocation)\$($majorRelativityVersion)"
    [string] $destinationFilePathForNetworkStorage = "$($destinationFileParentFolderPathForNetworkStorage)\$($global:vmNameAfterCreation).$($global:compressedFileExtension)"    
    
    # Create Parent folder if it doesn't already exist
    If (!(test-path $destinationFileParentFolderPathForNetworkStorage)) {
      New-Item -ItemType Directory -Force -Path $destinationFileParentFolderPathForNetworkStorage
    }
    Copy-DevVm-Zip-File $relativityVersionToCopy $destinationFilePathForNetworkStorage
  }
  else {
    Write-Message-To-Screen "toggleCopyToLocalNetworkStorage is set to False"    
  }

  # Copy to Local Drive
  if ($global:toggleCopyToLocalDriveStorage -eq $true) {
    Write-Message-To-Screen "toggleCopyToLocalDriveStorage is set to True"

    [string] $destinationFileParentFolderPathForLocalDriveStorage = "$($global:devVmLocalDriveStorageLocation)\$($majorRelativityVersion)"
    [string] $destinationFilePathForLocalDriveStorage = "$($destinationFileParentFolderPathForLocalDriveStorage)\$($global:vmNameAfterCreation).$($global:compressedFileExtension)"    
    
    # Create Parent folder if it doesn't already exist
    If (!(test-path $destinationFileParentFolderPathForLocalDriveStorage)) {
      New-Item -ItemType Directory -Force -Path $destinationFileParentFolderPathForLocalDriveStorage
    }
    Copy-DevVm-Zip-File $relativityVersionToCopy $destinationFilePathForLocalDriveStorage
  }
  else {
    Write-Message-To-Screen "toggleCopyToLocalDriveStorage is set to False"    
  }
  
  Write-Message-To-Screen "Copied DevVm created [$($relativityVersionToCopy)] to All File storages."
  Write-Empty-Line-To-Screen
}

function Copy-DevVm-Zip-File([string] $relativityVersionToCopy, [string] $destinationFilePath) {
  Write-Heading-Message-To-Screen "Copying DevVm created [$($relativityVersionToCopy)] to storage [$($destinationFilePath)]" 
  
  [string] $sourceZipFilePath = "$($global:vmExportPath)\$($global:vmNameAfterCreation).$($global:compressedFileExtension)"
        
  if (Test-Path $sourceZipFilePath) {
    Copy-File-Overwrite-If-It-Exists $sourceZipFilePath $destinationFilePath  
  }
  else {
    Write-Message-To-Screen "Source file[$($sourceZipFilePath)] doesn't exist. Skipped copying to storage [$($destinationFilePath)]"
  }  
  
  Write-Message-To-Screen "Copied DevVm created [$($relativityVersionToCopy)] to storage [$($destinationFilePath)]"
  Write-Empty-Line-To-Screen  
}

function Upload-DevVm-Zip-To-Azure-Blob-Storage([string] $relativityVersionToUpload) {
  if ($global:toggleUploadToAzureDevVmBlobStorage -eq $true) {
    Write-Message-To-Screen "toggleUploadToAzureDevVmBlobStorage is set to True"

    Write-Heading-Message-To-Screen "Uploading DevVM zip file to Azure DevVM Blob Storage"

    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToUpload)
    [string] $majorRelativityVersion = "$($relativityVersion.Major).$($relativityVersion.Minor)"

    [string] $sourceZipFilePath = "$($global:vmExportPath)\$($global:vmNameAfterCreation).$($global:compressedFileExtension)"

    # Make sure we are in the same folder where the current running script (AutomationScript.ps1) exists
    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot
    
    # Run separate DevVM Azure Blob storage Upload PowerShell Script
    &"$PSScriptroot\UploadFileToAzureBlobStorage.ps1" $majorRelativityVersion $sourceZipFilePath "$($global:vmNameAfterCreation).$($global:compressedFileExtension)" $false

    # Example(s) for running the separate DevVM Azure Blob storage Upload PowerShell Script
    # .\UploadFileToAzureBlobStorage.ps1 [parentFolderName] [sourceFileFullPath] [destinationFileName] [skipAzurePsModuleInstallation] # Showing Arguments
    # .\UploadFileToAzureBlobStorage.ps1 "ParentFolder" "S:\Local_DevVms\abc.zip" "abc2.zip" $false
    # .\UploadFileToAzureBlobStorage.ps1 "11.0" "D:\DevVmExport\RelativityDevVm-11.0.232.1.zip" "RelativityDevVm-11.0.232.1.zip" $false
  
    Write-Message-To-Screen "Finished running PowerShell script to Upload DevVM zip file to Azure DevVM Blob Storage"
    Write-Empty-Line-To-Screen
  }
  else {
    Write-Message-To-Screen "toggleUploadToAzureDevVmBlobStorage is set to False"
  }
}

function Send-Slack-Success-Message([string] $relativityVersionToCopy) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($global:toggleSendSlackMessage -eq $true) {
    Write-Message-To-Screen "toggleSendSlackMessage is set to True"

    Write-Heading-Message-To-Screen "Sending Slack Success Message"
    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
    [string] $majorRelativityVersion = "$($relativityVersion.Major).$($relativityVersion.Minor)"

    [string] $destinationFilePathNetworkStorage = "$($global:devVmNetworkStorageLocation)\$($majorRelativityVersion)\$($global:vmNameAfterCreation).$($global:compressedFileExtension)"
    [string] $destinationFilePathLocalDriveStorage = "\\P-DV-DSK-DEVEX\DevVmImages\$($majorRelativityVersion)\$($global:vmNameAfterCreation).$($global:compressedFileExtension)"

    $BodyJSON = @{
      "text" = "New DevVm ($($relativityVersionToCreate)) is available at the following location(s) - [$($destinationFilePathNetworkStorage)] & [$($destinationFilePathLocalDriveStorage)]"
    } | ConvertTo-Json

    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json
    Write-Message-To-Screen "Sent Slack Success Message"
  }
  else {
    Write-Message-To-Screen "toggleSendSlackMessage is set to False"
  }
}

function Send-Slack-Success-Message-Follow-Up-Tasks([string] $relativityVersionToCopy) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($global:toggleSendSlackMessage -eq $true) {
    Write-Message-To-Screen "toggleSendSlackMessage is set to True"

    Write-Heading-Message-To-Screen "Sending Slack Success Message - Follow Up Tasks"
    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
    $BodyJSON = @{
      "text" = "Follow this checklist (https://einstein.kcura.com/x/61e3C) for the new Relativity Version ($($relativityVersionToCreate))."
    } | ConvertTo-Json

    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_tools_group_key -ContentType application/json
    Write-Message-To-Screen "Sent Slack Success Message - Follow Up Tasks"
  }
  else {
    Write-Message-To-Screen "toggleSendSlackMessage is set to False"
  }
}

function Send-Slack-Failure-Message([string] $relativityVersionToCopy) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($global:toggleSendSlackMessage -eq $true) {
    Write-Message-To-Screen "toggleSendSlackMessage is set to True"

    Write-Heading-Message-To-Screen "Sending Slack Failure Message"
    [System.Version] $relativityVersion = [System.Version]::Parse($relativityVersionToCopy)
    $BodyJSON = @{
      "text" = "Failed to create Relativity DevVm ($($relativityVersionToCreate))"
    } | ConvertTo-Json

    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_tools_group_key -ContentType application/json
    Write-Message-To-Screen "Sent Slack Failure Message"
  }
  else {
    Write-Message-To-Screen "toggleSendSlackMessage is set to False"
  }
}

function Add-Relativity-Version-To-Solution-Snapshot-Database([string] $relativityVersion) {
  if ($global:toggleAddVersionToSolutionSnapshotDatabase -eq $true) {
    Write-Message-To-Screen "toggleAddVersionToSolutionSnapshotDatabase is set to True"

    Write-Heading-Message-To-Screen "Adding Relativity Version to the Solution Snapshot Database"

    # Make sure we are in the same folder where the current running script (AutomationScript.ps1) exists
    Write-Message-To-Screen "PSScriptroot: $($PSScriptroot)"
    Set-Location $PSScriptroot
    
    # Run separate Solution Snapshot API Calls PowerShell Script
    &"$PSScriptroot\AddRelativityVersionToSolutionSnapshotDatabase.ps1" "$Env:devvm_automation_salesforce_username" "$Env:devvm_automation_salesforce_password" "$relativityVersion" $global:sendSlackMessage $global:releaseName
  
    Write-Message-To-Screen "Ran Add Relativity Version to Solution Snapshot Database script."
    Write-Empty-Line-To-Screen
  }
  else {
    Write-Message-To-Screen "toggleAddVersionToSolutionSnapshotDatabase is set to False"
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

  # Set new VM name in a variable which will be used in the CreateDevVm.ps1 script to rename the VM once it's created
  $global:vmNameAfterCreation = "$($global:vmName)-$($relativityVersionToCreate)"

  $global:devVmCreationWasSuccess = $false
  Write-Message-To-Screen "Total attempts: $($global:maxRetry)"

  Do {
    try {
      Write-Heading-Message-To-Screen "Attempt #$($global:count)"
    
      # Find Invariant version
      # Find-Invariant-Version $relativityVersionToCreate

      if ($global:foundCompatibleInvariantVersion) {

        if ($global:toggleSimulateDevVmCreation -eq $true) {
          Write-Message-To-Screen "toggleSimulateDevVmCreation is set to True"

          $global:devVmCreationWasSuccess = $true
          Write-Message-To-Screen "Skipped DevVm creation! (In DevVM Creation Siumlation mode)"
        }
        else {
          Write-Message-To-Screen "toggleSimulateDevVmCreation is set to False"

          # Copy Relativity and Invariant Installer and Respponse Files
          if ($global:toggleSkipCopyingRelativityAndInvariantInstallerAndResponseFiles -eq $true) {
            Write-Message-To-Screen "Skipped Copying Relativity and Invariant Installer and Response Files for Pre-Release DevVM [`$global:toggleSkipCopyingRelativityAndInvariantInstallerAndResponseFiles: $($global:toggleSkipCopyingRelativityAndInvariantInstallerAndResponseFiles)]....."
          } 
          else {
            Copy-Relativity-Installer-And-Response-Files $relativityVersionToCreate
            Copy-Invariant-Installer-And-Response-Files $global:invariantVersion
            Copy-Relativity-And-Invariant-Version-Numbers-To-Text-File $relativityVersionToCreate $global:invariantVersion
          }

          # Create DevVM
          Run-DevVm-Creation-Script
          Write-Message-To-Screen "Created DevVm. [$($relativityVersionToCreate)]"

          Check-DevVm-Result-Text-File-For-Success
        }        
       
        if ($global:devVmCreationWasSuccess -eq $true) {
          # Copy Zip file to all file storage locations with the version number in name
          Copy-DevVm-Zip-File-To-All-File-Storages $relativityVersionToCreate

          # Upload Zip file to Azure DevVM Blob storage with the version number in name
          Upload-DevVm-Zip-To-Azure-Blob-Storage $relativityVersionToCreate
          
          # Send Slack Message that copying to all the local file storage(s) succeeded
          Send-Slack-Success-Message $relativityVersionToCreate
          
          # Send Slack Message to the Tools Slack channel to remind about the follow up tasks
          Send-Slack-Success-Message-Follow-Up-Tasks $relativityVersionToCreate
          
          # Add Relativity Version to Solution Snapshot Database
          Add-Relativity-Version-To-Solution-Snapshot-Database $relativityVersionToCreate
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