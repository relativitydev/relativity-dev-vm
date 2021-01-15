using Helpers.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Relativity.Services.ServiceProxy;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class SmokeTestHelper : ISmokeTestHelper
	{
		private ServiceFactory ServiceFactory { get; }
		private IRestHelper RestHelper { get; }
		private IRetryLogicHelper RetryLogicHelper { get; }
		private IWorkspaceHelper WorkspaceHelper { get; }
		private string InstanceAddress { get; }
		private string AdminUsername { get; }
		private string AdminPassword { get; }

		public SmokeTestHelper(IConnectionHelper connectionHelper, IRestHelper restHelper, IRetryLogicHelper retryLogicHelper, IWorkspaceHelper workspaceHelper, string instanceAddress, string adminUsername, string adminPassword)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
			RestHelper = restHelper;
			RetryLogicHelper = retryLogicHelper;
			WorkspaceHelper = workspaceHelper;
			InstanceAddress = instanceAddress;
			AdminUsername = adminUsername;
			AdminPassword = adminPassword;
		}

		//public async Task<bool> WaitForSmokeTestToCompleteAsync(string workspaceName, int timeoutValueInMinutes)
		//{
		//	try
		//	{
		//		bool completed = false;
		//		bool hasFailingTests = false;
		//		int maxTimeInMilliseconds = timeoutValueInMinutes * 60 * 1000;
		//		const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
		//		int currentWaitTimeInMilliseconds = 0;

		//		Console.WriteLine("Querying for Workspace Artifact Id");
		//		int workspaceArtifactId = await WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);
		//		Console.WriteLine("Queried for Workspace Artifact Id");

		//		while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && completed == false)
		//		{
		//			Thread.Sleep(sleepTimeInMilliSeconds);

		//			Console.WriteLine("Querying for Smoke Test RDOs");
		//			var queryResultSet = QueryForTestRDOs(workspaceArtifactId);
		//			Console.WriteLine("Queried for Smoke Test RDOs");

		//			int numberSuccess = 0;
		//			int numberFail = 0;
		//			foreach (Result<RDO> result in queryResultSet.Results)
		//			{
		//				string status = result.Artifact.Fields.Get(new Guid(Constants.SmokeTest.Guids.Fields.Status_FixedLengthText)).ValueAsFixedLengthText;
		//				if (status.Contains("Success"))
		//				{
		//					numberSuccess++;
		//				}
		//				if (status.Contains("Fail"))
		//				{
		//					numberFail++;
		//					hasFailingTests = true;
		//					completed = true;
		//					string testName = result.Artifact.Fields.Get(new Guid(Constants.SmokeTest.Guids.Fields.Name_FixedLengthText)).ValueAsFixedLengthText;
		//					string errorDetails = result.Artifact.Fields.Get(new Guid(Constants.SmokeTest.Guids.Fields.ErrorDetails_LongText)).ValueAsLongText;
		//					Console.WriteLine($"Failing Test Found: {testName}");
		//					Console.WriteLine($"Error Details: {errorDetails}");
		//					break;
		//				}
		//			}

		//			if (queryResultSet.Results.Count > 0)
		//			{
		//				if ((numberSuccess + numberFail) == queryResultSet.Results.Count)
		//				{
		//					completed = true;
		//				}
		//			}

		//			currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
		//		}
		//		//}

		//		return completed && !hasFailingTests;
		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception("An error occurred Waiting for the Smoke Test to Complete", ex);
		//	}
		//}

		public async Task<bool> WaitForSmokeTestToCompleteAsync(string workspaceName, int timeoutValueInMinutes)
		{
			try
			{
				int workspaceId = await WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);
				int smokeTestObjectTypeId = await GetSmokeTestObjectTypeIdAsync(workspaceId);

				HttpResponseMessage queryResponse = await QueryForSmokeTestRdos(workspaceId, smokeTestObjectTypeId);

				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				//objectTypeId = result.Objects[0]["Values"][1];

				var test = result.Objects;

				return test != null;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred Waiting for the Smoke Test to Complete", ex);
			}
		}

		private async Task<int> GetSmokeTestObjectTypeIdAsync(int workspaceId)
		{
			int objectTypeId;
			try
			{
				string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceId.ToString());
				HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = Constants.OBJECT_TYPE_TYPE_ARTIFACT_ID },
						fields = new[]
						{
							new { Name = "Name" },
							new { Name = "Artifact Type ID" },
						},
						Condition = $"'Name' == '{Constants.SmokeTest.ObjectNames.SmokeTestApplicationObjectTypeName}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					throw new Exception("Failed to Query for Smoke Test Object Type");
				}

				string resultString = await queryResponse.Content.ReadAsStringAsync();
				//dynamic result = JObject.Parse(resultString) as JObject;
				dynamic queryResult = JsonConvert.DeserializeObject<dynamic>(resultString);
				//objectTypeId = result.Objects[0]["ArtifactID"];
				objectTypeId = queryResult.Objects.First.ArtifactID;
			}
			catch (Exception ex)
			{
				throw new Exception(
					$"Error Reading the {Constants.SmokeTest.ObjectNames.SmokeTestApplicationName} ObjectType", ex);
			}

			return objectTypeId;
		}

		//private QueryResultSet<RDO> QueryForTestRDOs(int workspaceArtifactId)
		//{
		//	Query<RDO> query = new Query<RDO>();
		//	query.ArtifactTypeGuid = new Guid(Constants.SmokeTest.Guids.TestObjectType);
		//	query.Fields = new List<FieldValue>
		//	{
		//		new FieldValue(new Guid(Constants.SmokeTest.Guids.Fields.Name_FixedLengthText)),
		//		new FieldValue(new Guid(Constants.SmokeTest.Guids.Fields.Status_FixedLengthText)),
		//		new FieldValue(new Guid(Constants.SmokeTest.Guids.Fields.Error_LongText)),
		//		new FieldValue(new Guid(Constants.SmokeTest.Guids.Fields.ErrorDetails_LongText))
		//	};

		//	QueryResultSet<RDO> queryResultSet = rsapiClient.Repositories.RDO.Query(query);
		//	if (!queryResultSet.Success)
		//	{
		//		throw new Exception("Unable to query for Smoke Test Test Objects");
		//	}

		//	return queryResultSet;
		//}

		private async Task<HttpResponseMessage> QueryForSmokeTestRdos(int workspaceId, int smokeTestObjectTypeId)
		{
			string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceId.ToString());
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);

			var queryPayloadObject = new
			{
				request = new
				{
					objectType = new { Guid = Constants.SmokeTest.Guids.TestObjectType },
					//objectType = new { artifactTypeId = Constants.OBJECT_TYPE_TYPE_ARTIFACT_ID },
					//objectType = new { artifactTypeId = smokeTestObjectTypeId },
					fields = new[]
					{
						//new { Name = "Name" },
						new { Guid = Constants.SmokeTest.Guids.Fields.Name_FixedLengthText },
						new { Guid = Constants.SmokeTest.Guids.Fields.Status_FixedLengthText },
						new { Guid = Constants.SmokeTest.Guids.Fields.Error_LongText },
						new { Guid = Constants.SmokeTest.Guids.Fields.ErrorDetails_LongText },
					},
				},
				start = 1,
				length = 25
			};

			string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
			HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
			if (!queryResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Query for Smoke Test RDO");
			}

			return queryResponse;
		}
	}
}
