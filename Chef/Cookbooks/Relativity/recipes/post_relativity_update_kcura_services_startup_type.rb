log 'Starting kCura Services Startup Type update'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

dsc_resource 'Start kCura EDDS Agent Manager' do
  resource :service
  property :name, 'kCura EDDS Agent Manager'
  property :startuptype, 'Automatic'
  property :state, 'Running'
  # timeout node['timeout']['default'] #todo
end

dsc_resource 'Start kCura EDDS Web Processing Manager' do
  resource :service
  property :name, 'kCura EDDS Web Processing Manager'
  property :startuptype, 'Automatic'
  property :state, 'Running'
  # timeout node['timeout']['default'] #todo
end

dsc_resource 'Start kCura Service Host Manager' do
  resource :service
  property :name, 'kCura Service Host Manager'
  property :startuptype, 'Automatic'
  property :state, 'Running'
  # timeout node['timeout']['default'] #todo
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished kCura Services Startup Type update'
