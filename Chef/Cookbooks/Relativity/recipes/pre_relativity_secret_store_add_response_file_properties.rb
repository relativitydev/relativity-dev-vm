custom_log 'custom_log' do msg 'Adding response file properties to secret store' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Add response file properties to secret store
custom_log 'custom_log' do msg 'Running secret store client command to add response file properties to secret store' end
execute 'add_response_file_properties_to_secret_store' do
  command "#{node['secret_store']['install']['client']['location']} secret write #{node['windows']['new_computer_name']} EDDSDBOPASSWORD=#{node['sql']['user']['eddsdbo']['password']} SERVICEUSERNAME=#{node['windows']['hostname']}\\#{node['windows']['user']['admin']['login']} SERVICEPASSWORD=#{node['windows']['user']['admin']['password']} SQLPASSWORD=#{node['sql']['user']['eddsdbo']['password']} SQLUSERNAME=#{node['sql']['user']['sa']['login']}"
end
custom_log 'custom_log' do msg 'Ran secret store client command to add response file properties to secret store' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Added response file properties to secret store\n\n\n" end
