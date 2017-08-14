log 'Starting shared folders creation'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Directories
Backup_directory = 'Backup'
Data_directory = 'Data'
dtSearchIndexes_directory = 'dtSearchIndexes'
Fileshare_directory = 'Fileshare'
Fileshare_EDDS_directory = 'Fileshare\EDDS'
FullText_directory = 'FullText'
InvariantNetworkShare_directory = 'InvariantNetworkShare'
Logs_directory = 'Logs'

ProcessingSourceLocation_directory = 'ProcessingSourceLocation'

BCPPath_directory = 'BCPPath'

Chef_Install_directory = 'Chef_Install'
Chef_Install_ServiceBusDefectWindowsUpdate_directory = 'Chef_Install\ServiceBusDefectWindowsUpdate'

directories = [Backup_directory, Data_directory, dtSearchIndexes_directory, Fileshare_directory, Fileshare_EDDS_directory, FullText_directory, InvariantNetworkShare_directory, Logs_directory, ProcessingSourceLocation_directory, BCPPath_directory, Chef_Install_directory, Chef_Install_ServiceBusDefectWindowsUpdate_directory]

# Create directories
directories.each do |dir|
    directory dir do
        rights :full_control, 'Everyone'
        recursive true
        action :create
    end
end


# Share directories
powershell_script 'Backup_directory' do
    code <<-EOH
    New-SmbShare -Name 'Backup' -Path 'C:/Backup' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Backup'"
    action :run
end

powershell_script 'Data_directory' do
    code <<-EOH
    New-SmbShare -Name 'Data' -Path 'C:/Data' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Data'"
    action :run
end

powershell_script 'dtSearchIndexes_directory' do
    code <<-EOH
    New-SmbShare -Name 'dtSearchIndexes' -Path 'C:/dtSearchIndexes' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'dtSearchIndexes'"
    action :run
end

powershell_script 'Fileshare_directory' do
    code <<-EOH
    New-SmbShare -Name 'Fileshare' -Path 'C:/Fileshare' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Fileshare'"
    action :run
end

powershell_script 'Fileshare_EDDS_directory' do
    code <<-EOH
    New-SmbShare -Name 'Fileshare_EDDS' -Path 'C:/Fileshare/EDDS' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Fileshare_EDDS'"
    action :run
end

powershell_script 'FullText_directory' do
    code <<-EOH
    New-SmbShare -Name 'FullText' -Path 'C:/FullText' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'FullText'"
    action :run
end

powershell_script 'InvariantNetworkShare_directory' do
    code <<-EOH
    New-SmbShare -Name 'InvariantNetworkShare' -Path 'C:/InvariantNetworkShare' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'InvariantNetworkShare'"
    action :run
end

powershell_script 'Logs_directory' do
    code <<-EOH
    New-SmbShare -Name 'Logs' -Path 'C:/Logs' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Logs'"
    action :run
end

powershell_script 'ProcessingSourceLocation_directory' do
    code <<-EOH
    New-SmbShare -Name 'ProcessingSourceLocation' -Path 'C:/ProcessingSourceLocation' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'ProcessingSourceLocation'"
    action :run
end

powershell_script 'BCPPath_directory' do
    code <<-EOH
    New-SmbShare -Name 'BCPPath' -Path 'C:/BCPPath' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'BCPPath'"
    action :run
end

powershell_script 'Chef_Install_directory' do
    code <<-EOH
    New-SmbShare -Name 'Chef_Install' -Path 'C:/Chef_Install' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Chef_Install'"
    action :run
end

powershell_script 'Chef_Install_ServiceBusDefectWindowsUpdate_directory' do
    code <<-EOH
    New-SmbShare -Name 'Chef_Install_ServiceBusDefectWindowsUpdate' -Path 'C:/Chef_Install/ServiceBusDefectWindowsUpdate' -FullAccess 'Everyone'
    EOH
    not_if "(Get-SmbShare).Name -Contains 'Chef_Install_ServiceBusDefectWindowsUpdate'"
    action :run
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished shared folders creation'
