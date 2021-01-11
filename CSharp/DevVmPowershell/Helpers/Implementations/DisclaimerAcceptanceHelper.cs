using Helpers.Interfaces;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using System;
using System.Linq;
using System.Net.Http;
using Helpers.RequestModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectType = kCura.Relativity.Client.DTOs.ObjectType;

namespace Helpers.Implementations
{
	public class DisclaimerAcceptanceHelper : IDisclaimerAcceptanceHelper
	{
		private IRSAPIClient RsapiClient { get; }
		private IObjectManager ObjectManager { get; }
		private ServiceFactory ServiceFactory { get; }
		private string InstanceAddress { get; set; }
		private string AdminUsername { get; set; }
		private string AdminPassword { get; set; }

		public DisclaimerAcceptanceHelper(IConnectionHelper connectionHelper, string instanceAddress, string adminUsername, string adminPassword)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
			RsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>();
			ObjectManager = ServiceFactory.CreateProxy<IObjectManager>();
			InstanceAddress = instanceAddress;
			AdminUsername = adminUsername;
			AdminPassword = adminPassword;
		}

		public void AddDisclaimerConfiguration(string workspaceName)
		{
			try
			{
				int workspaceId = GetWorkspaceId(workspaceName);
				int objectTypeId = GetDisclaimerSolutionConfigurationObjectTypeId(workspaceId);
				int layoutId = GetDisclaimerSolutionConfigurationLayoutId();
				CreateDisclaimerConfigurationRDO(objectTypeId, layoutId, workspaceId);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Adding Disclaimer Configuration", ex);
			}
		}

		public bool CheckIfDisclaimerConfigurationRDOExists(int workspaceId)
		{
			//RsapiClient.APIOptions.WorkspaceID = workspaceId;
			//int objectTypeId = GetDisclaimerSolutionConfigurationObjectTypeId(workspaceId);
			//Query<RDO> query = new Query<RDO>
			//{
			//	ArtifactTypeID = objectTypeId
			//};

			//QueryResultSet<RDO> queryResultSet = RsapiClient.Repositories.RDO.Query(query);
			//return queryResultSet.TotalCount > 0;

			string url = $"Relativity.REST/api/Relativity.Objects/workspace/-1/object/queryslim";
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
			int objectTypeId = GetDisclaimerSolutionConfigurationObjectTypeId(workspaceId);
			var queryPayloadObject = new
			{
				request = new
				{
					objectType = new { artifactTypeId = objectTypeId },
					fields = new[]
					{
						new { Name = "Name" },
					},
					Condition = "",
				},
				start = 1,
				length = 25
			};

			string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
			HttpResponseMessage queryResponse = RestHelper.MakePostAsync(httpClient, url, queryPayload).Result;
			if (!queryResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Query for Disclaimer Configuration RDO");
			}
			string resultString = queryResponse.Content.ReadAsStringAsync().Result;
			dynamic result = JObject.Parse(resultString) as JObject;
			int totalCount = result.TotalCount;
			return totalCount > 0;
		}

		public void AddDisclaimer(string workspaceName)
		{
			try
			{
				int workspaceId = GetWorkspaceId(workspaceName);
				int objectTypeId = GetDisclaimerObjectTypeId(workspaceId);
				int layoutId = GetDisclaimerLayoutId();
				CreateDisclaimerRDO(objectTypeId, layoutId, workspaceId);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Adding Disclaimer", ex);
			}
		}

		public bool CheckIfDisclaimerRDOExists(int workspaceId)
		{
			//RsapiClient.APIOptions.WorkspaceID = workspaceId;
			//int objectTypeId = GetDisclaimerObjectTypeId(workspaceId);
			//Query<RDO> query = new Query<RDO>
			//{
			//	ArtifactTypeID = objectTypeId
			//};

			//QueryResultSet<RDO> queryResultSet = RsapiClient.Repositories.RDO.Query(query);
			//return queryResultSet.TotalCount > 0;

			string url = $"Relativity.REST/api/Relativity.Objects/workspace/-1/object/queryslim";
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
			int objectTypeId = GetDisclaimerObjectTypeId(workspaceId);
			var queryPayloadObject = new
			{
				request = new
				{
					objectType = new { artifactTypeId = objectTypeId },
					fields = new[]
					{
						new { Name = "Name" },
					},
					Condition = "",
				},
				start = 1,
				length = 25
			};

			string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
			HttpResponseMessage queryResponse = RestHelper.MakePostAsync(httpClient, url, queryPayload).Result;
			if (!queryResponse.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Query for Disclaimer RDO");
			}
			string resultString = queryResponse.Content.ReadAsStringAsync().Result;
			dynamic result = JObject.Parse(resultString) as JObject;
			int totalCount = result.TotalCount;
			return totalCount > 0;
		}

