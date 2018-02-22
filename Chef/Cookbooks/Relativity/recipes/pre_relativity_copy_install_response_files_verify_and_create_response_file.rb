# Methods
def parsed_values_count_less_than_replacement_values_count_method(parsed_values_count, replacement_values_count)
  parsed_values_count <= replacement_values_count
end

def does_parsed_and_replacement_values_match_method(name, parsed_values, replacement_values)
  replacement_values_keys = Array.new
  replacement_values.each do |replacement_value|
    replacement_values_keys.push(replacement_value[:name])
  end

  parsed_values_keys = Array.new
  parsed_values.each do |parsed_value|
    parsed_values_keys.push(parsed_value[:name])
  end

  parsed_values_keys_found_replacement = Array.new
  parsed_values_keys_not_found_replacement = Array.new

  custom_log 'custom_log' do msg "Start - #{name} - Loop through parsed values to verify if it has a replacement values" end
  # Loop through parsed values to verify if it has a replacement values
  parsed_values_keys.each do |parsed_value_key|
    # Check if current parsed value exists in replacement value array
    if replacement_values_keys.include?(parsed_value_key)
      parsed_values_keys_found_replacement.push(parsed_value_key)
    else
      parsed_values_keys_not_found_replacement.push(parsed_value_key)
    end
  end
  custom_log 'custom_log' do msg "Finish - #{name} - Looping through parsed values to verify if it has a replacement values" end
  custom_log 'custom_log' do msg "#{name} - parsed_values_keys_found_replacement.length = #{parsed_values_keys_found_replacement.length}" end
  custom_log 'custom_log' do msg "#{name} - parsed_values_keys_not_found_replacement.length = #{parsed_values_keys_not_found_replacement.length}" end

  if parsed_values_keys_not_found_replacement.length > 0
    custom_log 'custom_log' do msg "#{name} - List of current parsed values which do not have a replacement value" end
    parsed_values_keys_not_found_replacement.each do |current_parsed_values_keys_not_found_replacement|
      custom_log 'custom_log' do msg "#{name} - Replacement Value not found - #{current_parsed_values_keys_not_found_replacement}" end
    end
    return false
  else
    return true
  end
end

def create_new_response_file_with_replacement_values_method(name, response_file_path, parsed_values, replacement_values)
  parsed_values_keys = Array.new
  parsed_values.each do |parsed_value|
    parsed_values_keys.push(parsed_value[:name])
  end

  custom_log 'custom_log' do msg "Start - #{name} - Create a new Response File" end
  # Create a new Response File
  ::File.open(response_file_path, 'a') do |line|
    # Loop through each replacement value
    replacement_values.each do |replacement_value|
      replacement_value_key = replacement_value[:name]
      replacement_value_value = replacement_value[:value]
      # If current replacement value is one of the parsed value then write it to Response File
      if parsed_values_keys.include?(replacement_value_key)
        line_to_write = "#{replacement_value_key}=#{replacement_value_value}".strip
        line.write "#{line_to_write}\n"
      end
    end
  end
  custom_log 'custom_log' do msg "Finish - #{name} - Create a new Response File" end
end

def verify_and_create_response_file_method(name, response_file_path, parsed_values, replacement_values)
  # Check if parsed values array count is less than or equal to replacement values array count
  custom_log 'custom_log' do msg "Start = #{name} - Check if parsed values array count is less than or equal to replacement values array count" end
  is_parsed_values_count_less_than_replacement_values_count = parsed_values_count_less_than_replacement_values_count_method(parsed_values.length, replacement_values.length)
  custom_log 'custom_log' do msg "Finish = #{name} - Check if parsed values array count is less than or equal to replacement values array count" end

  # If parsed values array count is less than or equal to replacement values array count, verify each parsed value has a replacement value
  custom_log 'custom_log' do msg "Start - #{name} - If parsed values array count is less than or equal to replacement values array count, verify each parsed value has a replacement value" end
  if is_parsed_values_count_less_than_replacement_values_count
    custom_log 'custom_log' do msg "#{name} - Parsed values count is less than or equal to Replacement values count" end
    # Check if all parsed values exist in replacement values array
    does_parsed_and_replacement_values_match = does_parsed_and_replacement_values_match_method(name, parsed_values, replacement_values)
    if does_parsed_and_replacement_values_match
      custom_log 'custom_log' do msg "#{name} - Parsed and Replacement values match" end
      # Do variable replacement
      create_new_response_file_with_replacement_values_method(name, response_file_path, parsed_values, replacement_values)
    else
      custom_log 'custom_log' do msg "#{name} - Parsed and Replacement values do not match" end
      # Throw custom error
      throw_custom_error 'throw_custom_error' do error_message "#{name} - Parsed and Replacement values do not match" end
    end
  else
    custom_log 'custom_log' do msg "#{name} - Parsed values count is greater than Replacement values count" end
    # Throw custom error
    throw_custom_error 'throw_custom_error' do error_message "#{name} - Parsed values count is greater than Replacement values count" end
  end
  custom_log 'custom_log' do msg "Finish - #{name} - If parsed values array count is less than or equal to replacement values array count, verify each parsed value has a replacement value" end
end

relativity_response_file_path = "#{node['relativity']['install']['destination_folder']}\\#{node['relativity']['response_file']['file_name']}"
relativity_parsed_values = node['relativity']['response_file']['parsed_values']
relativity_replacement_values = node['relativity']['response_file']['replacement_values']
invariant_response_file_path = "#{node['invariant']['install']['destination_folder']}\\#{node['invariant']['response_file']['file_name']}"
invariant_parsed_values = node['invariant']['response_file']['parsed_values']
invariant_replacement_values = node['invariant']['response_file']['replacement_values']

custom_log 'custom_log' do msg 'Verifying and creating Relativity Response file' end
verify_and_create_response_file_method('Relativity', relativity_response_file_path, relativity_parsed_values, relativity_replacement_values)

custom_log 'custom_log' do msg 'Verifying and creating Invariant Response file' end
verify_and_create_response_file_method('Invariant', invariant_response_file_path, invariant_parsed_values, invariant_replacement_values)
