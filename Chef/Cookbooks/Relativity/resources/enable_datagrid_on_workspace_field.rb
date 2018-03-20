resource_name :enable_datagrid_on_workspace_field

property :workspace_name, String
property :field_name, String
property :sqlps_module_path, String
property :sql_server_name, String
property :sql_server_username, String
property :sql_server_password, String

actions :start
default_action :start

action :start do

    powershell_script 'Enable DataGrid on Workspace Field' do
        code <<-EOH
        Import-Module "#{sqlps_module_path}"
        Invoke-Sqlcmd -username '#{sql_server_username}' -password '#{sql_server_password}' -serverinstance '#{sql_server_name}' -Query "
        
        DECLARE @workspaceName nvarchar(max) = '#{workspace_name}'
        DECLARE @serverName nvarchar(max)
        DECLARE @workspaceArtifactID INT
        DECLARE @fieldName nvarchar(max) = '#{field_name}'
        DECLARE @sql NVARCHAR(MAX)
      
        SELECT
          @serverName = [ERS].[Name],
          @workspaceArtifactID = [EC].ArtifactID
        FROM
          [EDDS].[eddsdbo].[ExtendedCase] [EC] WITH(NOLOCK)
        INNER JOIN
          [EDDS].[eddsdbo].[ExtendedResourceServer] [ERS] WITH(NOLOCK) ON [EC].ServerID = [ERS].[ArtifactID]
        WHERE
          [EC].Name = @workspaceName
      
            SET @sql = '
              DECLARE @fieldArtifactID INT
              SET @fieldArtifactID = (SELECT TOP 1 [ArtifactID] FROM [' +@serverName +'].[edds' +CAST(@workspaceArtifactID AS NVARCHAR(MAX)) +'].[eddsdbo].[ExtendedField] WHERE [DisplayName] =''' +@fieldName +'''  AND [FieldArtifactTypeID] = 10)
              IF @fieldArtifactID > 0
                  BEGIN
                      UPDATE [' +@serverName +'].[edds' +CAST(@workspaceArtifactID AS NVARCHAR(MAX)) +'].[eddsdbo].[Field] set [EnableDataGrid] = 1 where [DisplayName] =''' +@fieldName +''' AND [FieldArtifactTypeID] = 10
                      INSERT INTO [' +@serverName +'].[edds' +CAST(@workspaceArtifactID AS NVARCHAR(MAX)) +'].[eddsdbo].[FieldMapping]([FieldArtifactID], [DataGridFieldName], [DataGridFieldNamespace]) VALUES(@fieldArtifactID, ''' +REPLACE(@fieldName, ' ','') +''',''Fields'')
                  END'
              EXECUTE sp_executesql @sql
        "
        EOH
    end
end

action_class do
  def whyrun_supported?
    true
  end
end
