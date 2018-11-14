custom_log 'custom_log' do msg 'Saving Secret Store unseal key file' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

file_source = node['secret_store']['unseal_key']
file_destination = node['secret_store']['unseal_key']['save_to_local_file_destination_path']
file_destination_folder = node['secret_store']['unseal_key']['save_to_local_file_destination_folder']

custom_log 'custom_log' do msg 'Creating secret store folder' end
# Create destination folder if it not already exists
directory file_destination_folder do
  action :create
end

# Copying file
powershell_script 'Copying secret store unseal key to local file' do
  code <<-EOH
    If (Test-Path "#{file_destination}") {
      Remove-Item -path "#{file_destination}" -Force
    }
    If (Test-Path "#{file_source}") {
      Copy-Item -Path "#{file_source}" -Destination "#{file_destination}" -Force
    }
  EOH
end
custom_log 'custom_log' do msg 'Copied secret store unseal key to local file.' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Saved Secret Store unseal key file\n\n\n" end
