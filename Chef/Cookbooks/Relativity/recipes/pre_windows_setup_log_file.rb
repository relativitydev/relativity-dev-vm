log 'Starting Log file Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create Log Destination folder if it not already exists
directory node['file']['log']['default_destination_folder'] do
  action :create
end

log_file = "#{node['file']['log']['default_destination_folder']}\\#{node['file']['log']['name']}"
generated_log_file_name = 'Chef_Old_Log_' + Time.now.strftime('%Y%m%dT%H%M%S%z') + '.txt'
log_file_old = "#{node['file']['log']['default_destination_folder']}\\#{generated_log_file_name}"

# Rename log file if it already exists
ruby_block 'rename_previous_log_file' do
  block do
    File.rename(log_file, log_file_old) if File.exist? log_file
  end
  action :run
end

log 'Renamed previous log file'

# Create log file if not already exists
ruby_block 'create_log_file' do
  block do
    File.new(log_file, 'w+')
  end
  action :run
end

log 'Finished setting up log file'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Log file Setup'
