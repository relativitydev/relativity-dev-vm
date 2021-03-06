custom_log 'custom_log' do msg 'Writing Relativity and Invariant Version Numbers to log file' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

version_number_file = 'relativity_invariant_version.txt'
version_number_file_destination = "#{node['relativity']['install']['destination_folder']}\\#{version_number_file}"

if ::File.file?(version_number_file_destination)
  custom_log 'custom_log' do msg 'Relativity and Invariant Version Number file exists on VM' end
  file_content = ::File.read(version_number_file_destination)
  custom_log 'custom_log' do msg "#{file_content}" end
else
  custom_log 'custom_log' do msg 'Relativity and Invariant Version Number file does NOT exists on VM' end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished writing Relativity and Invariant Version Numbers to log file\n\n\n" end
