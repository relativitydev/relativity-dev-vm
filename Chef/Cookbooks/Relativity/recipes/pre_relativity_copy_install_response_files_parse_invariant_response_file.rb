custom_log 'custom_log' do msg 'Starting Pre-Relativity Parse Invariant File' end
start_time = DateTime.now
custom_log 'custom_log' do msg "recipe_start_time(#{recipe_name}): #{start_time}" end

response_file_path = "#{node['invariant']['response_file']['destination_folder']}\\#{node['invariant']['response_file']['file_name_original']}"

# Read File into String
File::open(response_file_path, "r:bom|utf-8") do |f|
  values = Array.new 
  f.each_line do |line|
    # Remove surrounding whitespace
    clean_line = line.strip

    # Skip empty lines and skip lines lines that start with pound sign
    if clean_line != "" && clean_line.length > 0 && clean_line[0] != "#"
      # Split string into array to target Response File Properties
      # INSTALLPRIMARYDATABASE=0 -> we only want "INSTALLPRIMARYDATABASE"
      line_elements = clean_line.split("=")

      if line_elements.length > 0
        # Add response file property to node array
        name = line_elements[0].strip
        value = ''
        values.push( Hash['name' => name, 'value' => value])
        puts "Invariant Response File - Parsed Value: #{name}"
      end
    end
  end
  node.default['invariant']['response_file']['parsed_values'] = values
end

end_time = DateTime.now
custom_log 'custom_log' do msg "recipe_end_Time(#{recipe_name}): #{end_time}" end
custom_log 'custom_log' do msg "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds" end
custom_log 'custom_log' do msg 'Finished Pre-Relativity Parse Invariant Response File' end
