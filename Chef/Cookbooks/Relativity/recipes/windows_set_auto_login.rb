custom_log 'custom_log' do msg 'Starting Auto Login setup' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

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

custom_log 'custom_log' do msg 'Finished setting up windows auto login' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished Auto Login setup\n\n\n" end
