Clear-Host

[string] $salesforceUsername = $args[0]
[string] $salesforcePassword = $args[1]
[string] $relativityVersion = $args[2]
[Boolean] $sendSlackMessage =  $args[3]
[string] $relativityVersionReleaseName = ''

$global:salesforceSessionObject = $null

class Environment {
  [string]$Name
  [string]$Server

  Environment($name, $server) {
    $this.Name = $name
    $this.Server = $server
  }
}

[Environment[]] $environments = @(
  [Environment]::new('Production','api.solutionsnapshot.relativity.com'),
  [Environment]::new('Staging','solutionsnapshotstagingapi.azurewebsites.net'),
  [Environment]::new('Develop','solutionsnapshotdevapi.azurewebsites.net')
)

class Application {
  [string]$Name
  [string]$Guid
  [string]$Version

  Application($name, $guid, $version) {
    $this.Name = $name
    $this.Guid = $guid
    $this.Version = $version
  }
}

[Application[]] $applications = @(
  [Application]::new('Propogate Coding Post-Import', 'C0C7A2BE-12C9-49C0-BF3F-44EB85CFF3EA', '15.1.0.0; 15.1.0.12'),
  [Application]::new('Collect Folder Path Data', '4EFA7758-06B9-4A46-A936-9E035356748E', '7.0.0.0'),
  [Application]::new('Remove Documents from Batch Sets', '02F4FD31-BB51-4F27-956D-20E4D6A9DB59', '3.6.0.2'),
  [Application]::new('File Validation Agent', '912EE010-B25E-4B11-B1F5-8D42D03F33C6', '6.10.0.0'),
  [Application]::new('Copy Redactions Across Markup Sets', '460FD2CC-8BB6-465D-B2DA-720E80294FA4', '1.2.0.0'),
  [Application]::new('Copy Redactions Across Workspaces', '9748387D-3BAA-4A33-BD39-83AF02FFBEB5', '4.4.0.1'),
  [Application]::new('Disable Inactive Users', 'F93FE68D-C732-4CFF-ABDA-0124248D2239', '8.0.0.1'),
  [Application]::new('Populate Parent ID and Child ID', 'B2E09BD0-438D-4C66-B247-0891528BF4A3', '8.0.0.1'),
  [Application]::new('Production Gap and Overlap Check', '6FB47352-7E1C-4A3E-8C8A-7FB10ABDC805', '3.2.0.0'),
  [Application]::new('Reproduce Redaction To Document Universe', '1724DAA9-396C-4DFA-9685-9C7104BA2098', '2.3.0.0'),
  [Application]::new('User Import Application', '08AF6BFE-6B4E-445D-AFB1-16BFBF91B7A4', '19.1.0.41'),
  [Application]::new('Auto Increment Field on Object', '41DE3DEA-F760-4A1D-84D1-392484A81B63', '2.1.0.1'),
  [Application]::new('Document Utilities', '6894DF96-B204-4157-9318-4073D8A7476D', '10.1.7.8'),
  [Application]::new('Reviewer Productivity', '2FDDD2D6-53E3-4888-BD7E-EC265E0C5F7A', '9.0.0.2'),
  [Application]::new('Track Document Field Edits By Reviewer', 'DC31F042-2653-4801-88ED-13CDC10A8A0C', '5.7.0.1'),
  [Application]::new('Data Field Parsing', 'E41DE486-8775-4A38-A4C0-CEFC382E7CF8', '0.0.0.1'),
  [Application]::new('Native Time Zone Offset with DST', 'C3B47CCC-4469-4FEF-8080-5BCF78BB81DC', '6.1.1.1'),
  [Application]::new('Change Redaction Type', '5EA43B8D-2B93-4944-A06E-1D86C8A74665', '4.0.0.1'),
  [Application]::new('Normalize Redactions Across Relational Groups', 'E8E1CAB6-47C5-4535-9D66-EB8AD69ACAE0', '0.0.0.2'),
  [Application]::new('Collect Saved Search Data Sizes', '9B50933A-7C1E-4A06-9859-1C2CC62A4250', '0.0.0.1'),
  [Application]::new('Environment Level User Login and Workspace Admin', 'E327D1D4-3591-4BFA-86BC-24A3B4A2F666', '2.5.0.1'),
  [Application]::new('Login History By User Report', 'D5A67A08-B7B9-4E54-9C24-B91254AA1F2B', '5.0.0.0'),
  [Application]::new('Search Term Counts', '751070C2-5C96-4F72-99D0-626FC96B354D', '0.0.0.1'),
  [Application]::new('User Counts Per Workspace', '45A67CA3-DE44-4411-8C51-8DDE3B7C22BB', '1.0.0.1'),
  [Application]::new('User Workspace Access and Last Login', '9B52B19E-6ADB-4020-8330-B44219BFEA28', '1.0.0.1'),
  [Application]::new('Solution Snapshot', 'D51C3E3E-EBF4-402A-B41E-35C4018C8396', '4.0.0.4')
)

