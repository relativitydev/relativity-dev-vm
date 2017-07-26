resource_name :extract_iso

property :name, String, name_property: true
property :iso_source, String
property :target, String
property :creates, String

actions :extract
default_action :extract

action :extract do
  local_path = "#{Chef::Config[:file_cache_path]}/#{name}.iso"

  remote_file local_path do
    source iso_source
  end

  powershell_script "extract_#{name}" do
    code <<-EOS
      $mounted_iso = Mount-DiskImage -ImagePath #{local_path} -PassThru
      $drive_letter = ($mounted_iso | Get-Volume).DriveLetter
      Copy-Item "${drive_letter}:/*" #{target} -Recurse
      Dismount-DiskImage -ImagePath #{local_path}
    EOS
    notifies :delete, "file[#{local_path}]", :immediately
  end

  file local_path do
    action :nothing
  end
end

action_class do
  def whyrun_supported?
    true
  end
end
