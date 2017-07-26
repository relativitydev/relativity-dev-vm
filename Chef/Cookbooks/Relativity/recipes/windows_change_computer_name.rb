log 'Starting computer name update'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

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

# reboot after chef run completes
reboot 'change_computer_name_requires_reboot' do
  action :request_reboot
  reason 'Need to reboot when the run completes successfully.'
  delay_mins 5
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished computer name update'
