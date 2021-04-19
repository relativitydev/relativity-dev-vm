custom_log 'custom_log' do msg 'Starting Updating Elasticsearch Yml File' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Update Elasticsearch Yaml File
custom_log 'custom_log' do msg 'Updating Elasticsearch Yml File' end

powershell_script 'update_data_grid_yml_file' do
  code <<-EOH
    #{node['powershell_module']['import_module']}
    Reset-ElasticSearchYmlFile  -ErrorAction Stop
    EOH
end

custom_log 'custom_log' do msg 'Updated Elasticsearch Yml File' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Updating Elasticsearch Yml File\n\n\n" end