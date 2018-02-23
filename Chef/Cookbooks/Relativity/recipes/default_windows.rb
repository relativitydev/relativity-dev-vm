custom_log 'custom_log' do msg 'Starting Windows Setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Disable Windows Firewall
include_recipe 'Relativity::windows_disable_firewall'

# Set Windows Explorer Properties
include_recipe 'Relativity::windows_set_explorer_properties'

# Gives background processes priority
include_recipe 'Relativity::windows_give_background_processes_priority'

# Set windows auto login with system admin credentials when the system starts
include_recipe 'Relativity::windows_set_auto_login'

# Install softwares
include_recipe 'Relativity::windows_install_software'

# Add programs to windows taskbar
include_recipe 'Relativity::windows_add_programs_to_taskbar'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Windows Setup\n\n\n" end
