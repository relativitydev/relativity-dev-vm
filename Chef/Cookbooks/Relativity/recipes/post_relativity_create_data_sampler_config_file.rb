custom_log 'custom_log' do msg 'Starting Create Data Sampler Config File' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Create a path to the powershell scripts in the Chef cache.
powershell_functions_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_functions.ps1'))
create_data_sampler_config_file_script_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], 'powershell_post_relativity_create_data_sampler_config_file.ps1'))

# Copy the powershell functions file from the cookbook to the Chef cache
cookbook_file powershell_functions_script_path do
  source 'powershell_functions.ps1'
end

# Copy the powershell script template file from the cookbook to the Chef cache
template create_data_sampler_config_file_script_path do
  variables(
    'config_file_name': node['sample_data_population']['config_file_name'],
    'config_file_destination_location': node['sample_data_population']['config_file_path'],
    'relativity_username': node['sample_data_population']['relativity_admin_account']['login'],
    'relativity_password': node['sample_data_population']['relativity_admin_account']['password'],
    'number_of_documents': node['sample_data_population']['number_of_documents'],
    'import_images_with_documents': node['sample_data_population']['import_images_with_Documents'],
    'import_production_images_with_documents': node['sample_data_population']['import_production_images_with_documents']
  )
  source 'powershell_post_relativity_create_data_sampler_config_file.ps1.erb'
end

# Run the powershell scripts to create a workspace with sample data
powershell_script 'Create Data Sampler Config File' do
  code create_data_sampler_config_file_script_path
end

# add to resource files variable which will be pushed to relativity later
node.default['relativity_resource_files_to_push'] << 
  # [resource file name located in files/default], Application Guid
  [node['sample_data_population']['config_file_name'], '3E86B18F-8B55-45C4-9A57-9E0CBD7BAF46'] # This file is dynamically created by post_relativity_install_push_resource_file
  
end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Creating Data Sampler Config File\n\n\n" end
