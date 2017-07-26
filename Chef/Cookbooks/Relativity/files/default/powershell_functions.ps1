function CreateJsonConfigFile([string] $fileName, [string] $relativityUsername, [string] $relativityPassword, [string] $relativityLibraryFolder, [int] $numberOfDocuments, [bool] $importImagesWithDocuments, [bool] $importProductionImagesWithDocuments) {
  Write-Host "Writing config file"
  $fileLocation = [System.IO.Path]::Combine($env:TEMP, [System.Guid]::NewGuid())
  $contents = 
  $(
    " {
                ""RelativityUsername"": ""$relativityUsername"",
                ""RelativityPassword"": ""$relativityPassword"",
                ""RelativityLibraryFolder"": ""$relativityLibraryFolder"",
                ""NumberOfDocuments"": $numberOfDocuments,
                ""ImportImagesWithDocuments"": $($importImagesWithDocuments.ToString().ToLower()),
                ""ImportProductionImagesWithDocuments"": $($importProductionImagesWithDocuments.ToString().ToLower())
            }")
  $contents | Out-File $fileLocation 
  Write-Host "Config file written: $($fileLocation)"
  $fileLocation
}
    
function GetRsapiClient([string] $relativityServicesUrl, [string] $username, [string] $password) {
  Write-Host "Creating Rsapi Client"
  $retVal = $NULL

  $app = New-Object -TypeName kCura.Relativity.Client.AppInstallRequest -Property @{
    'FullFilePath' = $rapFileLocation
  }

  $credentials = New-Object -TypeName kCura.Relativity.Client.UsernamePasswordCredentials -ArgumentList $username, $password
  $clientSettings = New-Object -TypeName kCura.Relativity.Client.RSAPIClientSettings
  $retVal = New-Object -TypeName kCura.Relativity.Client.RSAPIClient -ArgumentList $relativityServicesUrl, $credentials, $clientSettings
  Write-Host "New Rsapi Client Created"

  $retVal
}

    
function PushResource([kCura.Relativity.Client.RSAPIClient] $rsapiClient, [string] $fileName, [string] $fullfilePath, [System.Guid] $appGuid) {
  Write-Host "Uploading Resource $($fullfilePath)"
  $retVal = $FALSE

  $resourceFile = New-Object -TypeName kCura.Relativity.Client.ResourceFileRequest -Property @{
    'AppGuid'      = $appGuid
    'FileName'     = $fileName 
    'FullFilePath' = $fullfilePath
  }
  $resourceList = New-Object -TypeName "System.Collections.Generic.List[kCura.Relativity.Client.ResourceFileRequest]"
  $resourceList.Add($resourceFile)

  $rsapiClient.APIOptions.WorkspaceID = -1
  $results = $rsapiClient.PushResourceFiles($rsapiClient.APIOptions, $resourceList)
  if ($results.Success -eq $FALSE) {
    Write-Error "Unable to push resource file"
  }
  else {
    $retVal = $TRUE
    Write-Host "Successfully uploaded $($fileName)"
  }

  $retVal
}

function CreateWorkspace([kCura.Relativity.Client.RSAPIClient] $rsapiClient, [string] $workspaceName) {
  Write-Host "Creating Workspace $($workspaceName)"
  $retVal = 0

  $workspaceQuery = New-Object -TypeName kCura.Relativity.Client.DTOs.Query[kCura.Relativity.Client.DTOs.Workspace] -Property @{
    'Fields' = [kCura.Relativity.Client.DTOs.FieldValue]::NoFields
  }

  $rsapiClient.APIOptions.WorkspaceID = -1
  $workspaceQueryResults = $rsapiClient.Repositories.Workspace.Query($workspaceQuery)

  if ($workspaceQueryResults.Success -and $workspaceQueryResults.TotalCount -gt 0) {
    #smallest workspace artifactID will represent the template workspace
    $templateWorkspaceArtifactID = @($workspaceQueryResults.Results.Artifact.ArtifactID) | Sort-Object | Select-Object -First 1
    $templateWorkspace = @($workspaceQueryResults.Results.Artifact) | Where-Object {$_.ArtifactID -eq $templateWorkspaceArtifactID}
    $templateWorkspace.Name = $workspaceName

    $workspaceCreateRequestresults = $rsapiClient.Repositories.Workspace.CreateAsync($templateWorkspace.ArtifactID, $templateWorkspace)
    if ($workspaceCreateRequestresults.Success) {
      $info = $rsapiClient.GetProcessState($rsapiClient.APIOptions, $workspaceCreateRequestresults.ProcessID)
      $iteration = 0
      while ($info.State -ne [kCura.Relativity.Client.ProcessStateValue]::Completed) {
        # Sleeping for 30 secs for the workspace upgrade to complete
        Start-Sleep -s 30
        $info = $rsapiClient.GetProcessState($rsapiClient.APIOptions, $workspaceCreateRequestresults.ProcessID)

        if ($iteration -gt 6) {
          Write-Error "Workspace creation timed out"
          Return
        }
        $iteration++;
      }

      if ($info.OperationArtifactIDs.ToArray().Count -gt 0) {
        $retVal = $info.OperationArtifactIDs.ToArray()[0]
        Write-Host "Workspace $($workspaceName) successfully created!"
      }
      else {
        Write-Error "Unable to create workspace"
        Return
      }
    }
    else {
      Write-Error "Unable to create workspace $($workspaceCreateRequestresults.Message)"
      Return
    }
  }
  else {
    Write-Error "Unable to find template workspaces"
    Return
  }

  $retVal
}

