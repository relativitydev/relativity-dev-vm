custom_log 'custom_log' do msg 'Starting Updating Relativity Logos' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

admin_image_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\Relativity-Button-Admin.png'))
small_image_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\Relativity-Button.png'))
relativity_login_image_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], '\cookbooks\Relativity\files\default\relativityLogin.png'))
destination_admin_image_path = "C:\\Program Files\\kCura Corporation\\Relativity\\EDDS\\Images\\Relativity-Button-Admin.png"
destination_samll_image_path = "C:\\Program Files\\kCura Corporation\\Relativity\\EDDS\\Images\\Relativity-Button.png"
destination_relativity_loging_image_path = "C:\\Program Files\\kCura Corporation\\Relativity\\EDDS\\Images\\relativityLogin.png"

# Update Relativity Logos
custom_log 'custom_log' do msg 'Updating Relativity Logos' end

powershell_script 'update_relativity_logos' do
  code <<-EOH
    if (Test-Path -Path #{admin_image_path}){
      Copy-Item  -Path #{admin_image_path} -Destination "#{destination_admin_image_path}" -Force
    }
    else{
      throw [System.IO.FileNotFoundException] "#{admin_image_path} not found."
    }
    if (Test-Path -Path #{small_image_path}){
      Copy-Item  -Path #{small_image_path} -Destination "#{destination_samll_image_path}" -Force
    }
    else{
      throw [System.IO.FileNotFoundException] "#{small_image_path} not found."
    }
    if (Test-Path -Path #{relativity_login_image_path}){
      Copy-Item  -Path #{relativity_login_image_path} -Destination "#{destination_relativity_loging_image_path}" -Force
    }
    else{
      throw [System.IO.FileNotFoundException] "#{relativity_login_image_path} not found."
    }
    iisreset
    EOH
end

custom_log 'custom_log' do msg 'Updated Relativity Logos' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Updating Relativity Logos\n\n\n" end