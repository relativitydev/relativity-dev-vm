# Define script arguments
[string] $global:parentFolderName = $args[0]
[string] $global:sourceFileFullPath = $args[1]
[string] $global:destinationFileName = $args[2]

# Define script contants
$global:devVmAzureResourceGroupNameConstant = "DevVm"
$global:azureBlobStorageAccountNameConstant = "devvmblob"
$global:azureBlobContainerNameConstant = "images"

# Define Environment variables
$global:azureSubscriptionIdEnv = $Null

# Define custom display text functions
function Write-Host-Custom-Blue ([string] $writeMessage) {
  Write-Host "$($writeMessage)`n" -ForegroundColor Blue
}

function Write-Host-Custom-Green ([string] $writeMessage) {
  Write-Host "$($writeMessage)`n" -ForegroundColor Green
}

function Write-Host-Custom-Red ([string] $writeMessage) {
  Write-Host "$($writeMessage)`n" -ForegroundColor Red
}

# Functions
function Parse-And-Display-Azure-Environment-Variables {
  Write-Host-Custom-Green "Parsing and Displaying Azure Environment variables....."

  $global:azureSubscriptionIdEnv = "$Env:devvm_azure_blob_storage_subscription_id"
  Write-Host-Custom-Blue "`$global:azureSubscriptionIdEnv: $($global:azureSubscriptionIdEnv)"
}

function Display-Script-Constants {
  Write-Host-Custom-Green "Displaying Script constants....."

  # Display constants
  Write-Host-Custom-Blue "`$global:devVmAzureResourceGroupNameConstant: $($global:devVmAzureResourceGroupNameConstant)"
  Write-Host-Custom-Blue "`$global:azureBlobStorageAccountNameConstant: $($global:azureBlobStorageAccountNameConstant)"
  Write-Host-Custom-Blue "`$global:azureBlobContainerNameConstant: $($global:azureBlobContainerNameConstant)"
}

function Upload-File-To-Azure-Blob-Storage {
  Write-Host-Custom-Green "`$parentFolderName: $($global:parentFolderName)"
  Write-Host-Custom-Green "`$sourceFileFullPath: $($global:sourceFileFullPath)"
  Write-Host-Custom-Green "`$destinationFileName: $($global:destinationFileName)"
  Write-Host-Custom-Green ""
  
  [string] $error_message_for_input_arguments = ""

  try {
    if ([string]::IsNullOrWhitespace($global:parentFolderName)) {
      $error_message_for_input_arguments = "Error: Argument(`$parentFolderName) is Empty."
      Write-Host-Custom-Red $error_message_for_input_arguments
      throw $error_message_for_input_arguments
    }

    if ([string]::IsNullOrWhitespace($global:sourceFileFullPath)) {
      $error_message_for_input_arguments = "Error: Argument(`$sourceFileFullPath) is Empty."
      Write-Host-Custom-Red $error_message_for_input_arguments
      throw $error_message_for_input_arguments
    }

    if ([string]::IsNullOrWhitespace($global:destinationFileName)) {
      $error_message_for_input_arguments = "Error: Argument(`$destinationFileName) is Empty."
      Write-Host-Custom-Red $error_message_for_input_arguments
      throw $error_message_for_input_arguments
    }

    # Install Azure Az PS Module
    Write-Host-Custom-Green "Installing Azure Az PS Module.....";
    Install-Module `
      -Name Az `
      -AllowClobber `
      -Scope CurrentUser

    # Query for Current Installed Azure Az PS module version 
    Write-Host-Custom-Green "Getting Current Installed Azure Az PS Module Version.....";
    Get-InstalledModule `
      -Name Az `
      -AllVersions | Select-Object Name, Version | Write-Host

    #NOTE: There is one manual step here which has to be done on the machine - Connecting to Azure portal with your credentails by running this command in PowerShell - Connect-AzAccount

    # Get All Azure Subscriptions
    Write-Host-Custom-Green "Getting all Azure Subscriptions.....";
    Get-AzSubscription

    # Set Azure Subscription to the Production Subscription
    Write-Host-Custom-Green "Setting current Azure Subscription to DevVM Blob Storage Subscription.....";
    Set-AzContext `
      -SubscriptionId "$global:azureSubscriptionIdEnv"

    # Get Azure Storage Account for DevVM blob storage
    Write-Host-Custom-Green "Getting Azure Storage Account for DevVM blob storage.....";
    $storageAccount = Get-AzStorageAccount `
      -ResourceGroupName $global:devVmAzureResourceGroupNameConstant `
      -Name $global:azureBlobStorageAccountNameConstant

    # Get Azure Storage Account Context for DevVM blob storage
    Write-Host-Custom-Green "Getting Azure Storage Account Context for DevVM blob storage.....";
    $storageAccountContext = $storageAccount.Context

    # Upload a file to blob storage
    Write-Host-Custom-Green "Uploading file to Azure DevVM blob storage.....";
    $blobName = "$($global:parentFolderName)/$($global:destinationFileName)"
    Set-AzStorageBlobContent `
      -File $global:sourceFileFullPath `
      -Container $global:azureBlobContainerNameConstant `
      -Blob $blobName `
      -Context $storageAccountContext `
      -Force
    Write-Host-Custom-Green "Uploaded file to Azure DevVM blob storage.....";
  }
  Catch [Exception] {
    Write-Host-Custom-Red "An error occured when uploading file to Azure DevVM blob storage [$($global:sourceFileFullPath)]"
    Write-Host-Custom-Red "-----> Exception: $($_.Exception.GetType().FullName)"
    Write-Host-Custom-Red "-----> Exception Message: $($_.Exception.Message)"
    throw
  }
}

Write-Host-Custom-Blue "Start Time: $(Get-Date)";

Upload-File-To-Azure-Blob-Storage

Write-Host-Custom-Blue "End Time: $(Get-Date)";
