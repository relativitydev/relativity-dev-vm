using AgentsConsole.Models;
using AgentUtilities;
using Newtonsoft.Json.Linq;
using Relativity.Services.Agent;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AgentsConsole
{
    public class Program
    {
        private static readonly Guid ImagingApplicationGuid = new Guid("C9E4322E-6BD8-4A37-AE9E-C3C9BE31776B");

        public static void Main(string[] args)
        {
            //Constants
            const bool enableAgent = true;
            const int agentInterval = 5;
            const Agent.LoggingLevelEnum agentLoggingLevel = Agent.LoggingLevelEnum.All;
            const string protocal = "http";
            const string sqlDatabaseName = "EDDS";

            string serverName = args[0];
            string adminUsername = args[1];
            string adminPassword = args[2];
            string sqlDatabaseServerName = args[3];
            string sqlUsername = args[4];
            string sqlPassword = args[5];
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
                    sqlDatabaseName: sqlDatabaseName,
                    sqlUsername: sqlUsername,
                    sqlPassword: sqlPassword);

                int agentServerId = RetrieveAgentServerId(sqlDatabaseServerName, sqlUsername, sqlPassword);

                List<AgentModel> imagingAgentsToCreate = RetrieveImagingApplicationAgentsToCreate(sqlDatabaseServerName, sqlUsername, sqlPassword);

                foreach (AgentModel agentModel in imagingAgentsToCreate)
                {
                    //Create Agent
                    CreateAgentAsync(
                        agentHelper: agentHelper,
                        agentName: agentModel.AgentName,
                        agentTypeId: agentModel.AgentTypeId,
                        agentServerId: agentServerId,
                        enableAgent: enableAgent,
                        agentInterval: agentInterval,
                        agentLoggingLevel: agentLoggingLevel).Wait();
                }
            }
        }

        private static async Task<int> CreateAgentAsync(IAgentHelper agentHelper, string agentName, int agentTypeId, int agentServerId, bool enableAgent, int agentInterval, Agent.LoggingLevelEnum agentLoggingLevel)
        {
            Console.WriteLine($"Creating Agent [{nameof(agentName)} = {agentName}]");
            int newAgentArtifactId = await agentHelper.CreateAgentAsync(
                agentName: agentName,
                agentTypeId: agentTypeId,
                agentServer: agentServerId,
                enableAgent: enableAgent,
                agentInterval: agentInterval,
                agentLoggingLevel: agentLoggingLevel);
            Console.WriteLine($"Agent Created. [{nameof(agentName)} = {agentName}, {nameof(newAgentArtifactId)} = {newAgentArtifactId}]");
            return newAgentArtifactId;
        }

        private static int RetrieveAgentServerId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            int agentResourceServerTypeArtifactId = RetrieveAgentResourceServerTypeId(sqlDatabaseServerName, sqlUsername, sqlPassword);

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

        private static int RetrieveAgentResourceServerTypeId(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            string sqlConnectionString = $"Data Source={sqlDatabaseServerName};Initial Catalog=EDDS;User Id={sqlUsername};Password={sqlPassword};";
            SqlConnection sqlConnection = new SqlConnection(sqlConnectionString);
            try
            {
                string sql = "SELECT TOP 1 [ArtifactID] FROM [EDDSDBO].[SystemArtifact] WITH(NOLOCK) WHERE [SystemArtifactIdentifier] = 'AgentResourceServerType'";
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

        private static List<AgentModel> RetrieveImagingApplicationAgentsToCreate(string sqlDatabaseServerName, string sqlUsername, string sqlPassword)
        {
            List<AgentModel> agentsToCreate = new List<AgentModel>();
            List<string> agentNames = RetrieveAgentNamesInRelativityApplication(sqlDatabaseServerName, sqlUsername, sqlPassword, ImagingApplicationGuid);
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
