custom_log 'custom_log' do msg 'Writing script result to a text file' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

file_source_folder = node['file']['installers']['default_destination_folder']
file_source = "#{file_source_folder}\\#{node['file']['result']['name']}"
file_destination = node['file']['result']['destination_folder']

custom_log 'custom_log' do msg 'Creating and writing script result to a text file.' end
# Create Source folder if it not already exists
directory file_source_folder do
  action :create
end

# Create file
powershell_script 'Create and write script result to a text file.' do
  code <<-EOH
    If (Test-Path "#{file_source}") {
      Remove-Item -path "#{file_source}" -Force
    }
    New-Item "#{file_source}" -type file -Force
    Set-Content -Path "#{file_source}" -Value "success" -Force
    Copy-Item -Path "#{file_source}" -Destination "#{file_destination}" -Force
  EOH
end
custom_log 'custom_log' do msg 'Created and written script result to a text file.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished writing script result to a text file\n\n\n" end
