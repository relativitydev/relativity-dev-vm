custom_log 'custom_log' do msg 'Starting shared folders creation' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Directories
backup_directory = 'Backup'
data_directory = 'Data'
viewer_cache_directory = 'ViewerCache'
dt_search_indexes_directory = 'dtSearchIndexes'
fileshare_directory = 'Fileshare'
fileshare_edds_directory = 'Fileshare\EDDS'
full_text_directory = 'FullText'
invariant_network_share_directory = 'InvariantNetworkShare'
logs_directory = 'Logs'

processing_source_location_directory = 'ProcessingSourceLocation'

bcp_path_directory = 'BCPPath'

chef_install_directory = 'Chef_Install'
chef_install_service_bus_defect_windows_update_directory = 'Chef_Install\Service_Bus_Defect_Windows_Update'

directories = [backup_directory, data_directory, viewer_cache_directory, dt_search_indexes_directory, fileshare_directory, fileshare_edds_directory, full_text_directory, invariant_network_share_directory, logs_directory, processing_source_location_directory, bcp_path_directory, chef_install_directory, chef_install_service_bus_defect_windows_update_directory]

# Create directories
directories.each do |dir|
  directory dir do
    rights :full_control, 'Everyone'
    recursive true
    action :create
  end
end

# Share directories
powershell_script 'backup_directory' do
  code <<-EOH
  New-SmbShare -Name 'Backup' -Path 'C:/Backup' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Backup'"
  action :run
end

powershell_script 'data_directory' do
  code <<-EOH
  New-SmbShare -Name 'Data' -Path 'C:/Data' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Data'"
  action :run
end

powershell_script 'viewer_cache_directory' do
  code <<-EOH
  New-SmbShare -Name 'ViewerCache' -Path 'C:/ViewerCache' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'ViewerCache'"
  action :run
end

powershell_script 'dt_search_indexes_directory' do
  code <<-EOH
  New-SmbShare -Name 'dtSearchIndexes' -Path 'C:/dtSearchIndexes' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'dtSearchIndexes'"
  action :run
end

powershell_script 'fileshare_directory' do
  code <<-EOH
  New-SmbShare -Name 'Fileshare' -Path 'C:/Fileshare' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Fileshare'"
  action :run
end

powershell_script 'fileshare_edds_directory' do
  code <<-EOH
  New-SmbShare -Name 'Fileshare_EDDS' -Path 'C:/Fileshare/EDDS' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Fileshare_EDDS'"
  action :run
end

powershell_script 'full_text_directory' do
  code <<-EOH
  New-SmbShare -Name 'FullText' -Path 'C:/FullText' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'FullText'"
  action :run
end

powershell_script 'invariant_network_share_directory' do
  code <<-EOH
  New-SmbShare -Name 'InvariantNetworkShare' -Path 'C:/InvariantNetworkShare' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'InvariantNetworkShare'"
  action :run
end

powershell_script 'logs_directory' do
  code <<-EOH
  New-SmbShare -Name 'Logs' -Path 'C:/Logs' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Logs'"
  action :run
end

powershell_script 'processing_source_location_directory' do
  code <<-EOH
  New-SmbShare -Name 'ProcessingSourceLocation' -Path 'C:/ProcessingSourceLocation' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'ProcessingSourceLocation'"
  action :run
end

powershell_script 'bcp_path_directory' do
  code <<-EOH
  New-SmbShare -Name 'BCPPath' -Path 'C:/BCPPath' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'BCPPath'"
  action :run
end

powershell_script 'chef_install_directory' do
  code <<-EOH
  New-SmbShare -Name 'Chef_Install' -Path 'C:/Chef_Install' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Chef_Install'"
  action :run
end

powershell_script 'chef_install_service_bus_defect_windows_update_directory' do
  code <<-EOH
  New-SmbShare -Name 'Chef_Install_Service_Bus_Defect_Windows_Update' -Path 'C:/Chef_Install/Service_Bus_Defect_Windows_Update' -FullAccess 'Everyone'
  EOH
  not_if "(Get-SmbShare).Name -Contains 'Chef_Install_Service_Bus_Defect_Windows_Update'"
  action :run
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished shared folders creation\n\n\n" end
