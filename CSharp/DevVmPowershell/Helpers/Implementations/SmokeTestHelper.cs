using Helpers.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class SmokeTestHelper : ISmokeTestHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IRestHelper RestHelper { get; }
		private IRetryLogicHelper RetryLogicHelper { get; }
		private IWorkspaceHelper WorkspaceHelper { get; }

		public SmokeTestHelper(IConnectionHelper connectionHelper, IRestHelper restHelper, IRetryLogicHelper retryLogicHelper, IWorkspaceHelper workspaceHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
			RetryLogicHelper = retryLogicHelper;
			WorkspaceHelper = workspaceHelper;
		}

		public async Task<bool> WaitForSmokeTestToCompleteAsync(string workspaceName, int timeoutValueInMinutes)
		{
			try
			{
				int workspaceId = await WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);
				int smokeTestObjectTypeId = await RetryLogicHelper.RetryFunctionAsync<int>(Constants.Waiting.SMOKE_TEST_HELPER_RETRY_COUNT, Constants.Waiting.SMOKE_TEST_HELPER_SLEEP_TIME_IN_SECONDS,
					async () => await GetSmokeTestObjectTypeIdAsync(workspaceId));

				bool didTestsComplete = await RetryLogicHelper.RetryFunctionAsync<bool>(Constants.Waiting.SMOKE_TEST_HELPER_RETRY_COUNT, timeoutValueInMinutes * 60,
					async () =>
					{
						HttpResponseMessage queryResponse = await QueryForSmokeTestRdosAsync(workspaceId, smokeTestObjectTypeId);

						string resultString = await queryResponse.Content.ReadAsStringAsync();
						dynamic queryResult = JsonConvert.DeserializeObject<dynamic>(resultString);

						int totalTests = queryResult.TotalCount;
						int numberSuccess = 0;
						int numberFail = 0;
						bool completed = false;
						bool hasFailingTests = false;

						foreach (var test in queryResult.Objects)
						{
							string status = test.Values[1];
							if (status.Contains("Success"))
							{
								numberSuccess++;
							}
							if (status.Contains("Fail"))
							{
								numberFail++;
								hasFailingTests = true;
								completed = true;
								string testName = test.Values[0];
								string errorDetails = test.Values[3];
								Console.WriteLine($"Failing Test Found: {testName}");
								Console.WriteLine($"Error Details: {errorDetails}");
								break;
							}
						}

						if ((numberSuccess + numberFail) == totalTests && totalTests > 0)
						{
							completed = true;
						}
						else if (!completed)
						{
							throw new Exception("Smoke tests are not yet completed.");
						}

						return completed && !hasFailingTests;
					});

				return didTestsComplete;
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
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
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
				dynamic queryResult = JsonConvert.DeserializeObject<dynamic>(resultString);
				objectTypeId = queryResult.Objects.First.ArtifactID;
			}
			catch (Exception ex)
			{
				throw new Exception(
					$"Error Reading the {Constants.SmokeTest.ObjectNames.SmokeTestApplicationName} ObjectType", ex);
			}

			return objectTypeId;
		}

		private async Task<HttpResponseMessage> QueryForSmokeTestRdosAsync(int workspaceId, int smokeTestObjectTypeId)
		{
			string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceId.ToString());
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);

			var queryPayloadObject = new
			{
				request = new
				{
					objectType = new { Guid = Constants.SmokeTest.Guids.TestObjectType },
					fields = new[]
					{
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