function Write-Empty-Message() {
  Write-Host ""
}

function Write-Method-Call-Message($message) {
  Write-Empty-Message
  Write-Host -ForegroundColor Cyan $message
}

function Write-Message($message) {
  Write-Empty-Message
  Write-Host -ForegroundColor Green $message
}

function Write-Error-Message($message) {
  Write-Empty-Message
  Write-Host -ForegroundColor Red $message
}

function GetSessionId() {
  try {
    [Reflection.Assembly]::LoadFile("$PSScriptRoot\SolutionSnapshotSalesforceLoginHelper.dll")
    $salesforceSessionHelper = New-Object SolutionSnapshotSalesforceLoginHelper.SalesforceSessionHelper
	  
    Write-Method-Call-Message "Calling method to get SalesforceSessionInfo"
    $salesforcesessioninfo = $salesforceSessionHelper.GetSalesforceSessionInfo($salesforceUsername, $salesforcePassword)
    $global:salesforceSessionObject = new-object psobject
    $global:salesforceSessionObject | add-member noteproperty salesforceuserid $salesforcesessioninfo.salesforceuserid
    $global:salesforceSessionObject | add-member noteproperty sessionid $salesforcesessioninfo.sessionid
    $global:salesforceSessionObject | add-member noteproperty serverurl $salesforcesessioninfo.serverurl
  }
  catch {
    Write-Error-Message "An error occured when retrieving SalesforceSessionInfo."    
    Write-Error-Message "Error Message: ($_)"
  }
}

function CreateRelativityVersionAsync([Environment] $environment) {
  try {
    Write-Method-Call-Message "Calling CreateRelativityVersionAsync API"
    $request = new-object psobject
    $request | add-member noteproperty SalesforceSessionInfo $global:salesforceSessionObject
    $request | add-member noteproperty Version $relativityVersion
    $request | add-member noteproperty ReleaseName $relativityVersionReleaseName
    
    $requestJson = $request | ConvertTo-Json
    Write-Message "Request Json: $($requestJson)"

    $createRelativityVersionAsyncUrl = "https://$($environment.Server)/api/external/CreateRelativityVersionAsync"
    $response = invoke-restmethod -uri $createRelativityVersionAsyncUrl -method post -body $requestjson -contenttype 'application/json' -headers @{"x-csrf-header"="-"}
    $responsejson = $response | convertto-json
    Write-Message "Created Relativity Version in $($environment.Name)"

    # Send Slack Success Message
    Send-Slack-Success-Message $relativityVersion $environment.Name

    # Update Advice Hub Solutions with Compatibility for Newest Relativity Version
    UpdateNewestAdviceHubApplicationVersions $environment
  }
  catch {
	  if($_.ToString().Contains('Relativity Version ' + $relativityVersion + ' already exists')){
      Write-Error-Message "Relativity Version already exists"

      # Send Slack Skip Message
	  	Send-Slack-Skip-Message $relativityVersion $environment.Name
	  }
	  else {
	  	Write-Error-Message "An error occurred when calling CreateRelativityVersionAsyncUrl API."
	  	Write-Error-Message "Error Message: ($_)"
	  	# Dig into the exception to get the Response details. Note that value__ is not a typo.
	  	Write-Error-Message "Http Status Code: $($_.Exception.Response.StatusCode.value__)"
	  	Write-Error-Message "Http Status Description: $($_.Exception.Response.StatusDescription)"
	  	$responseJson = $_.Exception.Response | ConvertTo-Json
      Write-Error-Message "Response Json: $($responseJson)"
      
      # Send Slack Failure Message
	  	Send-Slack-Failure-Message $relativityVersion $environment.Name
	  }
  }
}

function UpdateNewestAdviceHubApplicationVersions([Environment] $environment) {
  try {
    Write-Method-Call-Message "Updating Newest Advice Hub Application Versions"

    foreach ($application in $applications) {
      if ($application.Version.Contains(";")){
        $versions = $application.Version -split ";"
        foreach ($version in $versions) {
          $version = $version.trim()
          Write-Message "Updating Application: $($application.Name), Version: $($version)"
          UpdateApplicationVersionAsync $application.Guid $version $environment
        }
      }
      else {
        $application.Version = $application.Version.trim()
        Write-Message "Updating Application: $($application.Name), Version: $($application.Version)"
        UpdateApplicationVersionAsync $application.Guid $application.Version $environment
      }
    }

    # Send Slack Message that Updating Apps Finished
    Send-Slack-Message-Update-Finished $relativityVersion $environment.Name
  }
  catch {
    Write-Error-Message "An error occurred when trying to Update All Advice Hub Solutions."
    Write-Error-Message "Error Message: ($_)"

    # Send Slack Message that Updating Apps Failed
    Send-Slack-Message-Update-Failed $relativityVersion $environment.Name
  }
}

