using DbContextHelper;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Helpers
{
	public class SqlHelper : ISqlHelper
	{
		private IDBContext DbContext { get; set; }
		private ConnectionHelper connectionHelper { get; set; }


		public SqlHelper(string relativityInstanceName, string sqlUserName, string sqlPassword)
		{
			connectionHelper = new ConnectionHelper(relativityInstanceName, sqlUserName, sqlPassword, Constants.Connection.Sql.ConnectionString_DefaultDatabase);

			DbContext = new DbContext(relativityInstanceName, Constants.Connection.Sql.ConnectionString_DefaultDatabase, sqlUserName, sqlPassword);
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

		public int GetErrorsCount()
		{
			string sqlErrorCount = "SELECT COUNT(*) [ArtifactID] FROM [EDDS].[eddsdbo].[Error]";
			try
			{
				int errorCount = DbContext.ExecuteSqlStatementAsScalar<int>(sqlErrorCount);
				return errorCount;
			}
			catch (Exception ex)
			{
				throw new Exception("Error Getting Error Count", ex);
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

		/// <summary>
		/// This only creates or alters the ShrinkDb procedure in the EDDS database.  Alter if it already exists, create if it does not
		/// </summary>
		/// <returns></returns>
		public bool CreateOrAlterShrinkDbProc()
		{
			bool wasSuccessful = false;
			Console.WriteLine($"{nameof(CreateOrAlterShrinkDbProc)} - Creating or Altering {Constants.SqlScripts.ShrinkDbProcName}");

			string sql = Constants.SqlScripts.CreateOrAlterShrinkDbProc;

			if (DoesSqlObjectExist(Constants.SqlScripts.SchemaName, Constants.SqlScripts.ShrinkDbProcName))
			{
				Console.WriteLine($"{nameof(CreateOrAlterShrinkDbProc)} - {Constants.SqlScripts.ShrinkDbProcName} exists so changing statement to ALTER");
				sql = sql.Replace("CREATE PROCEDURE", "ALTER PROCEDURE");
			}

			try
			{
				DbContext.ExecuteNonQuerySQLStatement(sql);
				wasSuccessful = true;
				Console.WriteLine($"{nameof(CreateOrAlterShrinkDbProc)} - Successfully updated {Constants.SqlScripts.ShrinkDbProcName}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{nameof(CreateOrAlterShrinkDbProc)} - Failed to create or alter {Constants.SqlScripts.ShrinkDbProcName}");
				Console.WriteLine(ex);
			}

			return wasSuccessful;
		}

		/// <summary>
		/// This will create and alter the ShrinkDb procedure before attempting to run.
		/// </summary>
		/// <returns></returns>
		public bool RunShrinkDbProc()
		{
			bool wasSuccessful = false;
			Console.WriteLine($"{nameof(RunShrinkDbProc)} - About to Run {Constants.SqlScripts.ShrinkDbProcName}");

			try
			{
				bool doesShrinkDbExist = CreateOrAlterShrinkDbProc();

				if (doesShrinkDbExist)
				{
					using (SqlConnection connection = connectionHelper.GetSqlConnection(Constants.Connection.Sql.ConnectionString_ConnectTimeoutLong))
					{
						Console.WriteLine($"{nameof(RunShrinkDbProc)} - Running {Constants.SqlScripts.ShrinkDbProcName}...");
						SqlCommand command = new SqlCommand(Constants.SqlScripts.ExecuteShringDbProc, connection);
						command.CommandTimeout = connection.ConnectionTimeout;
						command.Connection.Open();
						command.ExecuteNonQuery();
					}

					wasSuccessful = true;
					Console.WriteLine($"{nameof(RunShrinkDbProc)} - Successfully ran {Constants.SqlScripts.ShrinkDbProcName}");
				}
				else
				{
					Console.WriteLine($"{nameof(RunShrinkDbProc)} - Failed to run {Constants.SqlScripts.ShrinkDbProcName} as it could not be created properly");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{nameof(RunShrinkDbProc)} - Failed to run {Constants.SqlScripts.ShrinkDbProcName}");
				Console.WriteLine(ex);
			}

			return wasSuccessful;
		}

		private bool DoesSqlObjectExist(string schema, string objectName)
		{
			bool exists = false;
			string sql = Constants.SqlScripts.DoesExist.Replace("@@schema", schema).Replace("@@objectName", objectName);

			Console.WriteLine($"{nameof(DoesSqlObjectExist)} - Checking if object exists {Constants.SqlScripts.ShrinkDbProcName}");

			try
			{
				int queryResult = DbContext.ExecuteSqlStatementAsScalar<int>(sql);

				if (queryResult > 0)
				{
					exists = true;
					Console.WriteLine($"{nameof(DoesSqlObjectExist)} - {Constants.SqlScripts.ShrinkDbProcName} exists");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"{nameof(DoesSqlObjectExist)} - {Constants.SqlScripts.ShrinkDbProcName} does not exists");
				Console.WriteLine(ex);
			}

			return exists;
		}

		public void InsertRSMFViewerOverride()
		{
			bool wasSuccessful = false;
			try
			{
				string sqlDeleteFromTable = "DELETE FROM [EDDS].[eddsdbo].[Toggle] WHERE [Name] = 'Relativity.DocumentViewer.Toggle.ShowShortMessageFilesInViewerOverride'";
				string sqlInsertIntoTable = "INSERT INTO [EDDS].[eddsdbo].[Toggle] (Name, IsEnabled) VALUES ('Relativity.DocumentViewer.Toggle.ShowShortMessageFilesInViewerOverride', 1)";

				DbContext.ExecuteNonQuerySQLStatement(sqlDeleteFromTable);
				int rowsAffected = DbContext.ExecuteNonQuerySQLStatement(sqlInsertIntoTable);
				if (rowsAffected != 1)
				{
					throw new Exception("Failed to Insert ShowShortMessageFilesInViewerOverride to Database");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error Inserting ShowShortMessageFilesInViewerOverride", ex);
			}
		}
	}
}