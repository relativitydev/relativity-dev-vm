resource_name :start_service

property :service_name, String, name_property: true
property :item_type, String
property :serviceBusRelated, String
property :location, String

actions :start
default_action :start

action :start do

  powershell_script "extract_#{name}" do
    ignore_failure true
    code <<-EOS
    function DevVMServiceLauncher([string] $serviceOrProcessName, [string] $itemType, [bool] $serviceBusRelated, [string] $processLocation){
      $currentWait = 0
      $maxWait = 600
      $sleepSeconds = 5
      $serviceStatus = ""
      $process = $null

      # Service
      if($itemType -eq "Service" -and (Get-Service $serviceOrProcessName -ErrorAction SilentlyContinue)){
        $serviceStatus = (Get-Service -Name $serviceOrProcessName).Status
        if ($serviceStatus -ne [System.ServiceProcess.ServiceControllerStatus]::Running){

          # Service is already starting, wait until it finished
          if ($serviceStatus -eq [System.ServiceProcess.ServiceControllerStatus]::StartPending){
            while($serviceStatus -eq [System.ServiceProcess.ServiceControllerStatus]::StartPending -and  $currentWait -lt $maxWait){
              Start-Sleep -s $sleepSeconds
              $currentWait += $sleepSeconds
              $serviceStatus = (Get-Service -Name $serviceOrProcessName).Status
            }
          }
          # Start Service and wait until it is running
          else{
            if($serviceBusRelated){
              Start-SBFarm
            }else{
              Start-Service $serviceOrProcessName
            }
            while($serviceStatus -ne [System.ServiceProcess.ServiceControllerStatus]::Running -and $currentWait -lt $maxWait){
              Start-Sleep -s $sleepSeconds
              $currentWait += $sleepSeconds
              $serviceStatus = (Get-Service -Name $serviceOrProcessName).Status
            }
          }
        }
      }
      # Process
      elseIf($itemType -eq "Process" -and (Get-Process $serviceOrProcessName -ErrorAction SilentlyContinue)){
        $process = Get-Process -Name $serviceOrProcessName -ErrorAction SilentlyContinue
        if ($process -eq $null){
          Start-Process -FilePath $processLocation
          while($process -eq $null -and  $currentWait -lt $maxWait){
            Start-Sleep -s $sleepSeconds
            $currentWait += $sleepSeconds
            $process = Get-Process -Name $serviceOrProcessName -ErrorAction SilentlyContinue
          }
        }
      }

      if ( $process -ne $null -or $serviceStatus -eq [System.ServiceProcess.ServiceControllerStatus]::Running){
        exit 0 #Success
      }else{
        exit 1 #Failure
      }
    }

    DevVMServiceLauncher "#{service_name}" "#{item_type}" #{serviceBusRelated} "#{location}"

    EOS
  end
end

action_class do
  def whyrun_supported?
    true
  end
end
