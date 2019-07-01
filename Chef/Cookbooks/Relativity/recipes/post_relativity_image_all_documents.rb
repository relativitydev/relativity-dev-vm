custom_log 'custom_log' do msg 'Starting Imaging All Documents' end
    start_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end
        
    # Image All Documents
    custom_log 'custom_log' do msg 'Imaging All Documents' end
    
    powershell_script 'image_all_documents' do
      code <<-EOH
        #{node['powershell_module']['import_module']}
        Add-ImagesForAllDocuments -RelativityInstanceName #{node['windows']['new_computer_name']} -RelativityAdminUserName #{node['relativity']['admin']['login']} -RelativityAdminPassword #{node['relativity']['admin']['password']} -WorkspaceName "#{node['sample_data_grid_workspace_name']}"
        EOH
    end
    
    custom_log 'custom_log' do msg 'Imaged All Documents' end
    
    end_time = DateTime.now
    custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
    custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
    custom_log 'custom_log' do msg "Finished Imaging All Documents\n\n\n" end