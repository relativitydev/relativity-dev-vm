log 'Starting Auto Login setup'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

registry_key 'HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon' do
  values [
    {
      name: 'AutoAdminLogon',
      type: :string,
      data: '1'
    },
    {
      name: 'ForceAdminLogon',
      type: :string,
      data: '1'
    },
    {
      name: 'DefaultUserName',
      type: :string,
      data: node['windows']['user']['admin']['login']
    },
    {
      name: 'DefaultPassword',
      type: :string,
      data: node['windows']['user']['admin']['password']
    }
  ]
  action :create
end

log_message "log_message" do  message "Finished setting up windows auto login" end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Auto Login setup'
