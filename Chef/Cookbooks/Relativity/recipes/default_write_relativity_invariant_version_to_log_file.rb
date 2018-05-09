custom_log 'custom_log' do msg 'Writing Relativity and Invariant Version Numbers to log file' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

relativity_install_file = "#{node['relativity']['install']['destination_folder']}\\relativity_invariant_version.txt"
log_file = "#{node['file']['log']['default_destination_folder']}\\#{node['file']['log']['name']}"

powershell_script 'Writing Relativity and Invariant Version Numbers to log file.' do
  code <<-EOH
    If (Test-Path "#{relativity_install_file}") {
      $fileContent = [IO.File]::ReadAllText("#{relativity_install_file}")
      Add-Content -Path "#{log_file}" -Value "`r`n"
      Add-Content -Path "#{log_file}" -Value $fileContent
    }
  EOH
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished writing Relativity and Invariant Version Numbers to log file\n\n\n" end
