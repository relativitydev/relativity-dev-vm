resource_name :custom_log

property :name, String, name_property: true
property :msg, String

actions :log
default_action :log

action :log do
  log_file = "#{node['file']['log']['default_destination_folder']}\\#{node['file']['log']['name']}"

  # Write to console log
  # log msg
  log 'msg' do
    message msg
    level :info
  end

  # Write to log file
  ruby_block 'write_to_log_file' do
    block do
      msg_with_timestamp = "[#{Time.now.strftime('%FT%T%:z')}] #{msg} \n"
      ::File.open(log_file, 'a') do |line|
        line.write msg_with_timestamp
      end
    end
    action :run
  end
end

action_class do
  def whyrun_supported?
    true
  end
end
