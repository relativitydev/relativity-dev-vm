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
	}
}
