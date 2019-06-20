custom_log 'custom_log' do msg 'Starting Pre-Relativity Setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy Installer Response Files
include_recipe 'Relativity::pre_relativity_copy_install_response_files'

# Copy Installer Files
include_recipe 'Relativity::pre_relativity_copy_install_files'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Pre-Relativity Setup\n\n\n" end
