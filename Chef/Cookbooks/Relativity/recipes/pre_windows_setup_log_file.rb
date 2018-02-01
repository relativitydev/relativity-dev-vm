log 'Starting Log file Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Create Log Destination folder if it not already exists
directory node['file']['log']['default_destination_folder'] do
  action :create
end

log_file = "#{node['file']['log']['default_destination_folder']}\\#{node['file']['log']['name']}"

# Delete log file if it already exists
ruby_block 'delete_log_file' do
  block do
    FileUtils.rm_f(log_file)
  end
  action :run
end

# Create log file if not already exists
ruby_block 'create_log_file' do
  block do
    File.new(log_file, 'w+')
  end
  action :run
end

log_message "log_message" do  message "Finished setting up log file" end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Log file Setup'
