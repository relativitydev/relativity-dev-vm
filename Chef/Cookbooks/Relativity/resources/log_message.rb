resource_name :log_message

property :name, String, name_property: true
property :message, String

actions :log
default_action :log

action :log do
  log_file = "#{node['file']['log']['default_destination_folder']}\\#{node['file']['log']['name']}"

  # Write to console log
  log message
  
  # Write to log file
  ruby_block 'write_to_log_file' do
    block do

      message_with_timestamp = "[#{Time.now.strftime('%FT%T%:z')}] #{message} \n"

      ::File.open(log_file, 'a') do |line|
        line.write message_with_timestamp
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
