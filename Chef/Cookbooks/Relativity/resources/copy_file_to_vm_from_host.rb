resource_name :copy_file_to_vm_from_host

property :name, String, name_property: true
property :file_source, String
property :file_destination, String
property :file_destination_folder, String

actions :copy
default_action :copy

action :copy do
  # Create Destination folder if it not already exists
  directory file_destination_folder do
    action :create
  end

  # Copy file
  remote_file file_destination do
    source file_source
    action :create
    remote_user                node["smb_username"]
    remote_password            node["smb_password"]
    remote_domain              node["smb_domain"]
  end
end

action_class do
  def whyrun_supported?
    true
  end
end
