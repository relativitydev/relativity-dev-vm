custom_log 'custom_log' do msg 'Starting Checking Correct Relativity Version is Installed' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    rap_file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\RA_Smoke_Test_2.2.2.1.rap'))
    
    # Install Checking Correct Relativity Version is Installed
    custom_log 'custom_log' do msg 'Checking Correct Relativity Version is Installed' end
    
    powershell_script 'checking_correct_relativity_version_is_installed' do
      code <<-EOH
        #{node['powershell_module']['import_module']}
        $relativityAndInvariantVersions = [System.IO.File]::ReadAllText(#{node['relativity_and_invariant']['response_file']['version_file']})
        $indexOfComma = $relativityAndInvariantVersions.IndexOf(',')
        $indexOfColon = $relativityAndInvariantVersions.IndexOf(':')
        $installerRelativityVersion = $relativityAndInvariantVersions.Substring(($indexOfColon + 2), ($indexOfComma - ($indexOfColon + 2)))
        $installerRelativityVersion = $installerRelativityVersion.Replace(" ", "")
        Show-InstallerAndInstanceRelativityVersionAreEqual -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -SqlAdminUserName #{node['sql']['user']['eddsdbo']['login']} -SqlAdminPassword #{node['sql']['user']['sa']['password']} -InstallerRelativityVersion $installerRelativityVersion 
        EOH
    end
    
    
    custom_log 'custom_log' do msg 'Checked Correct Relativity Version is Installed' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Checking Correct Relativity Version is Installed\n\n\n" end