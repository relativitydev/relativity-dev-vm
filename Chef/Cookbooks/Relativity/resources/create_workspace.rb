resource_name :create_workspace

property :workspace_name, String, name_property: true
property :relativity_services_url, String
property :relativity_username, String
property :relativity_password, String
property :powershell_functions_script_path, String

actions :start
default_action :start

action :start do

  powershell_script "create_workspace_#{workspace_name}" do
    code <<-EOS
    ##########################################################################
    # This script creates a workspaces.                                      #
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
    $newWorkspaceName = "#{workspace_name}"
    $relativityUrl = "#{relativity_services_url}"
    $relativityUsername = "#{relativity_username}"
    $relativityPassword = "#{relativity_password}"
    $maxRetry = 3
    ###########################################################################
    
    $relativityServicesUrl = New-Object -TypeName System.Uri -ArgumentList $relativityUrl
    
    # First check if workspace with the same name already exists
    [Boolean]$workspaceExists
	[Boolean]$successfulQuery
    $retryCnt = 0
    $existingWorkspaceArtifactID = 0
    while ($retryCnt -lt $maxRetry -and $successfulQuery -eq $FALSE) {
      $rsapiClient = $NULL
    
      try {
        $rsapiClient = GetRsapiClient $relativityServicesUrl $relativityUsername $relativityPassword
        $existingWorkspaceArtifactID = QueryWorkspace $rsapiClient $newWorkspaceName
        if ($existingWorkspaceArtifactID -gt 0) {
          $workspaceExists = $TRUE
        }
		$successfulQuery = $TRUE
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
    
    # Create workspace if the workspace not already exists
    if ($workspaceExists) {
      Write-Host "Skipping Workspace creation."
    }
    else {
      #Create Workspace
      $retryCnt = 0
      $newWorkspaceArtifactID = 0
      while ($retryCnt -lt $maxRetry -and $newWorkspaceArtifactID -eq 0) {
        $rsapiClient = $NULL
    
        try {
          $rsapiClient = GetRsapiClient $relativityServicesUrl $relativityUsername $relativityPassword
          $newWorkspaceArtifactID = CreateWorkspace $rsapiClient $newWorkspaceName
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
