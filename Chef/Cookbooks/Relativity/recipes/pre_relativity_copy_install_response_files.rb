custom_log 'custom_log' do msg 'Starting copying install response files' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy and Parse Relativity Response File
include_recipe 'Relativity::pre_relativity_copy_install_response_files_parse_relativity_response_file'

# Copy and Parse Invariant Response File
include_recipe 'Relativity::pre_relativity_copy_install_response_files_parse_invariant_response_file'

# Verify and Create Relativity and Invariant Response files
include_recipe 'Relativity::pre_relativity_copy_install_response_files_verify_and_create_response_file'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished copying install response files\n\n\n" end
