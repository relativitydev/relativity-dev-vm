custom_log 'custom_log' do msg 'Copying Relativity and Invariant Version Numbers file to VM' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

version_number_file = 'relativity_invariant_version.txt'
version_number_file_source = "#{node['relativity']['install']['source_folder']}\\#{version_number_file}"
version_number_file_destination = "#{node['relativity']['install']['destination_folder']}\\#{version_number_file}"

# Copy Relativity and Invariant Version Number file to VM
copy_file_to_vm_from_host 'copy_relativity_invariant_version_number_file' do
  file_source version_number_file_source
  file_destination version_number_file_destination
  file_destination_folder node['relativity']['install']['destination_folder']
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished copying Relativity and Invariant Version Numbers file to VM\n\n\n" end
