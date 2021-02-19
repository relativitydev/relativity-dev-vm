using Helpers.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class ImagingHelper : IImagingHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IRestHelper RestHelper { get; set; }
		private IRetryLogicHelper RetryLogicHelper { get; set; }

		public ImagingHelper(IConnectionHelper connectionHelper, IRestHelper restHelper, IRetryLogicHelper retryLogicHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
			RetryLogicHelper = retryLogicHelper;
		}

		public async Task ImageAllDocumentsInWorkspaceAsync(int workspaceArtifactId)
		{
			try
			{
				int savedSearchArtifactId = await CreateKeywordSearchAsync(workspaceArtifactId);
				int imagingProfileArtifactId = await CreateImagingProfileAsync(workspaceArtifactId);
				int imagingSetArtifactId = await CreateImagingSetAsync(workspaceArtifactId, savedSearchArtifactId, imagingProfileArtifactId);
				await RunImagingJobAsync(workspaceArtifactId, imagingSetArtifactId);
				await WaitForImagingJobToCompleteAsync(workspaceArtifactId, imagingSetArtifactId);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Imaging All Documents in the Workspace", ex);
			}
		}

		public async Task<int> CreateImagingProfileAsync(int workspaceArtifactId)
		{
			Console.WriteLine($"Creating Imaging Profile [Name: {Constants.Imaging.Profile.NAME}]");

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			var basicImagingProfile = new
			{
				imagingProfile = new
				{
					ImagingMethod = Constants.Imaging.Profile.IMAGING_METHOD,
					BasicOptions = new
					{
						ImageOutputDpi = Constants.Imaging.Profile.IMAGE_OUTPUT_DPI,
						BasicImageFormat = Constants.Imaging.Profile.BASIC_IMAGE_FORMAT,
						ImageSize = Constants.Imaging.Profile.IMAGE_SIZE
					},
					NativeTypes = new object[]
					{
					},
					ApplicationFieldCodes = new object[]
					{
					},
					Name = Constants.Imaging.Profile.NAME,
				},
				workspaceId = workspaceArtifactId
			};
			string request = JsonConvert.SerializeObject(basicImagingProfile);
			// Save the ImagingProfile. Successful saves returns the ArtifactID of the ImagingProfile.
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ImagingProfile.CreateEndpointUrl, request);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Create Imaging Profile");
			}
			int imagingProfileArtifactId = Convert.ToInt32(await response.Content.ReadAsStringAsync());

			Console.WriteLine("Created Imaging Profile!");

			return imagingProfileArtifactId;
		}

		public async Task<int> CreateImagingSetAsync(int workspaceArtifactId, int savedSearchArtifactId, int imagingProfileArtifactId)
		{
			Console.WriteLine($"Creating Imaging Set [Name: {Constants.Imaging.Set.NAME}]");

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			var imagingSet = new
			{
				imagingSet = new
				{
					imagingProfile = new
					{
						ArtifactID = imagingProfileArtifactId
					},
					DataSource = savedSearchArtifactId,
					EmailNotificationRecipients = Constants.Imaging.Set.EMAIL_NOTIFICATION_RECIPIENTS,
					Name = Constants.Imaging.Set.NAME
				},
				workspaceId = workspaceArtifactId
			};
			string request = JsonConvert.SerializeObject(imagingSet);
			// Save the ImagingSet. Successful saves return the ArtifactID of the ImagingSet.
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ImagingSet.CreateEndpointUrl, request);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Create Imaging Set");
			}
			int imagingSetArtifactId = Convert.ToInt32(await response.Content.ReadAsStringAsync());

			Console.WriteLine("Created Imaging Set!");

			return imagingSetArtifactId;
		}

		public async Task RunImagingJobAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			Console.WriteLine("Creating Imaging Job");

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			var imagingJob = new
			{
				imagingJob = new
				{
					imagingSetId = imagingSetArtifactId,
					workspaceId = workspaceArtifactId
				}
			};
			string request = JsonConvert.SerializeObject(imagingJob);
			//Run an ImagingSet job.
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.ImagingJob.RunImagingSetEndpointUrl, request);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Run Imaging Job");
			}

			Console.WriteLine("Created Imaging Job!");
		}

		public async Task WaitForImagingJobToCompleteAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			Console.WriteLine("Waiting for Imaging Job to finish");
			bool publishComplete = await JobCompletedSuccessfullyAsync(workspaceArtifactId, imagingSetArtifactId);
			if (!publishComplete)
			{
				throw new Exception("Imaging Job failed to Complete");
			}
			Console.WriteLine("Imaging Job Complete!");
		}

		private async Task<bool> JobCompletedSuccessfullyAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			bool jobComplete = false;
			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
					string url = Constants.Connection.RestUrlEndpoints.ObjectManager.ReadUrl.Replace("-1", workspaceArtifactId.ToString());
					var readPayloadObject = new
					{
						request = new
						{
							Object = new 
							{
								ArtifactID = imagingSetArtifactId
							},
							fields = new[]
							{
								new { Name = "Status"},
							},
						},
					};

					string readPayload = JsonConvert.SerializeObject(readPayloadObject);
					
					// Retry until we get the applicationId from the library
					string jobStatus = await RetryLogicHelper
					.RetryFunctionAsync<string>(Constants.Waiting.IMAGING_HELPER_RETRY_COUNT,
					Constants.Waiting.IMAGING_HELPER_RETRY_DELAY,
					async () => 
					{
						HttpResponseMessage readResponse = await RestHelper.MakePostAsync(httpClient, url, readPayload);
						if (!readResponse.IsSuccessStatusCode)
						{
							return null;
						}
						string resultString = await readResponse.Content.ReadAsStringAsync();
						dynamic result = JObject.Parse(resultString) as JObject;
						string status = result.Object["FieldValues"][0]["Value"].ToString();
						jobComplete = status.Contains("Completed");
						if (!jobComplete)
						{
							throw new Exception("Job not yet complete");
						}
						return status;
					});
					if (jobStatus == null)
					{
						throw new Exception("Failed to Read Imaging Set RDO");
					}

					return jobComplete;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error when checking for Imaging Job Completion. [ErrorMessage: {ex}]", ex);
			}
		}

		public async Task<int> CreateKeywordSearchAsync(int workspaceArtifactId)
		{
			Console.WriteLine("Creating Keyword search for DtSearch Index");

			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			string value = null;
			var keywordSearch = new
			{
				workspaceArtifactID = workspaceArtifactId,
				searchDTO = new
				{
					ArtifactTypeID = 10,
					Name = Constants.Search.KeywordSearch.NAME,
					SearchCriteria = new
					{
						Conditions = new object[]
						{
							new
							{
								Condition = new
								{
									Operator = "IsSet",
									FieldIdentifier = new
									{
										Name = Constants.Search.KeywordSearch.CONDITION_FIELD_EXTRACTED_TEXT
									},
									ConditionType = "Criteria",
									Value = value
								}
							}
						}
					},
					Fields = new object[]
					{
						new
						{
							Name = Constants.Search.KeywordSearch.FIELD_CONTROL_NUMBER
						}
					}
				}
			};
			string request = JsonConvert.SerializeObject(keywordSearch);
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, Constants.Connection.RestUrlEndpoints.KeywordSearch.CreateEndpointUrl, request);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Created a Keyword Search");
			}
			int keywordSearchArtifactId = Convert.ToInt32(await response.Content.ReadAsStringAsync());

			Console.WriteLine("Created Keyword search for DtSearch Index!");

			return keywordSearchArtifactId;
		}

		public async Task<bool> CheckThatAllDocumentsInWorkspaceAreImaged(int workspaceArtifactId)
		{
			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
				string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceArtifactId.ToString());
				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = 10 },
						fields = new[]
						{
							new { Name = "Has Images" }
						},
						Condition = "",
					},
					start = 0,
					length = 200
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					throw new Exception("Failed to Query for Documents");
				}
				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				int totalCount = result.TotalCount;
				bool allDocumentsHaveImages = true;
				for (int i = 0; i < totalCount; i++)
				{
					string hasImageValue = result.Objects[i]["Values"][0]["Name"];
					if (hasImageValue.Equals("No"))
					{
						allDocumentsHaveImages = false;
						break;
					}
				}

				return allDocumentsHaveImages;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when Checking that all documents in the Workspace are Imaged", ex);
			}
		}
	}
}
