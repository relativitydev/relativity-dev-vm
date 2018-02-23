custom_log 'custom_log' do msg 'Starting creating default folders' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Create Install Destination folder if it not already exists
directory node['file']['installers']['default_destination_folder'] do
  action :create
end

# Create Sql install folder if it not already exists
directory node['sql']['install']['destination_folder'] do
  action :create
end

# Create ServiceBus Defect Windows Update install folder if it not already exists
directory node['service_bus']['defect_windows_update']['install']['destination_folder'] do
  action :create
end

# Create Relativity install folder if it not already exists
directory node['relativity']['install']['destination_folder'] do
  action :create
end

# Create Invariant install folder if it not already exists
directory node['invariant']['install']['destination_folder'] do
  action :create
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished creating default folders\n\n\n" end
