custom_log 'custom_log' do msg 'Starting Install Data Grid Configure Environment' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

# Update Data Grid Instance Settings
custom_log 'custom_log' do msg 'Updating RelativityWebAPI Instance Setting.' end
powershell_script 'update_instance_setting - RelativityWebAPI' do
    code <<-EOH
    Import-Module "#{sqlps_module_path}"
    Invoke-Sqlcmd -Query "
    
    -- Update Datagrid Endpoint
        UPDATE
            [EDDS].[eddsdbo].[InstanceSetting]
        SET
            [Value] = 'http://#{node['windows']['hostname']}:9200/'
        WHERE 
            [Section] = 'Relativity.DataGrid' AND
            [Name] = 'DataGridEndPoint'	
    
    -- Update Datagrid DataGridIndexPrefix
        UPDATE
            [EDDS].[eddsdbo].[InstanceSetting]
        SET
            [Value] = '#{node['windows']['hostname'].downcase}'
        WHERE 
            [Section] = 'Relativity.DataGrid' AND
            [Name] = 'DataGridIndexPrefix'	
    
    -- Update RestUriForCAAT			
        UPDATE
            [EDDS].[eddsdbo].[InstanceSetting]
        SET
            [Value] = 'http://#{node['windows']['hostname']}/Relativity.Rest/API/'
        WHERE 
            [Section] = 'Relativity.Core' AND
            [Name] = 'RestUriForCAAT'	

    -- Update DG Search Index Value			
    UPDATE
        [EDDS].[eddsdbo].[InstanceSetting]
    SET
        [Value] = '1'
    WHERE 
        [Section] = 'Relativity.DataGrid' AND
        [Name] = 'DataGridSearchIndex'
    
    -- Update the number of shards and replicas
        DECLARE @jsonSettings NVARCHAR(MAX)
    
        SET @jsonSettings = (
                                SELECT
                                    [VALUE]
                                FROM [EDDS].[eddsdbo].[InstanceSetting] WITH(NOLOCK)
                                WHERE 
                                    ISJSON(VALUE)> 0 AND
                                    [Section] = 'Relativity.DataGrid' AND
                                    [Name] = 'DataGridIndexCreationSettings'
                            )
        
        -- Change Number of Chards
        SET @jsonSettings = JSON_MODIFY(@jsonSettings, '$.settings.index.number_of_shards', #{node['software']['elastic_search']['number_of_chards']})
    
        -- Change Number of Replicas
        SET @jsonSettings = JSON_MODIFY(@jsonSettings, '$.settings.index.number_of_replicas', #{node['software']['elastic_search']['number_of_replicas']})
    
        -- Save Update
        UPDATE
            [EDDS].[eddsdbo].[InstanceSetting]
        SET
            [Value] = @jsonSettings
        WHERE 
            [Section] = 'Relativity.DataGrid' AND
            [Name] = 'DataGridIndexCreationSettings'
    
    "
    EOH
end

# Add/Update the DataGrid Enabled Instance Setting
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

# Update Data Grid Instance Settings
custom_log 'custom_log' do msg 'Updating RelativityWebAPI Instance Setting.' end
powershell_script 'update_instance_setting - RelativityWebAPI' do
    code <<-EOH
    #############################INCLUDE SECTION##############################
    Import-Module "#{powershell_functions_script_path}" -force
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\kCura.Relativity.Client.dll"
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\Relativity.API.dll"
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\Relativity.Services.Interfaces.Private.dll"
    Add-Type -Path "C:\\Program Files\\kCura Corporation\\Relativity\\Library\\Relativity.Services.ServiceProxy.dll"
    ##########################################################################

    $serverName = "#{node['windows']['hostname']}"
    $username = "#{node['sample_data_population']['relativity_admin_account']['login']}"
    $password = "#{node['sample_data_population']['relativity_admin_account']['password']}"
    $instanceSettingSection = "Relativity.DataGrid"
    $instanceSettingName = "DataGridEnabled"

    # Find the datagridenabled instance setting
    $dataGridInstanceSettingQueryResults = QueryInstanceSetting $serverName $username  $password #instanceSettingSection $instanceSettingName
    
    if($dataGridInstanceSettingQueryResults.Success -eq $TRUE -and $instanceSettingResults.Results.Count -gt 0){
        # instance setting exists, make sure it's true
        UpdateInstanceSettingValue $serverName $username  $password #instanceSettingSection $instanceSettingName "True"
    }else{
        # Create instance setting
        CreateInstanceSetting $serverName $username $password $instanceSettingSection $instanceSettingName TrueFalse "True"
    }

        EOH
    end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Install Data Grid Configure Environment\n\n\n" end
    