function QueryWorkspace([kCura.Relativity.Client.RSAPIClient] $rsapiClient, [string] $workspaceName) {
  Write-Host "Querying workspace($workspaceName)"
  $existingWorkspaceArtifactID = 0
 
  $nameCondition = New-Object -TypeName kCura.Relativity.Client.TextCondition([kCura.Relativity.Client.DTOs.WorkspaceFieldNames]::Name, [kCura.Relativity.Client.TextConditionEnum]::EqualTo, $workspaceName)
  $workspaceQuery = New-Object -TypeName kCura.Relativity.Client.DTOs.Query[kCura.Relativity.Client.DTOs.Workspace] -Property @{
    'Fields'    = [kCura.Relativity.Client.DTOs.FieldValue]::NoFields
    'Condition' = $nameCondition
  }

  $rsapiClient.APIOptions.WorkspaceID = -1
  $workspaceQueryResults = $rsapiClient.Repositories.Workspace.Query($workspaceQuery)

  if ($workspaceQueryResults.Success) {
    if ($workspaceQueryResults.TotalCount -eq 0) {
      Write-Host "Workspace($workspaceName) does not exits."
    }
    if ($workspaceQueryResults.TotalCount -gt 0) {
      Write-Host "Workspace($workspaceName) exists."
      if ($workspaceQueryResults.TotalCount -eq 1) {
        $existingWorkspaceArtifactID = @($workspaceQueryResults.Results.Artifact.ArtifactID) | Sort-Object | Select-Object -First 1
        Write-Host "ExistingWorkspaceArtifactID = $($existingWorkspaceArtifactID)"      
      }
      if ($workspaceQueryResults.TotalCount -gt 1) {
        Write-Error "Multiple workspaces exists with the same name."      
      }
    }
  }
  else {
    Write-Error "An error occured when querying for the workspace($workspaceName)"
    Return
  }

  $existingWorkspaceArtifactID
}
    
function InstallApplication([kCura.Relativity.Client.RSAPIClient] $rsapiClient, [int] $workspaceArtifactID, [string] $rapFileLocation) {
  Write-Host "Installing Application in Workspace $($workspaceArtifactID)"
  $retVal = $FALSE

  $app = New-Object -TypeName kCura.Relativity.Client.AppInstallRequest -Property @{
    'FullFilePath' = $rapFileLocation
  }

  $rsapiClient.APIOptions.WorkspaceID = $workspaceArtifactID

  $installationRequest = $rsapiClient.InstallApplication($rsapiClient.APIOptions, $app)
  if ($installationRequest.Success) {
    $info = $rsapiClient.GetProcessState($rsapiClient.APIOptions, $installationRequest.ProcessID)
    $iteration = 0
    while ($info.State -eq [kCura.Relativity.Client.ProcessStateValue]::Running) {
      # Sleeping for 30 secs for the application installation to complete
      Start-Sleep -s 30
      $info = $rsapiClient.GetProcessState($rsapiClient.APIOptions, $info.ProcessID)

      if ($iteration -gt 6) {
        Write-Error "Application Install creation timed out"
        Return
      }
      $iteration++
    }

    if ($info.Success -and $info.State -ne [kCura.Relativity.Client.ProcessStateValue]::HandledException -and $info.State -ne [kCura.Relativity.Client.ProcessStateValue]::UnhandledException -and $info.State -ne [kCura.Relativity.Client.ProcessStateValue]::CompletedWithError) {
      $retVal = $TRUE
      Write-Host "Application $($rapFileLocation) successfully installed in Workspace: $($workspaceArtifactID)"
    }
    else {
      Write-Error "Unable to install Application"
      Return
    }
  }
  else {
    Write-Error "Unable to install App $($installationRequest.Message)"
    Return
  }

  $retVal
}