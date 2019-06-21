custom_log 'custom_log' do msg 'Starting Updating DataGrid Instance Settings' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
    
    # Generate Import Powershell module code
    powershell_module_dll_file_full_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\VisualStudioDlls\DevVmPsModules.dll'))
    IMPORT_MODULE = "Import-Module \"#{powershell_module_dll_file_full_path}\" -ErrorAction Stop".freeze
    
    # Update Java Environment Variables
    custom_log 'custom_log' do msg 'Updating DataGrid Instance Settings' end
    
    powershell_script 'update_datagrid_instance_settings' do
      code <<-EOH
        #{IMPORT_MODULE}
        Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -Name DataGridEndPoint -Section Relativity.DataGrid -NewValue " "
        Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -Name AuditDataGridEndPoint -Section kCura.Audit -NewValue http://RelativityDevVm:9200
        Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -Name DataGridSearchIndex  -Section Relativity.DataGrid -NewValue 0
        Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -Name ESIndexCreationSettings -Section kCura.Audit " "
        Reset-InstanceSettingValue -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -Name ESIndexPrefix -Section kCura.Audit -NewValue audit
        EOH
    end
    
    custom_log 'custom_log' do msg 'Updated DataGrid Instance Settings' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Finished Updating DataGrid Instance Settings\n\n\n" end