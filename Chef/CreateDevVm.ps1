Clear-Host

[System.Int32]$global:maxRetry = 1
[System.Int32]$global:count = 1
[System.Int32]$global:count = 1
[Boolean] $global:exportVm = $false
[string] $global:scriptResultFileName = "result_file.txt"

function Write-Message-To-Screen ([string] $writeMessage) {
  Write-Host  "-----> [$(Get-Date -Format g)] $($writeMessage)" -ForegroundColor Magenta
}

function Delete-File-If-It-Exists ([string] $fileName) {
  If (Test-Path $fileName) {
    Remove-Item -path $fileName -force
    Write-Message-To-Screen  "File[$($fileName)] exists and deleted."
  }
  else {
    Write-Message-To-Screen  "Skipped Deletion. File[$($fileName)] doesn't exist."
  }
}

  function Export-DevVm([string] $vmExportPath) {
    # Delete Export folder if it already exists
    If (Test-Path $vmExportPath) {
      Remove-Item -path $vmExportPath -recurse -force
    }

    # Export VM
    Export-VM -Name $vmName -Path $vmExportPath
  }

  function New-DevVm([string] $vmName, [string] $vmCheckpointName, [string] $vmExportPath, [string] $compressPath, [string] $zipFileName) {
    try {
      Write-Message-To-Screen  "Attempt #$($global:count)"
    
      Write-Message-To-Screen  "Creating VM"
      vagrant up
      Write-Message-To-Screen  "Created VM"

      if ($global:exportVm) {
        Write-Message-To-Screen  "Stopping VM"
        Stop-VM -Name "RelativityDevVm"
        Write-Message-To-Screen  "Stopped VM"

        Write-Message-To-Screen  "Creating VM Checkpoint"
        Checkpoint-VM -Name $vmName -SnapshotName $vmCheckpointName
        Write-Message-To-Screen  "Created VM Checkpoint"

        Write-Message-To-Screen  "Export VM"
        Export-DevVm $vmExportPath
        Write-Message-To-Screen  "Exported VM"

        Write-Message-To-Screen  "Compressing Exported VM to Zip"
        Install-Module -NugetPackageId 7Zip4Powershell -PackageVersion 1.8.0
        Compress-7Zip -Path $compressPath -ArchiveFileName $zipFileName
        Write-Message-To-Screen  "Compressed Exported VM to Zip"
      }
      else {
        Write-Message-To-Screen "Skipped VM Export!"
      }
    }
    finally {
      if ($global:exportVm) {
        Write-Message-To-Screen  "Deleting VM"
        vagrant destroy -f $vmName
        Write-Message-To-Screen  "Deleted VM"
      }
      else {
        Write-Message-To-Screen "Skipped VM Deletion!"
      }    
    }
  }

  function Start-DevVm-Process() {
    $stopWatch = [System.Diagnostics.Stopwatch]::StartNew() 

    # Delete Results file if it already exists
    Delete-File-If-It-Exists $global:scriptResultFileName

    # while ($global:count -le $global:maxRetry) {
    try {
      $vmName = "RelativityDevVm"
      $vmCheckpointName = "RelativityDevVm Created"
      $vmExportPath = "C:\DevVmExport"
      $compressPath = "$($vmExportPath)\$($vmName)"
      $zipFileName = "$($vmExportPath)\$($vmName).7z"
    
      # Create New DevVm
      New-DevVm $vmName, $vmCheckpointName, $vmExportPath, $compressPath, $zipFileName
    }
    #catch {
    #  $global:count++ 
    #}
    Catch [Exception] {
      $global:count++
      Write-Message-To-Screen "-----> Exception: $($_.Exception.GetType().FullName)"
      Write-Message-To-Screen "-----> Exception Message: $($_.Exception.Message)"
    }
    finally {
      Write-Message-To-Screen  "Total time: $($stopWatch.Elapsed.TotalMinutes) minutes"
      $stopWatch.Stop() 
    }
    # }
  }

  Start-DevVm-Process
