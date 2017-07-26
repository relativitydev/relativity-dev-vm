log 'Starting Processing Choice creation'
start_time = DateTime.now
log "recipe_start_time(#{recipe_name}): #{start_time}"

# Get the full path to the SQLPS module.
sqlps_module_path = ::File.join(ENV['programfiles(x86)'], 'Microsoft SQL Server\130\Tools\PowerShell\Modules\SQLPS')

log 'Creating Processing Choice.'
powershell_script 'Create Processing Choice' do
  guard_interpreter :powershell_script
  code <<-CODE
  $rsapiDllPath = 'C:\\Program Files\\kCura Corporation\\Relativity\\Library\\kCura.Relativity.Client.dll'
  $rsapiUri =  New-Object System.Uri("http://#{node['windows']['hostname']}/Relativity.Services/")
  $rsapiUserName = "#{node['relativity']['admin']['login']}"
  $rsapiPassword = "#{node['relativity']['admin']['password']}"

  Add-Type -Path $rsapiDllPath
  $usernamePassCredentials = New-Object kCura.Relativity.Client.UsernamePasswordCredentials($rsapiUserName, $rsapiPassword)
  $proxy = New-Object kCura.Relativity.Client.RSAPIClient($rsapiUri, $usernamePassCredentials)
  $proxy.APIOptions.WorkspaceID = -1
  $choice = New-Object kCura.Relativity.Client.DTOs.Choice
  $choice.ChoiceTypeID =  1000017

  $choice.Name = "#{node['relativity']['processing']['source']['location']}"
  $choice.Order = 1
  $choiceResults = $proxy.Repositories.Choice.Create($choice)

  Write-Output $choiceResults
  Write-Output $choiceResults.Results[0]
  CODE
  only_if <<-EOH
  !(invoke-sqlcmd -username sa -password '#{node['sql']['user']['sa']['password']}' -serverinstance '#{node['windows']['hostname']}' -Query "select * from [EDDS].[eddsdbo].[Code] where CodeTypeID=(select ct.CodeTypeID from [EDDS].[eddsdbo].[CodeType] ct where Name = 'ProcessingSourceLocation') and name = '#{node['relativity']['processing']['source']['location']}'")
  EOH
  timeout node['timeout']['default']
end
log 'Created Processing Choice.'

end_time = DateTime.now
log "recipe_end_Time(#{recipe_name}): #{end_time}"
log "recipe_duration(#{recipe_name}): #{end_time.to_time - start_time.to_time} seconds"
log 'Finished Processing Choice creation'