function UpdateApplicationVersionAsync([string] $applicationGuid, [string] $applicationVersion, [Environment] $environment) {
  try {
    Write-Method-Call-Message "Calling UpdateApplicationVersionAsync API"
    $request = new-object psobject
    $request | add-member noteproperty SalesforceSessionInfo $global:salesforceSessionObject
    $request | add-member noteproperty ApplicationGuid $applicationGuid
    $request | add-member noteproperty Version $applicationVersion
    $request | add-member noteproperty ReleaseNotes 'N/A'
    $request | add-member noteproperty AppendRelativityVersion 'true'
       
    $relativityVersionArray = @()
    $relativityVersionObject1 = new-object psobject
    $relativityVersionObject1 | add-member noteproperty Version $relativityVersion
    $relativityVersionArray += $relativityVersionObject1

    $request | add-member noteproperty RelativityVersions $relativityVersionArray
    $requestJson = $request | ConvertTo-Json
    Write-Message "Request Json: $($requestJson)"

    $updateApplicationVersionAsyncUrl = "https://$($environment.Server)/api/external/UpdateApplicationVersionAsync"
    $response = Invoke-RestMethod -Uri $updateApplicationVersionAsyncUrl -Method Post -Body $requestJson -ContentType 'application/json' -Headers @{"x-csrf-header"="-"}
    $responseJson = $response | ConvertTo-Json
    Write-Message "Updated Application Version"
  }
  catch {
    Write-Error-Message "An error occured when calling UpdateApplicationVersionAsync API."
    Write-Error-Message "Error Message: ($_)"
    # Dig into the exception to get the Response details. Note that value__ is not a typo.
    Write-Error-Message "Http Status Code: $($_.Exception.Response.StatusCode.value__)"
    Write-Error-Message "Http Status Description: $($_.Exception.Response.StatusDescription)"
    $responseJson = $_.Exception.Response | ConvertTo-Json
    Write-Error-Message "Response Json: $($responseJson)"
  }  
}

function Send-Slack-Message-Update-Finished([string] $relativityVersionToUpdate, [string] $environmentName) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($sendSlackMessage -eq $true) {
    Write-Method-Call-Message "Sending Slack Message that Updating Advice Hub Apps finished"
    $BodyJSON = @{
       "text" = "Successfully updated all Advice Hub apps with compatibility with ($($relativityVersionToCreate)) in the Solution Snapshot Database in the $($environmentName) Environment"
    } | ConvertTo-Json
    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json
    Write-Message "Sent Slack Message that Updating Advice Hub Apps finished"
  }
}

function Send-Slack-Message-Update-Failed([string] $relativityVersionToUpdate, [string] $environmentName) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($sendSlackMessage -eq $true) {
    Write-Method-Call-Message "Sending Slack Message that Updating Advice Hub Apps failed to finish"
    $BodyJSON = @{
       "text" = "Failed to update all Advice Hub apps with compatibility with ($($relativityVersionToCreate)) in the Solution Snapshot Database in the $($environmentName) Environment"
    } | ConvertTo-Json
    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json
    Write-Message "Sent Slack Message that Updating Advice Hub Apps failed to finished"
  }
}

function Send-Slack-Success-Message([string] $relativityVersionToCreate, [string] $environmentName) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($sendSlackMessage -eq $true) {
    Write-Method-Call-Message "Sending Slack Success Message"
    $BodyJSON = @{
       "text" = "Added ($($relativityVersionToCreate)) to Solution Snapshot Database in the $($environmentName) Environment"
    } | ConvertTo-Json
    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json
    Write-Message "Sent Slack Success Message" 
  }
}

function Send-Slack-Skip-Message([string] $relativityVersionToCreate, [string] $environmentName) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($sendSlackMessage -eq $true) {
    Write-Method-Call-Message "Sending Slack Skip Message"
    $BodyJSON = @{
      "text" = "Skipped adding ($($relativityVersionToCreate)) to Solution Snapshot Database in the $($environmentName) Environment, since it already exists"
    } | ConvertTo-Json
    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json
    Write-Message "Sent Slack Skip Message" 
  }
}

function Send-Slack-Failure-Message([string] $relativityVersionToCreate, [string] $environmentName) {
  [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
  if ($sendSlackMessage -eq $true) {
    Write-Method-Call-Message "Sending Slack Failure Message"
    $BodyJSON = @{
      "text" = "Failed to add ($($relativityVersionToCreate)) to Solution Snapshot Database in the $($environmentName) Environment"
    } | ConvertTo-Json
    Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json
    Write-Message "Sent Slack Failure Message"
  }
}

GetSessionId

foreach($environment in $environments){
  CreateRelativityVersionAsync $environment
}