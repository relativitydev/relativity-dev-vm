custom_log 'custom_log' do msg 'Starting Relativity Setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Install Relativity
include_recipe 'Relativity::relativity_install'

include_recipe 'Relativity::post_relativity_configure_invariant_instance_setting_update_processingwebapipath'
include_recipe 'Relativity::post_relativity_instance_setting_update_cookie_secure'

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Relativity Setup\n\n\n" end
