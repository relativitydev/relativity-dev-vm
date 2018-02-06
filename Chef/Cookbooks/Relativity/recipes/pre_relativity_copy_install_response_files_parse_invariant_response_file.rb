log 'Starting Pre-Relativity Parse Invariant File'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

response_file_path = "#{node['invariant']['response_file']['destination_folder']}\\#{node['invariant']['response_file']['file_name_original']}"

# Copy response file
copy_file_to_vm_from_host "copy_relativity_response_file" do
	file_source "#{node['invariant']['response_file']['source_folder']}\\#{node['invariant']['response_file']['file_name']}"
    file_destination response_file_path
    file_destination_folder node['invariant']['response_file']['destination_folder']
end

# Read File into String
::File.open(response_file_path, "r:bom|utf-8") do |f|
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
				node.default['invariant']['response_file']['parsed_values'] << {name: line_elements[0].strip, value: ""}
			end
		end
	end
end

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Pre-Relativity Parse Invariant Response File'
