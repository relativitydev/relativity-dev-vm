custom_log 'custom_log' do msg 'Starting Default recipe' end
start_time = DateTime.now
custom_log 'custom_log' do msg "cookbook_start_time(#{cookbook_name}): #{start_time}" end

###################################################
# Please refer to Vagrantfile or kitchen.yml files
###################################################

end_time = DateTime.now
custom_log 'custom_log' do msg "cookbook_end_Time(#{cookbook_name}): #{end_time}" end
custom_log 'custom_log' do msg "cookbook_duration(#{cookbook_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Default recipe\n\n\n" end
