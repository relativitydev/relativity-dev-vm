[System.Int32]$global:maxRetry = 1
[System.Int32]$global:count = 1
[System.Int32]$global:count = 1
[Boolean] $global:exportVm = $false

function Export-DevVm([string] $vmExportPath) {
  # Remove Export folder if it already exists
  If (Test-Path $vmExportPath) {
    Remove-Item �path $vmExportPath �recurse �force
  }

  # Export VM
  Export-VM -Name $vmName -Path $vmExportPath
}

function New-DevVm([string] $vmName, [string] $vmCheckpointName, [string] $vmExportPath, [string] $compressPath, [string] $zipFileName) {
  try {
    Write-Host  "-----> [$(Get-Date -Format g)] Attempt #$($global:count)" -ForegroundColor Blue
    
    Write-Host  "-----> [$(Get-Date -Format g)] Creating VM" -ForegroundColor Blue
    vagrant up
    Write-Host  "-----> [$(Get-Date -Format g)] Created VM" -ForegroundColor Blue

    if ($global:exportVm) {
      Write-Host  "-----> [$(Get-Date -Format g)] Stopping VM" -ForegroundColor Blue
      Stop-VM -Name "RelativityDevVm"
      Write-Host  "-----> [$(Get-Date -Format g)] Stopped VM" -ForegroundColor Blue

      Write-Host  "-----> [$(Get-Date -Format g)] Creating VM Checkpoint" -ForegroundColor Blue
      Checkpoint-VM -Name $vmName -SnapshotName $vmCheckpointName
      Write-Host  "-----> [$(Get-Date -Format g)] Created VM Checkpoint" -ForegroundColor Blue

      Write-Host  "-----> [$(Get-Date -Format g)] Export VM" -ForegroundColor Blue
      Export-DevVm $vmExportPath
      Write-Host  "-----> [$(Get-Date -Format g)] Exported VM" -ForegroundColor Blue

      Write-Host  "-----> [$(Get-Date -Format g)] Compressing Exported VM to Zip" -ForegroundColor Blue
      Install-Module -NugetPackageId 7Zip4Powershell -PackageVersion 1.8.0
      Compress-7Zip -Path $compressPath -ArchiveFileName $zipFileName
      Write-Host  "-----> [$(Get-Date -Format g)] Compressed Exported VM to Zip" -ForegroundColor Blue
    }
    else {
      Write-Host "Skipped VM Export!"
    }
  }
  finally {
    if ($global:exportVm) {
      Write-Host  "-----> [$(Get-Date -Format g)] Deleting VM" -ForegroundColor Blue
      vagrant destroy -f $vmName
      Write-Host  "-----> [$(Get-Date -Format g)] Deleted VM" -ForegroundColor Blue
    }
    else {
      Write-Host "Skipped VM Deletion!"
    }    
  }
}

function Start-DevVm-Process() {
  $stopWatch = [System.Diagnostics.Stopwatch]::StartNew() 

  while ($global:count -le $global:maxRetry) {
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
      Write-Host "$($_.Exception.GetType().FullName) $($_.Exception.Message)"
    }
    finally {
      Write-Host  "-----> [$(Get-Date -Format g)] Total time: $($stopWatch.Elapsed.TotalMinutes) minutes" -ForegroundColor Blue
      $stopWatch.Stop() 
    }
  }
}

Start-DevVm-Process