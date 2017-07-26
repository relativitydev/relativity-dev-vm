log 'Starting software install'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Install Software
include_recipe 'chocolatey'
softwares = %w(notepadplusplus vs2015remotetools)
softwares.each do |package|
  chocolatey_package package do
    action :install
  end
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished software install'
