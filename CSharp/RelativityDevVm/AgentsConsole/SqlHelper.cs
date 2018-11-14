using AgentsConsole.CustomExceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AgentsConsole
{
	public class SqlHelper
	{
		private readonly string _sqlDatabaseServerName;
		private readonly string _sqlUsername;
		private readonly string _sqlPassword;

		public SqlHelper(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
		{
			_sqlDatabaseServerName = sqlDatabaseServerName;
			_sqlUsername = sqlUsername;
			_sqlPassword = sqlPassword;
		}

		public int RetrieveResourceServerArtifactId(int agentResourceServerTypeArtifactId)
		{
			Console.WriteLine("Retrieving ResourceServerArtifactId");
			string sqlConnectionString = $"Data Source={_sqlDatabaseServerName};Initial Catalog=EDDS;User Id={_sqlUsername};Password={_sqlPassword};";
			SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
			try
			{
				string sql = $"SELECT TOP 1 [ArtifactID] FROM [EDDSDBO].[ResourceServer] WITH(NOLOCK) WHERE [Type] = {agentResourceServerTypeArtifactId}";
				SqlCommand sqlCommand = new SqlCommand
				{
					Connection = sqlConnection,
					CommandText = sql
				};
				sqlConnection.Open();
				int resourceServerArtifactId = (int)sqlCommand.ExecuteScalar();
				Console.WriteLine($"{nameof(resourceServerArtifactId)}: {resourceServerArtifactId}");
				return resourceServerArtifactId;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when retrieving ResourceServerArtifactId", ex);
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		public int RetrieveResourceServerTypeArtifactId(string resourceServerType)
		{
			Console.WriteLine("Retrieving ResourceServerTypeArtifactId");
			string sqlConnectionString = $"Data Source={_sqlDatabaseServerName};Initial Catalog=EDDS;User Id={_sqlUsername};Password={_sqlPassword};";
			SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
			try
			{
				string sql = $"SELECT TOP 1 [ArtifactID] FROM [EDDSDBO].[SystemArtifact] WITH(NOLOCK) WHERE [SystemArtifactIdentifier] = '{resourceServerType}'";
				SqlCommand sqlCommand = new SqlCommand
				{
					Connection = sqlConnection,
					CommandText = sql
				};
				sqlConnection.Open();
				int resourceServerTypeArtifactId = (int)sqlCommand.ExecuteScalar();
				Console.WriteLine($"{nameof(resourceServerTypeArtifactId)}: {resourceServerTypeArtifactId }");
				return resourceServerTypeArtifactId;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when retrieving ResourceServerTypeArtifactId", ex);
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		public List<string> RetrieveAgentNamesInRelativityApplication(Guid relativityApplicationGuid)
		{
			Console.WriteLine("Retrieving AgentNames In RelativityApplication");
			string sqlConnectionString = $"Data Source={_sqlDatabaseServerName};Initial Catalog=EDDS;User Id={_sqlUsername};Password={_sqlPassword};";
			SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
			try
			{
				List<string> agentNames = new List<string>();
				string sql = $"SELECT [Treeview] FROM [EDDSDBO].[LibraryApplication] WITH(NOLOCK) WHERE [GUID] = '{relativityApplicationGuid}'";
				SqlCommand sqlCommand = new SqlCommand
				{
					Connection = sqlConnection,
					CommandText = sql
				};
				sqlConnection.Open();
				string treeViewJson = (string)sqlCommand.ExecuteScalar();
				JObject treeViewJObject = JObject.Parse(treeViewJson);
				JToken treeViewChildren = treeViewJObject["children"];
				foreach (JToken treeViewChild in treeViewChildren)
				{
					if (treeViewChild["data"].ToString().Equals("Agent Types"))
					{
						JToken agentTypesChildren = treeViewChild["children"];
						foreach (JToken agentTypesChild in agentTypesChildren)
						{
							string agentName = agentTypesChild["data"].ToString();
							agentNames.Add(agentName);
						}
					}
				}

				Console.WriteLine($"{nameof(agentNames)}: {String.Join("; ", agentNames)}");
				return agentNames;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException($"An error occured when retrieving Agent Names in Relativity Application. [{nameof(relativityApplicationGuid)} = {relativityApplicationGuid}]", ex);
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		public int RetrieveAgentTypeId(string agentName)
		{
			Console.WriteLine("Retrieving AgentTypeId");
			string sqlConnectionString = $"Data Source={_sqlDatabaseServerName};Initial Catalog=EDDS;User Id={_sqlUsername};Password={_sqlPassword};";
			SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
			try
			{
				string sql = $"SELECT * FROM [EDDSDBO].[AgentType] WITH(NOLOCK) WHERE [Name] = '{agentName}'";
				SqlCommand sqlCommand = new SqlCommand
				{
					Connection = sqlConnection,
					CommandText = sql
				};
				sqlConnection.Open();
				int agentTypeId = (int)sqlCommand.ExecuteScalar();
				Console.WriteLine($"{nameof(agentTypeId)}: {agentTypeId}");
				return agentTypeId;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException($"An error occured when retrieving AgentTypeId [{nameof(agentName)} = {agentName}]", ex);
			}
			finally
			{
				sqlConnection.Close();
			}
		}
	}
}
