resource_name :throw_custom_error

property :name, String, name_property: true
property :error_message, String

actions :throw
default_action :throw

action :throw do
  # Log error message
  log_message 'log_message' do message "Throwing custom error [Message: #{error_message}]" end

  # Sleep 2 secs for the logs to be written to log file
  sleep 2

  # Throw Error
  ruby_block 'throw_error' do
    block do
      fail error_message
    end
  end
end

action_class do
  def whyrun_supported?
    true
  end
end
