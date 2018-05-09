custom_log 'custom_log' do msg 'Create agents in RAP applications installed' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Copy the AgentsConsole app from the cookbook to the Chef cache
agents_console_exe_file_name = 'AgentsConsole.exe'
agents_console_exe_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], agents_console_exe_file_name))
cookbook_file agents_console_exe_path do
  source agents_console_exe_file_name
end

files_to_copy = {
  'AgentsConsole' => 'AgentsConsole.exe',
  'AgentUtilities' => 'AgentUtilities.dll',
  'Newtonsoft.Json' => 'Newtonsoft.Json.dll',
  # 'Relativity.Kepler.dll' => 'Relativity.Kepler.dll',
}

# Copy files from cookbook to the Chef cache
files_to_copy.each do |name, file|
  custom_log 'custom_log' do msg "Copying file from cookbook to the Chef cache - #{name}" end
  file_path = win_friendly_path(File.join(Chef::Config[:file_cache_path], file))
  cookbook_file file_path do
    source file
  end
  custom_log 'custom_log' do msg "Copied file from cookbook to the Chef cache - #{name}" end
end

node['relativity_apps_agents_to_install'].each do |app, guid|
  current_context = "Agents for '#{app}' relativity application [Guid: #{guid}]"
  custom_log 'custom_log' do msg "Creating #{current_context}" end
  execute "Create #{current_context}" do
    command "#{agents_console_exe_path} \"#{node['windows']['hostname']}\" \"#{node['relativity']['admin']['login']}\" \"#{node['relativity']['admin']['password']}\" \"#{node['windows']['hostname']}\" \"#{node['sql']['user']['eddsdbo']['login']}\" \"#{node['sql']['user']['eddsdbo']['password']}\" \"#{guid}\""
    retries 5
    retry_delay 5
  end
  custom_log 'custom_log' do msg "Created #{current_context}" end
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished creating agents in RAP applications installed\n\n\n" end
