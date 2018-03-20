function CreateJsonConfigFile([string] $fileName, [string] $destinationFileLocation, [string] $relativityUsername, [string] $relativityPassword, [string] $relativityLibraryFolder, [int] $numberOfDocuments, [bool] $importImagesWithDocuments, [bool] $importProductionImagesWithDocuments) {
  Write-Host "Writing config file"
  $destinationFileLocation = [System.IO.Path]::Combine($destinationFileLocation, $fileName)
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
  $contents | Out-File $destinationFileLocation 
  Write-Host "Config file written: $($destinationFileLocation)"
  $destinationFileLocation
}
    
function GetRsapiClient([string] $relativityServicesUrl, [string] $username, [string] $password) {
  Write-Host "Creating Rsapi Client"
  $retVal = $NULL

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
    $templateWorkspaceArtifactID = @($workspaceQueryResults.Results.Artifact.ArtifactID) | Sort-Object | Select-Object -Last 1
    $templateWorkspace = @($workspaceQueryResults.Results.Artifact) | Where-Object {$_.ArtifactID -eq $templateWorkspaceArtifactID}
    $templateWorkspace.Name = $workspaceName

    $workspaceCreateRequestresults = $rsapiClient.Repositories.Workspace.CreateAsync($templateWorkspace.ArtifactID, $templateWorkspace)
    if ($workspaceCreateRequestresults.Success) {
      $info = $rsapiClient.GetProcessState($rsapiClient.APIOptions, $workspaceCreateRequestresults.ProcessID)
      $iteration = 0
      while ($info.State -ne [kCura.Relativity.Client.ProcessStateValue]::Completed) {
        # Sleeping for 90 secs for the workspace upgrade to complete
        Start-Sleep -s 90
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

function EnableDataGrid([kCura.Relativity.Client.RSAPIClient] $rsapiClient, [int] $workspaceArtifactID) {
  Write-Host "Enabling DataGrid in Workspace: ($workspaceArtifactID)"

  $fieldName = [kCura.Relativity.Client.DTOs.WorkspaceFieldNames]::EnableDataGrid
  $dgField = New-Object -TypeName kCura.Relativity.Client.DTOs.FieldValue -ArgumentList $fieldName, $TRUE
  $fieldList = New-Object -TypeName System.Collections.Generic.List[kCura.Relativity.Client.DTOs.FieldValue]
  $fieldList.Add($dgField)

  $rsapiClient.APIOptions.WorkspaceID = -1
 
  $workspace = New-Object -TypeName kCura.Relativity.Client.DTOs.Workspace -ArgumentList $workspaceArtifactID -Property @{
    'Fields'    = $fieldList
  }

  $rsapiClient.Repositories.Workspace.UpdateSingle($workspace)
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
    while($info.State -eq [kCura.Relativity.Client.ProcessStateValue]::Running -and $iteration -lt 120){
      Start-Sleep -s 10
      $info = $rsapiClient.GetProcessState($rsapiClient.APIOptions, $info.ProcessID)
      $iteration++
    }
    if($info.State -eq [kCura.Relativity.Client.ProcessStateValue]::HandledException -or $info.State -eq [kCura.Relativity.Client.ProcessStateValue]::UnhandledException) {
      Write-Error "Application Install creation timed out"
      Return
    }else{
      $retVal = $TRUE
    }
  }
  else {
    Write-Error "Unable to install App $($installationRequest.Message)"
    Return
  }

  $retVal
}

function InstallLibraryApplication([string] $serverName, [string] $userName, [string] $password, [int] $workspaceArtifactID, [System.Guid] $applicationGuid) {
  Write-Host "Installing Library Application in Workspace $($workspaceArtifactID)"
  $retVal = $FALSE
  $proxy = $NULL
  $formattedUrl = [string]::Format("http://{0}/Relativity.Rest/Api", $serverName)
  $uri = New-Object -TypeName System.Uri -ArgumentList $formattedUrl
  $usernamePasswordCredentials = New-Object -TypeName Relativity.Services.ServiceProxy.UsernamePasswordCredentials -ArgumentList $userName, $password
  $serviceFactorySettings = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactorySettings -ArgumentList $uri, $uri, $usernamePasswordCredentials
  $serviceFactory = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactory -ArgumentList $serviceFactorySettings

  try {
    # The easiest way to call a generic method in powershell
    $proxy = $serviceFactory.GetType().GetMethod("CreateProxy").MakeGenericMethod([Relativity.Services.ApplicationInstallManager.IApplicationInstallManager]).Invoke($serviceFactory, $null)
    $retVal = $proxy.InitiateLibraryApplicationInstallAsync($workspaceArtifactID, $applicationGuid).Result;

    if ($result -eq $FALSE) {
        Write-Error "Error Installing Libarry Application"
    }
  }
  catch [Exception] {
    Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
  }
  finally {
    if ($proxy -ne $NULL){
        $proxy.Dispose()
    }
  }
  $retVal
}

function QueryInstanceSetting([string] $serverName, [string] $userName, [string] $password, [string] $section, [string] $name) {
  Write-Host "Querying Instance Setting"
  $retVal = $NULL
  $proxy = $NULL
  $formattedUrl = [string]::Format("http://{0}/Relativity.Rest/Api", $serverName)
  $uri = New-Object -TypeName System.Uri -ArgumentList $formattedUrl
  $usernamePasswordCredentials = New-Object -TypeName Relativity.Services.ServiceProxy.UsernamePasswordCredentials -ArgumentList $userName, $password
  $serviceFactorySettings = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactorySettings -ArgumentList $uri, $uri, $usernamePasswordCredentials
  $serviceFactory = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactory -ArgumentList $serviceFactorySettings

  try {
    $query = New-Object -TypeName Relativity.Services.Query -Property @{
        'Condition' = "'Section' == '$($section)' AND 'Name' == '$($name)'"
    }
    # The easiest way to call a generic method in powershell
    $proxy = $serviceFactory.GetType().GetMethod("CreateProxy").MakeGenericMethod([Relativity.Services.InstanceSetting.IInstanceSettingManager]).Invoke($serviceFactory, $null)
    $retVal = $proxy.QueryAsync($query).Result;

    if ($retVal.Success -eq $FALSE) {
        throw $retVal.Message
    }
  }
  catch [Exception] {
    Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
  }
  finally {
    if ($proxy -ne $NULL){
        $proxy.Dispose()
    }
  }
  $retVal
}

function CreateInstanceSetting([string] $serverName, [string] $userName, [string] $password, [string] $section, [string] $name, [Relativity.Services.InstanceSetting.ValueType] $valueType, [string] $value) {
  Write-Host "Creating Instance Setting"
  $retVal = $NULL
  $proxy = $NULL
  $formattedUrl = [string]::Format("http://{0}/Relativity.Rest/Api", $serverName)
  $uri = New-Object -TypeName System.Uri -ArgumentList $formattedUrl
  $usernamePasswordCredentials = New-Object -TypeName Relativity.Services.ServiceProxy.UsernamePasswordCredentials -ArgumentList $userName, $password
  $serviceFactorySettings = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactorySettings -ArgumentList $uri, $uri, $usernamePasswordCredentials
  $serviceFactory = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactory -ArgumentList $serviceFactorySettings

  try {
    $instanceSetting = New-Object -TypeName Relativity.Services.InstanceSetting.InstanceSetting -Property @{
        'Section' = $section
        'Name' = $name
        'Value' = $value
        'ValueType' = $valueType
    }
    # The easiest way to call a generic method in powershell
    $proxy = $serviceFactory.GetType().GetMethod("CreateProxy").MakeGenericMethod([Relativity.Services.InstanceSetting.IInstanceSettingManager]).Invoke($serviceFactory, $null)
    $retVal = $proxy.CreateSingleAsync($instanceSetting).Result;
  }
  catch [Exception] {
    Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
  }
  finally {
    if ($proxy -ne $NULL){
        $proxy.Dispose()
    }
  }
  $retVal
}

function UpdateInstanceSettingValue([string] $serverName, [string] $userName, [string] $password, [string] $section, [string] $name, [string] $value) {
  Write-Host "Updating Instance Setting"
  $retVal = $NULL
  $proxy = $NULL
  $instanceSettingResults = QueryInstanceSetting $serverName $userName $password $section $name

  if($instanceSettingResults.Success -eq $FALSE -or $instanceSettingResults.Results.Count -lt 1){
    Write-Host "Unable to find Instance Setting"
  }else{
    $instanceSetting = $instanceSettingResults.Results[0].Artifact
    $instanceSetting.Value = "True"

    $formattedUrl = [string]::Format("http://{0}/Relativity.Rest/Api", $serverName)
    $uri = New-Object -TypeName System.Uri -ArgumentList $formattedUrl
    $usernamePasswordCredentials = New-Object -TypeName Relativity.Services.ServiceProxy.UsernamePasswordCredentials -ArgumentList $userName, $password
    $serviceFactorySettings = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactorySettings -ArgumentList $uri, $uri, $usernamePasswordCredentials
    $serviceFactory = New-Object -TypeName Relativity.Services.ServiceProxy.ServiceFactory -ArgumentList $serviceFactorySettings

    try {
        # The easiest way to call a generic method in powershell
        $proxy = $serviceFactory.GetType().GetMethod("CreateProxy").MakeGenericMethod([Relativity.Services.InstanceSetting.IInstanceSettingManager]).Invoke($serviceFactory, $null)
        $proxy.UpdateSingleAsync($instanceSetting).Wait();
    }
    catch [Exception] {
      Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
    }
    finally {
      if ($proxy -ne $NULL){
          $proxy.Dispose()
      }
    }
  }
}