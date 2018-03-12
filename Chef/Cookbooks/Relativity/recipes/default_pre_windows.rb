custom_log 'custom_log' do msg 'Starting Pre-Windows Setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Create default folders on VM
include_recipe 'Relativity::pre_windows_create_default_folders'

# Install Nuget provider
include_recipe 'Relativity::pre_windows_install_nuget_provider'

# Change Computer Name
include_recipe 'Relativity::pre_windows_change_computer_name'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Pre-Windows Setup\n\n\n" end
