custom_log 'custom_log' do msg 'Starting Elastic Search Setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

install_file_destination = "#{node['software']['elastic_search']['destination_folder']}\\#{node['software']['elastic_search']['file_name']}"

# Copy elastic search install file to VM
copy_file_to_vm_from_host 'copy_elasticsearch_install_zip' do
    file_source node['software']['elastic_search']['source']
    file_destination install_file_destination
    file_destination_folder node['software']['elastic_search']['destination_folder']
end

# Extract Contents of iso file
seven_zip_archive "extract sql iso" do
    source    node['software']['elastic_search']['destination_folder']
    path      'c:/'
    overwrite true
    timeout   300
end
  
# Use Template to create elastic search config file
template node['software']['elastic_search']['config_file_location'] do
  source 'elasticsearch.erb'
  variables(
    elasticsearch_master: [node['windows']['hostname']],
    idsrv_location: "http://#{node['windows']['hostname']}/Relativity/Identity/"
  )
  action :create
  only_if { ::File.exist?(node['software']['elastic_search']['config_file_location']) }
end

# We need to overwrite the kservice.bat file because we were having issues with running a java command in a batch through Chef.
file_path = "#{node['software']['elastic_search']['bin_folder']}\\#{node['software']['elastic_search']['bat_filename']}"
cookbook_file file_path do
  source node['software']['elastic_search']['bat_filename']
  action :create
  notifies :run, 'batch[install_service]', :immediately
end

batch 'install_service' do
  code 'kservice.bat install > service_install.log'
  cwd node['software']['elastic_search']['bin_folder']
  #cwd Chef::Config[:file_cache_path]
  action :nothing
end

directory "C:/RelativityDataGrid/elasticsearch-main/plugins/marvel-agent" do
  recursive true
  action :delete
  only_if { ::Dir.exist?("C:/RelativityDataGrid/elasticsearch-main/plugins/marvel-agent")}
end

registry_key 'HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Apache Software Foundation\\Procrun 2.0\\elasticsearch-service-x64\\Parameters\\Java' do
  values [{
    :name => 'JvmMx',
    :type => :dword,
    :data => node['software']['elastic_search']['max_memory_mb']
  }]
  recursive true
  action :create
end

registry_key 'HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Apache Software Foundation\\Procrun 2.0\\elasticsearch-service-x64\\Parameters\\Log' do
  values [{
    :name => 'Level',
    :type => :string,
    :data => 'Error'
  }]
  recursive true
  action :create
end

# service 'elasticsearch-service-x64' do
#   action [:enable, :start]
# end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Elastic Search Setup\n\n\n" end