		private void CreateDisclaimerConfigurationRDO(int objectTypeId, int layoutId, int workspaceId)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
			string url = $"Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/create";
			ObjectManagerCreateRequestModel objectManagerCreateRequestModel = new ObjectManagerCreateRequestModel
			{
				Request = new request
				{
					ObjectType = new objectType
					{
						ArtifactTypeID = objectTypeId
					},
					FieldValues = new object[]
					{
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerSolutionConfigurationFieldGuids.Name
							},
							Value = "Sample Configuration"
						},
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerSolutionConfigurationFieldGuids.Enabled
							},
							Value = true
						},
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerSolutionConfigurationFieldGuids.AllowAccessOnError
							},
							Value = true
						}
					}
				},
				OperationOptions = new Helpers.RequestModels.OperationOptions
				{
					CallingContext = new Helpers.RequestModels.CallingContext
					{
						Layout = new Helpers.RequestModels.Layout
						{
							ArtifactID = layoutId
						}
					}
				}
			};
			string request = JsonConvert.SerializeObject(objectManagerCreateRequestModel);
			HttpResponseMessage response = RestHelper.MakePostAsync(httpClient, url, request).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Create for Disclaimer Configuration RDO.");
			}
		}

		private void CreateDisclaimerRDO(int objectTypeId, int layoutId, int workspaceId)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(InstanceAddress, AdminUsername, AdminPassword);
			string url = $"Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/create";
			ObjectManagerCreateRequestModel objectManagerCreateRequestModel = new ObjectManagerCreateRequestModel
			{
				Request = new request
				{
					ObjectType = new objectType
					{
						ArtifactTypeID = objectTypeId
					},
					FieldValues = new object[]
					{
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Title
							},
							Value = "DevVm Disclaimer"
						},
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Text
							},
							Value = Constants.DisclaimerAcceptance.DisclaimerValue
						},
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Order
							},
							Value = 10
						},
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Enabled
							},
							Value = true
						},
						new
						{
							Field = new
							{
								Guid = Constants.DisclaimerAcceptance.DisclaimerFieldGuids.AllUsers
							},
							Value = true
						}
					}
				},
				OperationOptions = new Helpers.RequestModels.OperationOptions
				{
					CallingContext = new Helpers.RequestModels.CallingContext
					{
						Layout = new Helpers.RequestModels.Layout
						{
							ArtifactID = layoutId
						}
					}
				}
			};
			string request = JsonConvert.SerializeObject(objectManagerCreateRequestModel);
			HttpResponseMessage response = RestHelper.MakePostAsync(httpClient, url, request).Result;
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Create for Disclaimer Configuration RDO.");
			}
		}

		private int GetDisclaimerObjectTypeId(int workspaceId)
		{
			int objectTypeId;
			RsapiClient.APIOptions.WorkspaceID = workspaceId;
			try
			{
				ResultSet<ObjectType> results = RsapiClient.Repositories.ObjectType.Read(
					new ObjectType(new Guid(Constants.DisclaimerAcceptance.ObjectGuids.Disclaimer))
					{
						Fields = { new FieldValue(ObjectTypeFieldNames.DescriptorArtifactTypeID) }
					});
				if (!results.Success)
				{
					throw new Exception(
						$"Reading {Constants.DisclaimerAcceptance.ObjectNames.Disclaimer} ObjectType was not successful");
				}

				objectTypeId = results.Results.First().Artifact.DescriptorArtifactTypeID.Value;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Reading the {Constants.DisclaimerAcceptance.ObjectNames.Disclaimer} ObjectType", ex);
			}

			return objectTypeId;
		}

		private int GetWorkspaceId(string workspaceName)
		{
			int workspaceId;
			RsapiClient.APIOptions.WorkspaceID = -1;
			Query<Workspace> query = new Query<Workspace>();
			query.Condition = new kCura.Relativity.Client.TextCondition(WorkspaceFieldNames.Name,
				kCura.Relativity.Client.TextConditionEnum.EqualTo, workspaceName);
			query.Fields = FieldValue.NoFields;
			try
			{
				kCura.Relativity.Client.DTOs.QueryResultSet<Workspace> resultSet =
					RsapiClient.Repositories.Workspace.Query(query, 0);
				if (resultSet.Success && resultSet.Results.Count > 0)
				{
					workspaceId = resultSet.Results.First().Artifact.ArtifactID;
				}
				else
				{
					throw new Exception($"Unable to find workspace with the name: {workspaceName}");
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Error finding workspace with name: {workspaceName}", ex);
			}

			return workspaceId;
		}

		private int GetDisclaimerSolutionConfigurationObjectTypeId(int workspaceId)
		{
			int objectTypeId;
			RsapiClient.APIOptions.WorkspaceID = workspaceId;
			try
			{
				ResultSet<ObjectType> results = RsapiClient.Repositories.ObjectType.Read(
					new ObjectType(new Guid(Constants.DisclaimerAcceptance.ObjectGuids.DisclaimerSolutionConfiguration))
					{
						Fields = { new FieldValue(ObjectTypeFieldNames.DescriptorArtifactTypeID) }
					});
				if (!results.Success)
				{
					throw new Exception(
						$"Reading {Constants.DisclaimerAcceptance.ObjectNames.DisclaimerSolutionConfiguration} ObjectType was not successful");
				}

				objectTypeId = results.Results.First().Artifact.DescriptorArtifactTypeID.Value;
			}
			catch (Exception ex)
			{
				throw new Exception(
					$"Error Reading the {Constants.DisclaimerAcceptance.ObjectNames.DisclaimerSolutionConfiguration} ObjectType", ex);
			}

			return objectTypeId;
		}

		private int GetDisclaimerLayoutId()
		{
			int layoutId;
			TextCondition queryCondition =
				new TextCondition(LayoutFieldNames.Name, TextConditionEnum.EqualTo, Constants.DisclaimerAcceptance.LayoutNames.DisclaimerLayout);
			Query<kCura.Relativity.Client.DTOs.Layout> layoutQuery = new Query<kCura.Relativity.Client.DTOs.Layout>();
			layoutQuery.Condition = queryCondition;
			layoutQuery.Fields = FieldValue.NoFields;
			ResultSet<kCura.Relativity.Client.DTOs.Layout> layoutQueryResultSet =
				new ResultSet<kCura.Relativity.Client.DTOs.Layout>();
			try
			{
				layoutQueryResultSet = RsapiClient.Repositories.Layout.Query(layoutQuery);
				layoutId = layoutQueryResultSet.Results.First().Artifact.ArtifactID;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Reading {Constants.DisclaimerAcceptance.LayoutNames.DisclaimerSolutionConfigurationLayout} Id", ex);
			}

			return layoutId;
		}

		private int GetDisclaimerSolutionConfigurationLayoutId()
		{
			int layoutId;
			TextCondition queryCondition =
				new TextCondition(LayoutFieldNames.Name, TextConditionEnum.EqualTo, Constants.DisclaimerAcceptance.LayoutNames.DisclaimerSolutionConfigurationLayout);
			Query<kCura.Relativity.Client.DTOs.Layout> layoutQuery = new Query<kCura.Relativity.Client.DTOs.Layout>();
			layoutQuery.Condition = queryCondition;
			layoutQuery.Fields = FieldValue.NoFields;
			ResultSet<kCura.Relativity.Client.DTOs.Layout> layoutQueryResultSet =
				new ResultSet<kCura.Relativity.Client.DTOs.Layout>();
			try
			{
				layoutQueryResultSet = RsapiClient.Repositories.Layout.Query(layoutQuery);
				layoutId = layoutQueryResultSet.Results.First().Artifact.ArtifactID;
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Reading {Constants.DisclaimerAcceptance.LayoutNames.DisclaimerSolutionConfigurationLayout} Id", ex);
			}

			return layoutId;
		}
	}
}
