custom_log 'custom_log' do msg 'Delete script result file' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

file_source_folder = node['file']['installers']['default_destination_folder']
file_source = "#{file_source_folder}\\#{node['file']['result']['name']}"

# Create file
powershell_script 'Delete script result file' do
  code <<-EOH
    If (Test-Path "#{file_source}") {
      Remove-Item -path "#{file_source}" -Force
    }
  EOH
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Deleted script result file\n\n\n" end
