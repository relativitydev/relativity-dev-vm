log 'Starting IIS Reset'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

powershell_script 'IIS_Reset' do
  code <<-EOH
    IISReset
  EOH
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished IIS Reset'
