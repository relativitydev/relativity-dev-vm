custom_log 'custom_log' do msg 'Starting Pre-Relativity Parse Invariant File' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

response_file_path = "#{node['invariant']['response_file']['destination_folder']}\\#{node['invariant']['response_file']['file_name_original']}"

# Copy response file
copy_file_to_vm_from_host 'copy_relativity_response_file' do
  file_source "#{node['invariant']['response_file']['source_folder']}\\#{node['invariant']['response_file']['file_name']}"
  file_destination response_file_path
  file_destination_folder node['invariant']['response_file']['destination_folder']
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished Pre-Relativity Parse Invariant Response File' end
