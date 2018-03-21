resource_name :install_library_application

property :relativity_services_url, String
property :workspace_name, String
property :server_name, String
property :relativity_username, String
property :relativity_password, String
property :application_guid, String
property :powershell_functions_script_path, String

actions :start
default_action :start

action :start do

  powershell_script "Install_Library_Application" do
    code <<-EOS
    ##########################################################################
    # This script creates a library application.                             #
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
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\Relativity.Services.Interfaces.Private.dll"
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\Relativity.Services.ServiceProxy.dll"
    ##########################################################################
    
    ############################USER CONFIG SECTION###########################
    $relativityUrl = "#{relativity_services_url}"
    $workspaceName = "#{workspace_name}"
    $serverName = "#{server_name}"
    $relativityUsername = "#{relativity_username}"
    $relativityPassword = "#{relativity_password}"
    $applicationGuid = New-Object -TypeName System.Guid -ArgumentList "#{application_guid}"
    $maxRetry = 3
    ###########################################################################
    
    $relativityServicesUrl = New-Object -TypeName System.Uri -ArgumentList $relativityUrl

    # First make sure workspace with the same name exists
    [Boolean]$workspaceExists
    $retryCnt = 0
    $existingWorkspaceArtifactID = 0
    while ($retryCnt -lt $maxRetry -and $existingWorkspaceArtifactID -eq 0) {
      $rsapiClient = $NULL
    
      try {
        $rsapiClient = GetRsapiClient $relativityServicesUrl $relativityUsername $relativityPassword
        $existingWorkspaceArtifactID = QueryWorkspace $rsapiClient $workspaceName
        if ($existingWorkspaceArtifactID -gt 0) {
          $workspaceExists = $true
        }
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
    if ($workspaceExists -eq $FALEE) {
      Write-Host "Skipping install could Not Find workspace: $($workspaceName)"
    }
    else {
      #Install Application
      $retryCnt = 0
      $applicationInstalled = $FALSE
      while ($retryCnt -lt $maxRetry -and $applicationInstalled -eq $FALSE) {
        try {
          $applicationInstalled = InstallLibraryApplication $serverName $relativityUsername $relativityPassword $existingWorkspaceArtifactID $applicationGuid
        }
        Catch [Exception] {
          Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
        }
    
        $retryCnt++
      }
    
      if ($applicationInstalled -eq $TRUE) {
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
