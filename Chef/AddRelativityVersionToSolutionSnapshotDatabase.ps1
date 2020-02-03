Clear-Host

[string] $salesforceUsername = $args[0]
[string] $salesforcePassword = $args[1]
[string] $relativityVersion = $args[2]
[string] $relativityVersionReleaseName = ''

[string] $server = 'solutionsnapshotapi.azurewebsites.net'

[string] $CreateRelativityVersionAsyncUrl = "https://$($server)/api/external/CreateRelativityVersionAsync"
[string] $ReadRelativityVersionAsyncUrl = "https://$($server)/api/external/ReadRelativityVersionAsync"

$global:salesforceSessionObject = $null

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

function CreateRelativityVersionAsync() {
  try {
    Write-Method-Call-Message "Calling CreateRelativityVersionAsync API"
    $request = new-object psobject
    $request | add-member noteproperty SalesforceSessionInfo $global:salesforceSessionObject
    $request | add-member noteproperty Version $relativityVersion
    $request | add-member noteproperty ReleaseName $relativityVersionReleaseName
    
    $requestJson = $request | ConvertTo-Json
    Write-Message "Request Json: $($requestJson)"

    $response = invoke-restmethod -uri $createrelativityversionasyncurl -method post -body $requestjson -contenttype 'application/json' -headers @{"x-csrf-header"="-"}
    $responsejson = $response | convertto-json
    Write-Message "Created Relativity Version"
	Send-Slack-Success-Message $relativityVersion
  }
  catch {
	if($_.ToString().Contains('Relativity Version ' + $relativityVersion + ' already exists')){
		Write-Error-Message "Relativity Version already exists"
		Send-Slack-Skip-Message $relativityVersion
	}
	else {
		Write-Error-Message "An error occurred when calling CreateRelativityVersionAsyncUrl API."
		Write-Error-Message "Error Message: ($_)"
		# Dig into the exception to get the Response details. Note that value__ is not a typo.
		Write-Error-Message "Http Status Code: $($_.Exception.Response.StatusCode.value__)"
		Write-Error-Message "Http Status Description: $($_.Exception.Response.StatusDescription)"
		$responseJson = $_.Exception.Response | ConvertTo-Json
		Write-Error-Message "Response Json: $($responseJson)"
		Send-Slack-Failure-Message $relativityVersion
	}
  }
}

function Send-Slack-Success-Message([string] $relativityVersionToCreate) {
   Write-Heading-Message-To-Screen "Sending Slack Success Message"
   $BodyJSON = @{
     "text" = "Added ($($relativityVersionToCreate)) to Solution Snapshot Database"
   } | ConvertTo-Json

   Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json

   Write-Message-To-Screen "Sent Slack Success Message"
}

function Send-Slack-Skip-Message([string] $relativityVersionToCreate) {
   Write-Heading-Message-To-Screen "Sending Slack Skip Message"
   $BodyJSON = @{
     "text" = "Skipped adding ($($relativityVersionToCreate)) to Solution Snapshot Database, since it already exists."
   } | ConvertTo-Json

   Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json

   Write-Message-To-Screen "Sent Slack Skip Message"
}

function Send-Slack-Failure-Message([string] $relativityVersionToCreate) {
   Write-Heading-Message-To-Screen "Sending Slack Failure Message"
   $BodyJSON = @{
     "text" = "Failed to add ($($relativityVersionToCreate)) to Solution Snapshot Database"
   } | ConvertTo-Json

   Invoke-WebRequest -Method Post -Body "$BodyJSON" -Uri $Env:slack_devex_announcements_group_key -ContentType application/json

   Write-Message-To-Screen "Sent Slack Failure Message"
}

GetSessionId
CreateRelativityVersionAsync