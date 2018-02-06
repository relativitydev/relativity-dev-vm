log 'Starting copying install response files'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Copy and Parse Relativity Response File
include_recipe 'Relativity::pre_relativity_copy_install_response_files_parse_relativity_response_file'

# Copy and Parse Invariant Response File
include_recipe 'Relativity::pre_relativity_copy_install_response_files_parse_invariant_response_file'

# Verify and Create Relativity and Invariant Response files
include_recipe 'Relativity::pre_relativity_copy_install_response_files_verify_and_create_response_file'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished copying install response files'
