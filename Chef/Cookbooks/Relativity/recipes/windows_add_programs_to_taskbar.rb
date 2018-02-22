custom_log 'custom_log' do msg 'Starting adding programs to taskbar' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

programs = {
  'powershell.exe' => 'C:\Windows\System32\WindowsPowerShell\v1.0',
  'msvsmon.exe' => 'C:\Program Files\Microsoft Visual Studio 15.0\Common7\IDE\Remote Debugger\x64',
  'notepad++.exe' => 'C:\Program Files\Notepad++',
  'services.msc' => 'C:\Windows\system32',
}

programs.each do |app, location|
  powershell_script 'pin apps to taskbar' do
    code <<-EOH
      $shell = new-object -com "Shell.Application"
      $folder = $shell.Namespace('#{location}')
      $item = $folder.Parsename('#{app}')
      $verb = $item.Verbs() | ? {$_.Name -eq 'Pin to Tas&kbar'}
      if ($verb) {$verb.DoIt()}
    EOH
    timeout node['timeout']['default']
  end
end

custom_log 'custom_log' do msg 'Finished adding program shortcuts to taskbar' end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished adding programs to taskbar' end
