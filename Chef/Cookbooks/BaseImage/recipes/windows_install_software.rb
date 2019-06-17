custom_log 'custom_log' do msg 'Starting software install' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

# Install JungUm
include_recipe 'Relativity::windows_install_jungum'
custom_log 'custom_log' do msg 'Finished installing JungUm' end

# Install Lotus Notes
include_recipe 'Relativity::windows_install_lotus_notes'

custom_log 'custom_log' do msg 'Finished installing Lotus Notes' end

# Install Microsoft Office
include_recipe 'Relativity::windows_install_msoffice'

custom_log 'custom_log' do msg 'Finished installing Microsoft Office' end

# Install Microsoft Works Converter
include_recipe 'Relativity::windows_install_msworksconverter'

custom_log 'custom_log' do msg 'Finished installing Microsoft Works Converter' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg "Finished software install\n\n\n" end
