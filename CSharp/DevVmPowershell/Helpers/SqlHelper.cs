using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbContextHelper;
using Relativity.API;

namespace Helpers
{
	public class SqlHelper : ISqlHelper
	{
		private IDBContext DbContext { get; set; }

		public SqlHelper(string relativityInstanceName, string sqlUserName, string sqlPassword)
		{
			DbContext = new DbContext(relativityInstanceName, "EDDS", sqlUserName, sqlPassword);
		}

		public bool DeleteAllErrors()
		{
		  string sqlSelectTopError = "SELECT TOP (1) [ArtifactID] FROM [EDDS].[eddsdbo].[Error]";
			string sqlDeleteFromArtifactGuidTable = "DELETE FROM [EDDS].[eddsdbo].[ArtifactGuid] WHERE [ArtifactID] = @artifactId";
			string sqlDeleteFromErrorTable = "DELETE FROM [EDDS].[eddsdbo].[Error] WHERE [ArtifactID] = @artifactID";
			string sqlDeleteFromArtifactAncestryTable = "DELETE FROM [EDDS].[eddsdbo].[ArtifactAncestry] WHERE [ArtifactID] = @artifactId";
			string sqlDeleteFromArtifactTable = "DELETE FROM [EDDS].[eddsdbo].[Artifact] WHERE [ArtifactID] = @artifactId";
			try
			{
				int? artifactId = DbContext.ExecuteSqlStatementAsScalar<int?>(sqlSelectTopError);
				if (artifactId.HasValue)
				{
					Console.WriteLine("Deleting Error Records");
				}
				else
				{
					Console.WriteLine("No Error Records to Delete");
					return false;
				}
				while (artifactId.HasValue)
				{
					List<SqlParameter> sqlParams = new List<SqlParameter>
					{
						new SqlParameter("@artifactId", SqlDbType.Int) {Value = artifactId.Value}
					};

					// Delete from all tables
					DbContext.ExecuteNonQuerySQLStatement(sqlDeleteFromArtifactGuidTable, sqlParams);
					DbContext.ExecuteNonQuerySQLStatement(sqlDeleteFromErrorTable, sqlParams);
					DbContext.ExecuteNonQuerySQLStatement(sqlDeleteFromArtifactAncestryTable, sqlParams);
					DbContext.ExecuteNonQuerySQLStatement(sqlDeleteFromArtifactTable, sqlParams);

					// Check to see if there is another error record
					artifactId = DbContext.ExecuteSqlStatementAsScalar<int?>(sqlSelectTopError);
				}

				Console.WriteLine("Error Records Deleted!");
				return true;
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to Delete all Errors from the Errors Tab");
			}
		}

		public int GetFileShareResourceServerArtifactId()
		{
			try
			{
				string sql = @"SELECT [ArtifactID] FROM [EDDS].[eddsdbo].[ResourceServer] WHERE [Name] = '\\RELATIVITYDEVVM\Fileshare\'";

				int? artifactId = DbContext.ExecuteSqlStatementAsScalar<int?>(sql);
				if (!artifactId.HasValue)
				{
					throw new Exception("Resource Server does not exist");
				}

				return artifactId.Value;
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to Get FileShare Resource Server ArtifactId", ex);
			}
		}

		public void EnableDataGridOnExtractedText(string workspaceName)
		{
			try
			{
				string sql = $@"DECLARE @workspaceName nvarchar(max) = '{workspaceName}'
        DECLARE @serverName nvarchar(max)
        DECLARE @workspaceArtifactID INT
        DECLARE @fieldName nvarchar(max) = 'Extracted Text'
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
              EXECUTE sp_executesql @sql";

				DbContext.ExecuteNonQuerySQLStatement(sql);
				Console.WriteLine("Data Grid Enabled for Extracted Text Field");
			}
			catch (Exception ex)
			{
				throw new Exception("Error Enabling Data Grid for the Extracted Text Field", ex);
			}
		}
	}
}
