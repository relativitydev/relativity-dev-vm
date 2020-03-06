[string] $global:parentFolderName = $args[0]
[string] $global:sourceFileFullPath = $args[1]
[string] $global:destinationFileName = $args[2]

function Write-Host-Custom-Blue ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Blue
}

function Write-Host-Custom-Green ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Green
}

function Write-Host-Custom-Red ([string] $writeMessage) {
  Write-Host $writeMessage -ForegroundColor Red
}

function Upload-File-To-Azure-Blob-Storage {
  Write-Host-Custom-Green "parentFolderName: $($global:parentFolderName)"
  Write-Host-Custom-Green "sourceFileFullPath: $($global:sourceFileFullPath)"
  Write-Host-Custom-Green "destinationFileName: $($global:destinationFileName)"
  Write-Host-Custom-Green "`n"
  
  [string] $error_message = ""

  try {
    if ([string]::IsNullOrWhitespace($global:parentFolderName)) {
      $error_message = "Error: Argument(parentFolderName) is Empty.`n"
      Write-Host-Custom-Red $error_message
      throw $error_message
    }

    if ([string]::IsNullOrWhitespace($global:sourceFileFullPath)) {
      $error_message = "Error: Argument(sourceFileFullPath) is Empty.`n"
      Write-Host-Custom-Red $error_message
      throw $error_message
    }

    if ([string]::IsNullOrWhitespace($global:destinationFileName)) {
      $error_message = "Error: Argument(destinationFileName) is Empty.`n"
      Write-Host-Custom-Red $error_message
      throw $error_message
    }

    # Install Azure Az PS Module
    Write-Host-Custom-Green "Installing Azure Az PS Module.....`n";
    Install-Module -Name Az -AllowClobber -Scope CurrentUser

    # Query for Current Installed Azure Az PS module version 
    Write-Host-Custom-Green "Getting Current Installed Azure Az PS Module Version.....`n";
    Get-InstalledModule -Name Az -AllVersions | Select-Object Name, Version | Write-Host

    #NOTE: There is one manual step here which has to be done on the machine - Connecting to Azure portal with your credentails by running this command in PowerShell - Connect-AzAccount

    # Get All Azure Subscriptions
    Write-Host-Custom-Green "Getting all Azure Subscriptions.....`n";
    Get-AzSubscription

    # Set Azure Subscription to the Production Subscription
    Write-Host-Custom-Green "Setting current Azure Subscription to Production Subscription.....`n";
    Set-AzContext -SubscriptionId "9491b162-7d26-4956-890b-3f882ed78a8d" # Subscription (CS - DevEx - SolutionSnapshot - Prod)

    # Get Azure Storage Account for DevVM blob storage
    Write-Host-Custom-Green "Getting Azure Storage Account for DevVM blob storage.....`n";
    $storageAccount = Get-AzStorageAccount -ResourceGroupName "DevVm" -Name "devvmblob"

    # Get Azure Storage Account Context for DevVM blob storage
    Write-Host-Custom-Green "Getting Azure Storage Account Context for DevVM blob storage.....`n";
    $storageAccountContext = $storageAccount.Context

    # Set Azure Storage Account Container for DevVM blob storage
    $containerName = "images"

    # Upload a file to blob storage
    Write-Host-Custom-Green "Uploading file to Azure DevVM blob storage.....`n";
    Set-AzStorageBlobContent -File $global:sourceFileFullPath -Container $containerName -Blob "$($global:parentFolderName)/$($global:destinationFileName)" -Context $storageAccountContext -Force
    Write-Host-Custom-Green "Uploaded file to Azure DevVM blob storage.....`n";
  }
  Catch [Exception] {
    Write-Host-Custom-Red "An error occured when uploading file to Azure DevVM blob storage [$($global:sourceFileFullPath)]"
    Write-Host-Custom-Red "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Host-Custom-Red "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

Write-Host-Custom-Blue "Start Time: $(Get-Date)`n";

Upload-File-To-Azure-Blob-Storage

Write-Host-Custom-Blue "End Time: $(Get-Date)`n";
