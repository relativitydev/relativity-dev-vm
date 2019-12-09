using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Helpers.Interfaces;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Utility;
using kCura.Vendor.Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Relativity.API;
using Relativity.Services.ServiceProxy;
using Field = kCura.Relativity.Client.Field;

namespace Helpers.Implementations
{
	public class SmokeTestHelper : ISmokeTestHelper
	{
		private ServiceFactory ServiceFactory { get; }
		public SmokeTestHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		public bool WaitForSmokeTestToComplete(string workspaceName)
		{
			try
			{
				bool completed = false;
				bool hasFailingTests = false;
				const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
				const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
				int currentWaitTimeInMilliseconds = 0;
				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = -1;
					Console.WriteLine("Querying for Workspace Artifact Id");
					int workspaceArtifactId = QueryForWorkspaceArtifactId(workspaceName, rsapiClient);
					Console.WriteLine("Queried for Workspace Artifact Id");

					rsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
					while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && completed == false)
					{
						Thread.Sleep(sleepTimeInMilliSeconds);

						Console.WriteLine("Querying for Smoke Test RDOs");
						var queryResultSet = QueryForTestRDOs(rsapiClient);
						Console.WriteLine("Queried for Smoke Test RDOs");

						int numberSuccess = 0;
						int numberFail = 0;
						foreach (Result<RDO> result in queryResultSet.Results)
						{
							string status = result.Artifact.Fields.Get(new Guid(Constants.SmokeTest.Guids.Fields.Status_FixedLengthText)).ValueAsFixedLengthText;
							if (status.Contains("Success"))
							{
								numberSuccess++;
							}
							if (status.Contains("Fail"))
							{
								numberFail++;
								hasFailingTests = true;
								completed = true;
								Console.WriteLine("Failing Test Found");
								break;
							}
						}

						if (queryResultSet.Results.Count > 0)
						{
							if ((numberSuccess + numberFail) == queryResultSet.Results.Count)
							{
								completed = true;
							}
						}

						currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
					}
				}

				return completed && !hasFailingTests;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred Waiting for the Smoke Test to Complete", ex);
			}
		}

		private static int QueryForWorkspaceArtifactId(string workspaceName, IRSAPIClient rsapiClient)
		{
			Query<Workspace> workspaceQuery = new Query<Workspace>();
			workspaceQuery.Condition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
			QueryResultSet<Workspace> workspaceQueryResultSet = rsapiClient.Repositories.Workspace.Query(workspaceQuery);
			if (!workspaceQueryResultSet.Success)
			{
				throw new Exception("Failed to Query for Workspace");
			}

			int workspaceArtifactId = workspaceQueryResultSet.Results.First().Artifact.ArtifactID;
			return workspaceArtifactId;
		}

		private static QueryResultSet<RDO> QueryForTestRDOs(IRSAPIClient rsapiClient)
		{
			Query<RDO> query = new Query<RDO>();
			query.ArtifactTypeGuid = new Guid(Constants.SmokeTest.Guids.TestObjectType);
			query.Fields = new List<FieldValue>
			{
				new FieldValue(new Guid(Constants.SmokeTest.Guids.Fields.Name_FixedLengthText)),
				new FieldValue(new Guid(Constants.SmokeTest.Guids.Fields.Status_FixedLengthText))
			};

			QueryResultSet<RDO> queryResultSet = rsapiClient.Repositories.RDO.Query(query);
			if (!queryResultSet.Success)
			{
				throw new Exception("Unable to query for Smoke Test Test Objects");
			}

			return queryResultSet;
		}
	}
}
