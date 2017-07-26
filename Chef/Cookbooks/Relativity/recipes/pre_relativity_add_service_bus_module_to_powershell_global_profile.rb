log 'Starting adding service bus module to powershell global profile'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

powershell_script 'add service bus module to powershell global profile' do
  code <<-EOH
    $global_powershell_profile_file_location = 'C:/Windows/system32/WindowsPowerShell/v1.0'
    $global_powershell_profile_file_name = 'profile.ps1'
    $global_powershell_profile_file_name_with_location = $global_powershell_profile_file_location + '/' + $global_powershell_profile_file_name
    $import_service_bus_module = "Import-Module 'C:/Program Files/Service Bus/1.1/ServiceBus/ServiceBus.psd1'"
    $import_service_bus_module_with_newline = "`n" + $import_service_bus_module + "`n"


    if (!(Test-Path $global_powershell_profile_file_name_with_location))
    {
        # File does not exist - create a new file
        New-Item -path $global_powershell_profile_file_location -name $global_powershell_profile_file_name -type "file" -value $import_service_bus_module
    }
    else
    {
        # File already exists
        [string]$service_bus_module_string_in_profile = (Get-Content $global_powershell_profile_file_name_with_location | Select-String $import_service_bus_module)
        [Boolean]$does_module_string_exists = $service_bus_module_string_in_profile.Length -gt 0

        # Check if file contains service bus import module text
        if(!($does_module_string_exists))
        {
            # File doesn't contain service bus import module text so add it
            Add-Content -path $global_powershell_profile_file_name_with_location -value $import_service_bus_module
        }
    }
  EOH
  timeout node['timeout']['default']
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished adding service bus module to powershell global profile'
