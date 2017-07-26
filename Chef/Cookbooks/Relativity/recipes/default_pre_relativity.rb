log 'Starting Pre-Relativity Setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Nuget provider
include_recipe 'Relativity::pre_relativity_install_nuget_provider'

# Install Windows Features and Services
include_recipe 'Relativity::pre_relativity_install_windows_features_and_services'

# Create shared folders
include_recipe 'Relativity::pre_relativity_create_shared_folders'

#todo
# # Copy Install files to node
# include_recipe 'Relativity::pre_relativity_copy_install_files_to_node'

# Install Microsoft SQL Server
include_recipe 'Relativity::pre_relativity_install_sqlserver'

# Install ServiceBus
include_recipe 'Relativity::pre_relativity_install_servicebus'

# Add Service Bus module to powershell global profile
include_recipe 'Relativity::pre_relativity_add_service_bus_module_to_powershell_global_profile'

# Install Windows Update for Service Bus defect
include_recipe 'Relativity::pre_relativity_install_windows_update_for_service_bus_defect'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Pre-Relativity Setup'
