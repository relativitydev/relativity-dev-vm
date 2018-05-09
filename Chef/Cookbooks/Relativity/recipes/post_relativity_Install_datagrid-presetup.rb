custom_log 'custom_log' do msg 'Starting  Data Grid pre-setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
# Java should have been installed in the Windows_Install_Software recipe
    
# Update the Java Home Path and add it to the Path Variable
powershell_script 'Delete vagrant properties' do
    code <<-EOH
        $javaDirectory = "#{node['software']['java_runtime']['home']}"
        $installationFolderName = Get-ChildItem $javaDirectory | Where-Object {$_.PSIsContainer} | Foreach-Object {$_.Name}
                
        $fullPath = Join-Path -Path $javaDirectory -ChildPath $installationFolderName
        [Environment]::SetEnvironmentVariable("KCURA_JAVA_HOME", $fullPath, "Machine")
        [Environment]::SetEnvironmentVariable("JAVA_HOME", $fullPath, "Machine")
            
        $updatedEnvPath = "$([Environment]::GetEnvironmentVariable("Path"));$($fullPath)"
        [Environment]::SetEnvironmentVariable("Path", $updatedEnvPath, "Machine")
    exit 0
    EOH
end
    
end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Data Grid pre-setup\n\n\n" end