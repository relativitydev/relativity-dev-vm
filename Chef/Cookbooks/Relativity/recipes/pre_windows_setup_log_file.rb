log 'Starting Log file Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Generate log file name based on current time
generated_log_file_name = 'Chef_Log_' + Time.now.strftime('%Y%m%dT%H%M%S%z') + '.txt'
log "Log File name generated: #{generated_log_file_name}"
node.default['file']['log']['name'] = generated_log_file_name
log "Log File: #{node['file']['log']['name']}"

# Create Destination folder if it not already exists
directory node['file']['log']['default_destination_folder'] do
  action :create
end

log_file = "#{node['file']['log']['default_destination_folder']}\\#{node['file']['log']['name']}"

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
