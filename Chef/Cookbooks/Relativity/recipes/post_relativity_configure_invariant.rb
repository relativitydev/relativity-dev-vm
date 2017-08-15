log 'Starting configuring Invariant'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

powershell_script 'Ensure worker process has started' do
  code <<-EOH
  Start-Process -FilePath "C:\\InvariantNetworkShare\\InvariantWorker.exe"
  Get-Process -Name InvariantWorker
  EOH
  retries 12
  retry_delay 10
end

powershell_script 'Ensure service host has started' do
  code <<-EOH
  Start-Service -Name "kCura Service Host Manager"
  EOH
end

# Create Processing Choice for Processing Source Location
include_recipe 'Relativity::post_relativity_configure_invariant_create_processing_choice'

log 'sleeping 30 secs for the creation of processing choice powershell script to execute.'
sleep 30

# Create a Worker Manager Server code that is backwards compatible
include_recipe 'Relativity::post_relativity_configure_invariant_create_worker_manager_server'

# Add Processing To Resource Pool
include_recipe 'Relativity::post_relativity_configure_invariant_add_processing_to_default_resource_pool'

# Add Agent ResourceServer To Resource Pool
include_recipe 'Relativity::post_relativity_configure_invariant_add_agent_and_worker_servers_to_default_resource_pool'

#todo
# # Configure Worker Server for Processing
# include_recipe 'Relativity::post_relativity_configure_invariant_update_worker_server_for_processing'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished configuring Invariant'