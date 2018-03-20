resource_name :enable_datagrid_in_workspace

property :workspace_name, String
property :relativity_services_url, String
property :relativity_username, String
property :relativity_password, String
property :powershell_functions_script_path, String

actions :start
default_action :start

action :start do

  powershell_script "enable_datagrid_in_#{workspace_name}" do
    code <<-EOS
    ##########################################################################
    # This script enables datagrid in a workspace                            #
    #                                                                        #
    # How to use:                                                            #
    # 1. Point the include section to the directory of your Relativity       #
    #    Client Dlls                                                         #
    #                                                                        #
    # 2. Update the Config section dlls                                      #
    #                                                                        #
    # 3. Execute the script                                                  #
    #                                                                        #
    ##########################################################################
    
    #############################INCLUDE SECTION##############################
    Import-Module "#{powershell_functions_script_path}" -force
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\kCura.Relativity.Client.dll"
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\Relativity.API.dll"
    ##########################################################################
    
    ############################USER CONFIG SECTION###########################
    $workspaceName = "#{workspace_name}"
    $relativityUrl = "#{relativity_services_url}"
    $relativityUsername = "#{relativity_username}"
    $relativityPassword = "#{relativity_password}"
    $maxRetry = 3
    ###########################################################################
    
    $relativityServicesUrl = New-Object -TypeName System.Uri -ArgumentList $relativityUrl
    
    # First make sure workspace with the same name exists
    [Boolean]$workspaceExists
    [Boolean]$operationCompleted = $FALSE
    $retryCnt = 0
    $existingWorkspaceArtifactID = 0
    while ($retryCnt -lt $maxRetry -and $operationCompleted -eq $FALSE) {
        $rsapiClient = $NULL
    
        try {
        $rsapiClient = GetRsapiClient $relativityServicesUrl $relativityUsername $relativityPassword
        $existingWorkspaceArtifactID = QueryWorkspace $rsapiClient $workspaceName
        if ($existingWorkspaceArtifactID -gt 0) {
            $workspaceExists = $true
        }
        $operationCompleted = $TRUE
        }
        Catch [Exception] {
        Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
        }
        finally {
        if ($rsapiClient -ne $NULL) {
            $rsapiClient.Dispose()
        }
        }
    
        $retryCnt++
    }
    
    # Enable Data Grid only if the workspace is found
    if ($workspaceExists -eq $FALEE) {
        Write-Host "Not enabling datagrid because workspacew was not found: $($workspaceName)"
    }
    else {
      $retryCnt = 0
      [Boolean]$operationCompleted = $FALSE
      while ($retryCnt -lt $maxRetry -and $operationCompleted -eq $FALSE) {
        $rsapiClient = $NULL
    
        try {
          $rsapiClient = GetRsapiClient $relativityServicesUrl $relativityUsername $relativityPassword
          EnableDataGrid $rsapiClient $existingWorkspaceArtifactID
          $operationCompleted = $TRUE
        }
        Catch [Exception] {
          Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
        }
        finally {
          if ($rsapiClient -ne $NULL) {
            $rsapiClient.Dispose()
          }
        }
    
        $retryCnt++
      }
    
      if (($workspaceExists -eq $true) -or ($newWorkspaceArtifactID -gt 0)) {
        #
        #SUCCESS
        exit 0
      }
      else {
        #
        #FAILURE
        exit 1
      }
    }
    EOS
  end
end

action_class do
  def whyrun_supported?
    true
  end
end
