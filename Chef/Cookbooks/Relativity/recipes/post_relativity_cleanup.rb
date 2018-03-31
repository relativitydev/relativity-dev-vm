custom_log 'custom_log' do msg 'Cleaning up the VM by deleting install files' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

powershell_script 'Delete install files' do
  code <<-EOH
    Get-ChildItem -Path C:\\Chef_Install -Recurse | Remove-Item -Force
    Remove-Item C:\\vagrant-chef\\*.* -Force
  EOH
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Cleaned up the VM by deleting install files\n\n\n" end
