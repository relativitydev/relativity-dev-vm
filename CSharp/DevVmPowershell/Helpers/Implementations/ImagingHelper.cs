using Helpers.Interfaces;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Imaging.Services.Interfaces;
using Relativity.Services.Exceptions;
using Relativity.Services.Field;
using Relativity.Services.Search;
using Relativity.Services.ServiceProxy;
using Relativity.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Helpers.Implementations
{
	public class ImagingHelper : IImagingHelper
	{
		private IImagingProfileManager ImagingProfileManager { get; }
		private IImagingSetManager ImagingSetManager { get; }
		private IImagingJobManager ImagingJobManager { get; }
		private IKeywordSearchManager KeywordSearchManager { get; }
		private ServiceFactory ServiceFactory { get; }
		private string InstanceAddress { get; }
		private string AdminUsername { get; }
		private string AdminPassword { get; }
		private IRestHelper RestHelper { get; set; }
		private IRetryLogicHelper RetryLogicHelper { get; set; }

		public ImagingHelper(IConnectionHelper connectionHelper, IRestHelper restHelper, IRetryLogicHelper retryLogicHelper, string instanceAddress, string adminUsername, string adminPassword)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
			ImagingProfileManager = ServiceFactory.CreateProxy<IImagingProfileManager>();
			ImagingSetManager = ServiceFactory.CreateProxy<IImagingSetManager>();
			ImagingJobManager = ServiceFactory.CreateProxy<IImagingJobManager>();
			KeywordSearchManager = ServiceFactory.CreateProxy<IKeywordSearchManager>();
			RestHelper = restHelper;
			RetryLogicHelper = retryLogicHelper;
			InstanceAddress = instanceAddress;
			AdminUsername = adminUsername;
			AdminPassword = adminPassword;
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

			try
			{
				ImagingProfile basicImagingProfile = new ImagingProfile
				{

					BasicOptions = new BasicImagingEngineOptions
					{
						ImageOutputDpi = Constants.Imaging.Profile.IMAGE_OUTPUT_DPI,
						BasicImageFormat = Constants.Imaging.Profile.BASIC_IMAGE_FORMAT,
						ImageSize = Constants.Imaging.Profile.IMAGE_SIZE
					},
					Name = Constants.Imaging.Profile.NAME,
					ImagingMethod = Constants.Imaging.Profile.IMAGING_METHOD
				};

				// Save the ImagingProfile. Successful saves returns the ArtifactID of the ImagingProfile.
				int imagingProfileArtifactId = await ImagingProfileManager.SaveAsync(basicImagingProfile, workspaceArtifactId);

				Console.WriteLine("Created Imaging Profile!");

				return imagingProfileArtifactId;
			}
			catch (ServiceException ex)
			{
				//The service throws an exception of type ServiceException, performs logging and rethrows the exception.
				throw new Exception("An error occurred when creating Imaging Profile", ex);
			}
		}

		public async Task<int> CreateImagingSetAsync(int workspaceArtifactId, int savedSearchArtifactId, int imagingProfileArtifactId)
		{
			Console.WriteLine($"Creating Imaging Set [Name: {Constants.Imaging.Set.NAME}]");

			try
			{
				ImagingSet imagingSet = new ImagingSet
				{
					DataSource = savedSearchArtifactId,
					Name = Constants.Imaging.Set.NAME,
					ImagingProfile = new ImagingProfileRef
					{
						ArtifactID = imagingProfileArtifactId
					},
					EmailNotificationRecipients = Constants.Imaging.Set.EMAIL_NOTIFICATION_RECIPIENTS
				};

				// Save the ImagingSet. Successful saves return the ArtifactID of the ImagingSet.
				int imagingSetArtifactId = await ImagingSetManager.SaveAsync(imagingSet, workspaceArtifactId);

				Console.WriteLine("Created Imaging Set!");

				return imagingSetArtifactId;
			}
			catch (ServiceException ex)
			{
				//The service throws an exception of type ServiceException, performs logging and rethrows the exception.
				throw new Exception("An error occurred when creating Imaging Set", ex);
			}
		}

		public async Task RunImagingJobAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			Console.WriteLine("Creating Imaging Job");

			try
			{
				ImagingJob imagingJob = new ImagingJob
				{
					ImagingSetId = imagingSetArtifactId,
					WorkspaceId = workspaceArtifactId,
					QcEnabled = Constants.Imaging.Job.QC_ENABLED
				};

				//Run an ImagingSet job.
				Guid? jobGuid = (await ImagingJobManager.RunImagingSetAsync(imagingJob)).ImagingJobId;

				Console.WriteLine("Created Imaging Job!");
			}
			catch (ServiceException ex)
			{
				//The service throws an exception of type ServiceException, performs logging and rethrows the exception.
				throw new Exception("An error occurred when running Imaging Job", ex);
			}
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
				HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
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

			try
			{
				SearchContainerRef searchFolder = new SearchContainerRef();

				KeywordSearch keywordSearch = new KeywordSearch
				{
					Name = Constants.Search.KeywordSearch.NAME,
					SearchContainer = searchFolder
				};

				// Get all the query fields available to the current user.
				SearchResultViewFields searchResultViewFields = await KeywordSearchManager.GetFieldsForSearchResultViewAsync(workspaceArtifactId, 10);

				// Set the owner to the current user, in this case "Admin, Relativity," or "0" for public.
				List<UserRef> searchOwners = await KeywordSearchManager.GetSearchOwnersAsync(workspaceArtifactId);
				keywordSearch.Owner = searchOwners.First(o => o.Name == Constants.Search.KeywordSearch.OWNER);

				// Add the fields to the Fields collection.
				// If a field Name, ArtifactID, Guid, or ViewFieldID is known, a field can be set with that information as well.

				FieldRef fieldRef = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.KeywordSearch.FIELD_EDIT);
				keywordSearch.Fields.Add(fieldRef);

				fieldRef = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.KeywordSearch.FIELD_FILE_ICON);
				keywordSearch.Fields.Add(fieldRef);

				fieldRef = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.KeywordSearch.FIELD_CONTROL_NUMBER);
				keywordSearch.Fields.Add(fieldRef);

				// Create a Criteria for the field named "Extracted Text" where the value is set

				Criteria criteria = new Criteria
				{
					Condition = new CriteriaCondition(
						new FieldRef
						{
							Name = Constants.Search.KeywordSearch.CONDITION_FIELD_EXTRACTED_TEXT
						}, CriteriaConditionEnum.IsSet)
				};

				// Add the search condition criteria to the collection.
				keywordSearch.SearchCriteria.Conditions.Add(criteria);

				// Add a note.

				keywordSearch.Notes = Constants.Search.KeywordSearch.NOTES;
				keywordSearch.ArtifactTypeID = 10;

				// Create the search.
				int keywordSearchArtifactId = await KeywordSearchManager.CreateSingleAsync(workspaceArtifactId, keywordSearch);

				if (keywordSearchArtifactId == 0)
				{
					throw new Exception("Failed to create the Keyword Search");
				}

				Console.WriteLine("Created Keyword search for DtSearch Index!");

				return keywordSearchArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when creating Keyword Search", ex);
			}
		}

		public async Task<bool> CheckThatAllDocumentsInWorkspaceAreImaged(int workspaceArtifactId)
		{
			try
			{
				HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
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
