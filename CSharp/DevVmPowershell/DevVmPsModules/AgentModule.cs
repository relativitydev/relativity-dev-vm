using DevVmPsModules.CustomExceptions;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Get, "Salutation")]
	public class AgentModule : BaseModule
	{
		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Name of the Relativity Instance")]
		public string RelativityInstanceName { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Username of the Relativity Admin")]
		public string RelativityAdminUsername { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Password of the Relativity Admin")]
		public string RelativityAdminPassword { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Comma separated list of Relativity Application Guid to create agents")]
		public string[] ApplicationGuids { get; set; }

		private List<Guid> ApplicationGuidList { get; set; }
		private const string Protocol = "http";
		private readonly Uri _relativityServicesUri;
		private readonly Uri _relativityRestUri;
		private readonly UsernamePasswordCredentials _usernamePasswordCredentials;
		private readonly ServiceFactorySettings _serviceFactorySettings;
		private ServiceFactory _serviceFactory;


		public AgentModule()
		{
			_relativityServicesUri = new Uri($"{Protocol}://{RelativityInstanceName}/Relativity.Services");
			_relativityRestUri = new Uri($"{Protocol}://{RelativityInstanceName}/Relativity.Rest/Api");
			_usernamePasswordCredentials = new UsernamePasswordCredentials(
			 username: RelativityAdminUsername,
			 password: RelativityAdminPassword);
			_serviceFactorySettings = new ServiceFactorySettings(
			 relativityServicesUri: _relativityServicesUri,
			 relativityRestUri: _relativityRestUri,
			 credentials: _usernamePasswordCredentials);
			_serviceFactory = new ServiceFactory(
			 settings: _serviceFactorySettings);
		}

		protected override async Task ProcessRecordAsync()
		{
			await Task.Run(() =>
			{
				//foreach (string name in RelativityInstanceName)
				//{
				//	WriteVerbose("Creating salutation for " + name);
				//	string salutation = "Hello, " + name;
				//	WriteObject(salutation);
				//}
			});
		}

		private List<Guid> ParseRelativityApplicationGuids()
		{
			try
			{
				List<Guid> applicationGuids = ApplicationGuids.Select(applicationGuid => new Guid(applicationGuid)).ToList();
				return applicationGuids;
			}
			catch (Exception ex)
			{
				throw new DevVmPowerShellModuleException($"An error occured when parsing Relativity Application Guids. [{nameof(ApplicationGuids)}: {ApplicationGuids}]", ex);
			}
		}

		private void CreateAgents()
		{
			try
			{

				List<Guid> relativityApplicationGuids = ParseRelativityApplicationGuids();
				//int agentResourceServerArtifactId = RetrieveAgentResourceServerArtifactId();

				////Create agents in Relativity Application
				//foreach (Guid currentRelativityApplicationGuid in relativityApplicationGuids)
				//{
				//	int resourceServerArtifactId = agentResourceServerArtifactId;
				//	CreateAgentsInRelativityApplication(
				//		agentResourceServerArtifactId: resourceServerArtifactId,
				//		relativityApplicationGuid: currentRelativityApplicationGuid);
				//}
			}
			catch (Exception ex)
			{
				throw new DevVmPowerShellModuleException("An error occured when Creating Agents in a Relativity Application.", ex);
			}
		}

		private void CreateAgentsInRelativityApplication(int agentResourceServerArtifactId, Guid relativityApplicationGuid)
		{
			try
			{
				//List<AgentModel> agentsToCreate = RetrieveAgentsToCreateInRelativityApplication(relativityApplicationGuid);

				//foreach (AgentModel agentModel in agentsToCreate)
				//{
				//	bool doesAgentExists = CheckIfAgentExists(agentModel.AgentName).Result;

				//	if (doesAgentExists)
				//	{
				//		Console.WriteLine($"Agents Exists - Skipped Creation. [{nameof(agentModel.AgentName)}: {agentModel.AgentName}]");
				//	}
				//	else
				//	{
				//		CreateAgentAsync(
				//			agentName: agentModel.AgentName,
				//			agentTypeId: agentModel.AgentTypeId,
				//			agentResourceServerArtifactId: agentResourceServerArtifactId).Wait();
				//	}
				//}


			}
			catch (Exception ex)
			{
				throw new DevVmPowerShellModuleException("An error occured when creating agents in Relativity Application.", ex);
			}
		}
	}
}
