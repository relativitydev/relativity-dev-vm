custom_log 'custom_log' do msg 'Starting computer name update' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

ps_modules = %w(xComputerManagement)

ps_modules.each do |ps_module|
  powershell_script "install_#{ps_module}_module" do
    code "Install-Module #{ps_module} -Force"
    not_if "(Get-Module -ListAvailable).Name -Contains \"#{ps_module}\""
  end
end

# Change windows computer name
dsc_resource 'change_computer_name' do
  resource :xComputer
  property :Name, node['windows']['new_computer_name']
  timeout node['timeout']['default']
end

custom_log 'custom_log' do msg 'Finished changing computer name' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished computer name update\n\n\n" end
