custom_log 'custom_log' do msg 'Starting Updating Java Environment Variables' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    # Generate Import Powershell module code
    powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'DevVmPsModules.dll'))
    IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze
    
    # Update Java Environment Variables
    custom_log 'custom_log' do msg 'Updating Java Environment Variables' end
    
    powershell_script 'update_java_environment_variables' do
      code <<-EOH
        #{IMPORT_MODULE}
        Reset-JavaEnvironmentVariables
        EOH
    end
    
    custom_log 'custom_log' do msg 'Updated Java Environment Variables' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Finished Updating Java Environment Variables\n\n\n" end