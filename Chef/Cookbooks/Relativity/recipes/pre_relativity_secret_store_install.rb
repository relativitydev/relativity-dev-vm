custom_log 'custom_log' do msg 'Running Secret Store install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

secret_store_install_file = "#{node['secret_store']['install']['destination_folder']}\\#{node['secret_store']['install']['file_name']}"

# Copy Secret Store Install File
copy_file_to_vm_from_host 'copy_secret_store_install_file' do
  file_source "#{node['secret_store']['install']['source_folder']}\\#{node['secret_store']['install']['file_name']}"
  file_destination secret_store_install_file
  file_destination_folder node['secret_store']['install']['destination_folder']
end

# Install Secret Store
custom_log 'custom_log' do msg 'Installing Secret Store' end
windows_package 'install_secret_store' do
  source secret_store_install_file
  options "SqlInstanceServerName=#{node['windows']['new_computer_name']} SqlUsername=#{node['sql']['user']['sa']['login']} SqlPassword=#{node['sql']['user']['sa']['password']} /repair /log c:\\secretstoreinstall.log"
  installer_type :custom
end
custom_log 'custom_log' do msg 'Installed Secret Store' end

# Initialize Secret Store
custom_log 'custom_log' do msg 'Initializing Secret Store' end
execute 'initialize_secret_store' do
  command "#{node['secret_store']['install']['client']['location']} configure-service #{node['secret_store']['service']['port']} > #{node['secret_store']['init_result']}"
end

ruby_block 'parse_unseal' do
  block do
    init_result = File.readlines("#{node['secret_store']['init_result'].gsub("\\\"", "")}").select { |line| line =~ /#{node['secret_store']['unseal_key_identifier']}/ }
    File.open("#{node['secret_store']['unseal_key']}", 'w') { |file| file.write("#{init_result[0].split(" = ",2)[1]}") }
  end
  only_if { !File.exist?("#{node['secret_store']['unseal_key']}") }
end
custom_log 'custom_log' do msg 'Initialized Secret Store' end

# Unseal Secret Store
custom_log 'custom_log' do msg 'Unsealing Secret Store' end
execute 'initial_unseal' do
  command lazy {"#{node['secret_store']['install']['client']['location']} unseal #{File.readlines(node['secret_store']['unseal_key'])[0]}"}
end

windows_task 'unseal' do
  command lazy {"'#{node['secret_store']['install']['client']['location'].gsub("\\\"", "")}' unseal #{File.readlines(node['secret_store']['unseal_key'])[0]}"}
  frequency :minute
  frequency_modifier 1
end
custom_log 'custom_log' do msg 'Unsealed Secret Store' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Ran Secret Store install'\n\n\n" end
