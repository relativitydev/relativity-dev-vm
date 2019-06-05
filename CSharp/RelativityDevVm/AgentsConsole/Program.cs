using AgentsConsole.CustomExceptions;
using AgentsConsole.Models;
using Newtonsoft.Json;
using Relativity.Services.Agent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AgentsConsole
{
	public class Program
	{
		public const char StringSplitter = ';';
		public const string AgentResourceServerType = "AgentResourceServerType";
		const bool EnableAgent = true;
		const int AgentInterval = 5;
		const Agent.LoggingLevelEnum AgentLoggingLevel = Agent.LoggingLevelEnum.All;
		const string Protocol = "http";
		public const string RelativityLibraryDirectoryPath = @"C:\Program Files\kCura Corporation\Relativity\ServiceHost";
		private static string _relativityServerName;
		private static string _relativityAdminUsername;
		private static string _relativityAdminPassword;
		private static string _sqlDatabaseServerName;
		private static string _sqlUsername;
		private static string _sqlPassword;
		private static string _relativityApplicationGuidsString;
		private static RestHelper _restHelper;
		private static SqlHelper _sqlHelper;

		public static void Main(string[] args)
		{
			CopyAndLoadAssemblies();
			CreateAgents(args);
		}

		private static void CopyAndLoadAssemblies()
		{
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				string executingAssemblyPath = executingAssembly.Location;
				if (File.Exists(executingAssemblyPath))
				{
					FileInfo executingAssemblyFileInfo = new FileInfo(executingAssemblyPath);
					DirectoryInfo executingAssemblyDirectoryInfo = executingAssemblyFileInfo.Directory;
					if (executingAssemblyDirectoryInfo != null && executingAssemblyDirectoryInfo.Exists)
					{
						DirectoryInfo relativityLibraryDirectoryInfo = new DirectoryInfo(RelativityLibraryDirectoryPath);
						if (relativityLibraryDirectoryInfo.Exists)
						{
							List<string> relativityAssemblies = new List<string>
								{
										"Relativity.Kepler.dll",
										"Relativity.Services.DataContracts.dll",
										"Relativity.Services.Interfaces.dll",
										"Relativity.Services.Interfaces.Private.dll",
										"Relativity.Services.ServiceProxy.dll"
								};

							foreach (string relativityAssembly in relativityAssemblies)
							{
								string sourceAssemblyPath = Path.Combine(RelativityLibraryDirectoryPath, relativityAssembly);
								string destinationAssemblyPath = Path.Combine(executingAssemblyDirectoryInfo.FullName, relativityAssembly);
								if (File.Exists(sourceAssemblyPath))
								{
									FileInfo sourceAssemblyFileInfo = new FileInfo(sourceAssemblyPath);
									if (!File.Exists(destinationAssemblyPath))
									{
										// Copy assembly file
										sourceAssemblyFileInfo.CopyTo(destinationAssemblyPath);
									}
									// Load assembly file
									Assembly.LoadFrom(destinationAssemblyPath);
								}
							}
						}
						else
						{
							string errorMessage = "Relativity Library Folder doesn't exist.";
							Console.WriteLine(errorMessage);
							throw new AgentsConsoleException(errorMessage);
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when Copying and Loading Assemblies.", ex);
			}
		}

		private static void CreateAgents(string[] args)
		{
			try
			{
				SetupProperties(args);
				List<Guid> relativityApplicationGuids = ParseRelativityApplicationGuids();
				int agentResourceServerArtifactId = RetrieveAgentResourceServerArtifactId();

				//vs in Relativity Application
				foreach (Guid currentRelativityApplicationGuid in relativityApplicationGuids)
				{
					int resourceServerArtifactId = agentResourceServerArtifactId;
					CreateAgentsInRelativityApplication(
							agentResourceServerArtifactId: resourceServerArtifactId,
							relativityApplicationGuid: currentRelativityApplicationGuid);
				}
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when Creating Agents in a Relativity Application.", ex);
			}
		}

		private static void SetupProperties(string[] args)
		{
			try
			{
				_relativityServerName = args[0];
				Console.WriteLine($"{nameof(_relativityServerName)}: {_relativityServerName}");
				_relativityAdminUsername = args[1];
				Console.WriteLine($"{nameof(_relativityAdminUsername)}: {_relativityAdminUsername}");
				_relativityAdminPassword = args[2];
				Console.WriteLine($"{nameof(_relativityAdminPassword)}: {_relativityAdminPassword}");
				_sqlDatabaseServerName = args[3];
				Console.WriteLine($"{nameof(_sqlDatabaseServerName)}: {_sqlDatabaseServerName}");
				_sqlUsername = args[4];
				Console.WriteLine($"{nameof(_sqlUsername)}: {_sqlUsername}");
				_sqlPassword = args[5];
				Console.WriteLine($"{nameof(_sqlPassword)}: {_sqlPassword}");
				_relativityApplicationGuidsString = args[6];
				Console.WriteLine($"{nameof(_relativityApplicationGuidsString)}: {_relativityApplicationGuidsString}");

				//Create Helper instances
				_restHelper = new RestHelper(_relativityAdminUsername, _relativityAdminPassword);
				_sqlHelper = new SqlHelper(_sqlDatabaseServerName, _sqlUsername, _sqlPassword);
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException(Constants.ErrorMessages.SetupPropertiesError, ex);
			}
		}

		private static List<Guid> ParseRelativityApplicationGuids()
		{
			try
			{
				List<Guid> retVal = new List<Guid>();
				List<string> relativityApplicationGuidsList = _relativityApplicationGuidsString.Split(StringSplitter).ToList();

				foreach (string currentGuidString in relativityApplicationGuidsList)
				{
					retVal.Add(new Guid(currentGuidString));
				}

				return retVal;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException($"An error occured when parsing Relativity Application Guids. [{nameof(_relativityApplicationGuidsString)}: {_relativityApplicationGuidsString}]", ex);
			}
		}

		private static void CreateAgentsInRelativityApplication(int agentResourceServerArtifactId, Guid relativityApplicationGuid)
		{
			try
			{
				List<AgentModel> agentsToCreate = RetrieveAgentsToCreateInRelativityApplication(relativityApplicationGuid);

				foreach (AgentModel agentModel in agentsToCreate)
				{
					bool doesAgentExists = CheckIfAgentExists(agentModel.AgentName).Result;

					if (doesAgentExists)
					{
						Console.WriteLine($"Agents Exists - Skipped Creation. [{nameof(agentModel.AgentName)}: {agentModel.AgentName}]");
					}
					else
					{
						CreateAgentAsync(
								agentName: agentModel.AgentName,
								agentTypeId: agentModel.AgentTypeId,
								agentResourceServerArtifactId: agentResourceServerArtifactId).Wait();
					}
				}
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when creating agents in Relativity Application.", ex);
			}
		}

		private static async Task<bool> CheckIfAgentExists(string agentName)
		{
			if (string.IsNullOrWhiteSpace(agentName))
			{
				throw new ArgumentException($"{nameof(agentName)} cannot be an empty string.");
			}

			Console.WriteLine($"Checking if Agent Exists. [{nameof(agentName)} = {agentName}]");
			try
			{
				string agentQueryUrl = $"{Protocol}://{_relativityServerName}/Relativity.Rest/api/Relativity.Services.Agent.IAgentModule/Agent Manager/QueryAsync";
				string agentQueryRequestJsonString = "{\"query\": {\"condition\": \"'Name' LIKE '" + agentName + "'\"}}";
				string restCallResultString = await _restHelper.PerformRestPostCallAsync(agentQueryUrl, agentQueryRequestJsonString);

				List<Agent> agents = new List<Agent>();
				try
				{
					AgentQueryResultSet agentQueryResultSet = JsonConvert.DeserializeObject<AgentQueryResultSet>(restCallResultString);
					agents.AddRange(agentQueryResultSet.Results.Select(x => x.Artifact));
				}
				catch (Exception ex)
				{
					throw new AgentsConsoleException(Constants.ErrorMessages.RestStringDeserializationError, ex);
				}

				if (agents.Count > 0)
				{
					Console.WriteLine($"Agent Exists. [{nameof(agentName)} = {agentName}, {nameof(agents)} = {agents}]");
					return true;
				}
				else
				{
					Console.WriteLine($"Agent doesn't Exists. [{nameof(agentName)} = {agentName}, {nameof(agents)} = {agents}]");
				}
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException($"An error occured when checking if Agent exists. [{nameof(agentName)} = {agentName}]", ex);
			}
			return false;
		}

		private static async Task<int> CreateAgentAsync(string agentName, int agentTypeId, int agentResourceServerArtifactId)
		{
			if (string.IsNullOrWhiteSpace(agentName))
			{
				throw new ArgumentException($"{nameof(agentName)} cannot be an empty string.");
			}
			if (agentTypeId < 1)
			{
				throw new ArgumentException($"{nameof(agentTypeId)} should be a positive number.");
			}
			if (agentResourceServerArtifactId < 1)
			{
				throw new ArgumentException($"{nameof(agentResourceServerArtifactId)} should be a positive number.");
			}

			Console.WriteLine($"Creating Agent. [{nameof(agentName)} = {agentName}]");
			try
			{
				string createAgentUrl = $"{Protocol}://{_relativityServerName}/Relativity.Rest/api/Relativity.Services.Agent.IAgentModule/Agent Manager/CreateSingleAsync";
				string createAgentRequestJsonString = "{ \"agentDTO\": {\"Name\": \"" + agentName + "\",\"AgentType\": {\"ArtifactID\": " + agentTypeId + "},\"Server\": {\"ArtifactID\": " + agentResourceServerArtifactId + "},\"Enabled\":" + EnableAgent.ToString().ToLower() + ",\"Interval\": " + AgentInterval + ",\"LoggingLevel\": " + (int)AgentLoggingLevel + "}}";

				string restCallResultString = await _restHelper.PerformRestPostCallAsync(createAgentUrl, createAgentRequestJsonString);

				int newAgentArtifactId;
				try
				{
					newAgentArtifactId = JsonConvert.DeserializeObject<int>(restCallResultString);
				}
				catch (Exception ex)
				{
					throw new AgentsConsoleException(Constants.ErrorMessages.RestStringDeserializationError, ex);
				}

				Console.WriteLine($"Agent Created. [{nameof(agentName)} = {agentName}, {nameof(newAgentArtifactId)} = {newAgentArtifactId}]");
				return newAgentArtifactId;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException($"An error occured when creating Agent. [{nameof(agentName)} = {agentName}]", ex);
			}
		}

		private static int RetrieveAgentResourceServerArtifactId()
		{
			Console.WriteLine("Retrieving AgentResourceServerArtifactId");
			try
			{
				int agentResourceServerTypeArtifactId = RetrieveAgentResourceServerTypeArtifactId();
				int agentResourceServerArtifactId = _sqlHelper.RetrieveResourceServerArtifactId(agentResourceServerTypeArtifactId);
				Console.WriteLine($"{nameof(agentResourceServerArtifactId)}: {agentResourceServerArtifactId}");
				return agentResourceServerArtifactId;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when retrieving AgentResourceServerArtifactId", ex);
			}
		}

		private static int RetrieveAgentResourceServerTypeArtifactId()
		{
			Console.WriteLine("Retrieving AgentResourceServerTypeArtifactId");
			try
			{
				int agentResourceServerTypeArtifactId = _sqlHelper.RetrieveResourceServerTypeArtifactId(AgentResourceServerType);
				Console.WriteLine($"{nameof(agentResourceServerTypeArtifactId)}: {agentResourceServerTypeArtifactId}");
				return agentResourceServerTypeArtifactId;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException("An error occured when retrieving AgentResourceServerTypeArtifactId", ex);
			}
		}

		private static List<AgentModel> RetrieveAgentsToCreateInRelativityApplication(Guid relativityApplicationGuid)
		{
			Console.WriteLine("Retrieving AgentsToCreate In RelativityApplication");
			try
			{
				List<AgentModel> agentsToCreate = new List<AgentModel>();
				List<string> agentNames = _sqlHelper.RetrieveAgentNamesInRelativityApplication(relativityApplicationGuid);
				foreach (string agentName in agentNames)
				{
					int agentTypeId = _sqlHelper.RetrieveAgentTypeId(agentName);
					agentsToCreate.Add(new AgentModel
					{
						AgentName = agentName,
						AgentTypeId = agentTypeId
					});
				}
				return agentsToCreate;
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException($"An error occured when retrieving Agents to Create in Relativity Application. [{nameof(relativityApplicationGuid)} = {relativityApplicationGuid}]", ex);
			}
		}
	}
}