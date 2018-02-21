using AgentsConsole.Models;
using AgentUtilities;
using Newtonsoft.Json.Linq;
using Relativity.Services.Agent;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public const string WebProcessingServerType = "WebBackgroundProcessingServerType";
        private static readonly Guid ImagingSetSchedulerApplicationGuid = new Guid("6BE2880A-D951-4A98-A6FE-4A84835D3D06");
        //private static readonly Guid ImagingApplicationGuid = new Guid("C9E4322E-6BD8-4A37-AE9E-C3C9BE31776B");
        //private static readonly Guid DocumentViewerApplicationGuid = new Guid("5725CAB5-EE63-4155-B227-C74CC9E26A76");
        //private static readonly Guid ProductionApplicationGuid = new Guid("51B19AB2-3D45-406C-A85E-F98C01B033EC");
        //private static readonly Guid ProcessingApplicationGuid = new Guid("ED0E23F9-DA60-4298-AF9A-AE6A9B6A9319");
        //private static readonly Guid SmokeTestApplicationGuid = new Guid("0125C8D4-8354-4D8F-B031-01E73C866C7C");

        public static void Main(string[] args)
        {
            LoadAssemblies();
            CreateAgents(args);
        }

        private static void LoadAssemblies()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            string executingAssemblyPath = executingAssembly.Location;
            if (File.Exists(executingAssemblyPath))
            {
                FileInfo executingAssemblyFileInfo = new FileInfo(executingAssemblyPath);
                DirectoryInfo executingAssemblyDirectoryInfo = executingAssemblyFileInfo.Directory;
                if (executingAssemblyDirectoryInfo != null && executingAssemblyDirectoryInfo.Exists)
                {
                    string relativityLibraryDirectoryPath = @"C:\Program Files\kCura Corporation\Relativity\Library";
                    DirectoryInfo relativityLibraryDirectoryInfo = new DirectoryInfo(relativityLibraryDirectoryPath);
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
                            string sourceAssemblyPath = Path.Combine(relativityLibraryDirectoryPath, relativityAssembly);
                            string destinationAssemblyPath = Path.Combine(executingAssemblyDirectoryInfo.FullName, relativityAssembly);
                            if (File.Exists(sourceAssemblyPath))
                            {
                                FileInfo sourceAssemblyFileInfo = new FileInfo(sourceAssemblyPath);
                                sourceAssemblyFileInfo.CopyTo(destinationAssemblyPath);
                                Assembly.LoadFrom(destinationAssemblyPath);
                            }
                        }
                    }
                    else
                    {
                        string errorMessage = "Relativity Library Folder doesn't exist.";
                        Console.WriteLine(errorMessage);
                        throw new Exception(errorMessage);
                    }
                }
            }
        }

        private static void CreateAgents(string[] args)
        {
            //Constants
            const bool enableAgent = true;
            const int agentInterval = 5;
            const Agent.LoggingLevelEnum agentLoggingLevel = Agent.LoggingLevelEnum.All;
            const string protocal = "http";
            const string eddsSqlDatabaseName = "EDDS";

            string serverName = args[0];
            string adminUsername = args[1];
            string adminPassword = args[2];
            string sqlDatabaseServerName = args[3];
            string sqlUsername = args[4];
            string sqlPassword = args[5];
            string relativityApplicationGuidsString = args[6];
            List<Guid> relativityApplicationGuids = ParseRelativityApplicationGuids(relativityApplicationGuidsString);
            Uri relativityServicesUri = new Uri($"{protocal}://{serverName}/Relativity.Services");
            Uri relativityRestUri = new Uri($"{protocal}://{serverName}/Relativity.Rest/Api");

            UsernamePasswordCredentials usernamePasswordCredentials = new UsernamePasswordCredentials(
                username: adminUsername,
                password: adminPassword);
            ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(
                relativityServicesUri: relativityServicesUri,
                relativityRestUri: relativityRestUri,
                credentials: usernamePasswordCredentials);
            ServiceFactory serviceFactory = new ServiceFactory(
                settings: serviceFactorySettings);

            using (IAgentManager agentManager = serviceFactory.CreateProxy<IAgentManager>())
            {
                IAgentHelper agentHelper = new AgentHelper(
                    agentManager: agentManager, sqlDatabaseServerName: sqlDatabaseServerName,
                    sqlDatabaseName: eddsSqlDatabaseName,
                    sqlUsername: sqlUsername,
                    sqlPassword: sqlPassword);

                int agentResourceServerArtifactId = RetrieveAgentResourceServerArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword);
                int webProcessingResourceServerArtifactId = RetrieveWebProcessingResourceServerArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword);

                //Create agents in Relativity Application
                foreach (Guid currentRelativityApplicationGuid in relativityApplicationGuids)
                {
                    int resourceServerArtifactId =
                        currentRelativityApplicationGuid == ImagingSetSchedulerApplicationGuid
                            ? webProcessingResourceServerArtifactId
                            : agentResourceServerArtifactId;
                    CreateAgentsInRelativityApplication(
                        sqlDatabaseServerName: sqlDatabaseServerName,
                        sqlUsername: sqlUsername,
                        sqlPassword: sqlPassword,
                        agentHelper: agentHelper,
                        agentResourceServerArtifactId: resourceServerArtifactId,
                        enableAgent: enableAgent,
                        agentInterval: agentInterval,
                        agentLoggingLevel: agentLoggingLevel,
                        relativityApplicationGuid: currentRelativityApplicationGuid);
                }
            }
        }

        private static List<Guid> ParseRelativityApplicationGuids(string relativityApplicationGuidsString)
        {
            List<Guid> retVal = new List<Guid>();
            List<string> relativityApplicationGuidsList = relativityApplicationGuidsString.Split(StringSplitter).ToList();

            foreach (string currentGuidString in relativityApplicationGuidsList)
            {
                retVal.Add(new Guid(currentGuidString));
            }

            return retVal;
        }

        private static void CreateAgentsInRelativityApplication(string sqlDatabaseServerName, string sqlUsername, string sqlPassword, IAgentHelper agentHelper, int agentResourceServerArtifactId, bool enableAgent, int agentInterval, Agent.LoggingLevelEnum agentLoggingLevel, Guid relativityApplicationGuid)
        {
            List<AgentModel> agentsToCreate = RetrieveAgentsToCreateInRelativityApplication(sqlDatabaseServerName, sqlUsername, sqlPassword, relativityApplicationGuid);

            foreach (AgentModel agentModel in agentsToCreate)
            {
                //Create Agent
                CreateAgentAsync(
                    agentHelper: agentHelper,
                    agentName: agentModel.AgentName,
                    agentTypeId: agentModel.AgentTypeId,
                    agentResourceServerArtifactId: agentResourceServerArtifactId,
                    enableAgent: enableAgent,
                    agentInterval: agentInterval,
                    agentLoggingLevel: agentLoggingLevel).Wait();
            }
        }

        private static async Task<int> CreateAgentAsync(IAgentHelper agentHelper, string agentName, int agentTypeId, int agentResourceServerArtifactId, bool enableAgent, int agentInterval, Agent.LoggingLevelEnum agentLoggingLevel)
        {
            Console.WriteLine($"Creating Agent [{nameof(agentName)} = {agentName}]");
            int newAgentArtifactId = await agentHelper.CreateAgentAsync(
                agentName: agentName,
                agentTypeId: agentTypeId,
                agentServer: agentResourceServerArtifactId,
                enableAgent: enableAgent,
                agentInterval: agentInterval,
                agentLoggingLevel: agentLoggingLevel);
            Console.WriteLine($"Agent Created. [{nameof(agentName)} = {agentName}, {nameof(newAgentArtifactId)} = {newAgentArtifactId}]");
            return newAgentArtifactId;
        }

        private static int RetrieveResourceServerArtifactId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword, int agentResourceServerTypeArtifactId)
        {
            string sqlConnectionString = $"Data Source={sqlDatabaseServerName};Initial Catalog=EDDS;User Id={sqlUsername};Password={sqlPassword};";
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
                int agentServerId = (int)sqlCommand.ExecuteScalar();
                return agentServerId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when retrieving AgentServerId", ex);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private static int RetrieveAgentResourceServerArtifactId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            int agentResourceServerTypeArtifactId = RetrieveAgentResourceServerTypeArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword);
            return RetrieveResourceServerArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword, agentResourceServerTypeArtifactId);
        }

        private static int RetrieveWebProcessingResourceServerArtifactId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            int webProcessingResourceServerTypeArtifactId = RetrieveWebProcessingResourceServerTypeArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword);
            return RetrieveResourceServerArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword, webProcessingResourceServerTypeArtifactId);
        }

        private static int RetrieveResourceServerTypeArtifactId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword, string resourceServerType)
        {
            string sqlConnectionString = $"Data Source={sqlDatabaseServerName};Initial Catalog=EDDS;User Id={sqlUsername};Password={sqlPassword};";
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
                int agentResourceServerTypeArtifactId = (int)sqlCommand.ExecuteScalar();
                return agentResourceServerTypeArtifactId;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured when retrieving AgentResourceServerTypeId", ex);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private static int RetrieveAgentResourceServerTypeArtifactId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            return RetrieveResourceServerTypeArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword, AgentResourceServerType);
        }

        private static int RetrieveWebProcessingResourceServerTypeArtifactId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            return RetrieveResourceServerTypeArtifactId(sqlDatabaseServerName, sqlUsername, sqlPassword, WebProcessingServerType);
        }

        private static List<AgentModel> RetrieveAgentsToCreateInRelativityApplication(string sqlDatabaseServerName, string sqlUsername, string sqlPassword, Guid relativityApplicationGuid)
        {
            List<AgentModel> agentsToCreate = new List<AgentModel>();
            List<string> agentNames = RetrieveAgentNamesInRelativityApplication(sqlDatabaseServerName, sqlUsername, sqlPassword, relativityApplicationGuid);
            foreach (string agentName in agentNames)
            {
                int agentTypeId = RetrieveAgentTypeId(sqlDatabaseServerName, sqlUsername, sqlPassword, agentName);
                agentsToCreate.Add(new AgentModel
                {
                    AgentName = agentName,
                    AgentTypeId = agentTypeId
                });
            }
            return agentsToCreate;
        }

        private static List<string> RetrieveAgentNamesInRelativityApplication(string sqlDatabaseServerName, string sqlUsername, string sqlPassword, Guid applicationGuid)
        {
            string sqlConnectionString = $"Data Source={sqlDatabaseServerName};Initial Catalog=EDDS;User Id={sqlUsername};Password={sqlPassword};";
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            try
            {
                List<string> agentNames = new List<string>();
                string sql = $"SELECT [Treeview] FROM [EDDSDBO].[LibraryApplication] WITH(NOLOCK) WHERE [GUID] = '{applicationGuid}'";
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

                return agentNames;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured when retrieving Agents in Relativity Application. [{nameof(applicationGuid)} = {applicationGuid}]", ex);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private static int RetrieveAgentTypeId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword, string agentName)
        {
            string sqlConnectionString = $"Data Source={sqlDatabaseServerName};Initial Catalog=EDDS;User Id={sqlUsername};Password={sqlPassword};";
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
                return agentTypeId;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occured when retrieving AgentTypeId [{nameof(agentName)} = {agentName}]", ex);
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
