﻿using Helpers.Interfaces;
using System;
using System.Linq;
using System.Net.Http;
using Helpers.RequestModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class DisclaimerAcceptanceHelper : IDisclaimerAcceptanceHelper
	{
		private IConnectionHelper ConnectionHelper { get; }
		private IRestHelper RestHelper { get; set; }
		private IWorkspaceHelper WorkspaceHelper { get; set; }

		public DisclaimerAcceptanceHelper(IConnectionHelper connectionHelper, IRestHelper restHelper, IWorkspaceHelper workspaceHelper)
		{
			ConnectionHelper = connectionHelper;
			RestHelper = restHelper;
			WorkspaceHelper = workspaceHelper;
		}

		public async Task AddDisclaimerConfigurationAsync(string workspaceName)
		{
			try
			{
				int workspaceId = await WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);
				int objectTypeId = await GetDisclaimerSolutionConfigurationObjectTypeIdAsync(workspaceId);
				int layoutId = await GetDisclaimerSolutionConfigurationLayoutIdAsync(workspaceId);
				await CreateDisclaimerConfigurationRdoAsync(objectTypeId, layoutId, workspaceId);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Adding Disclaimer Configuration", ex);
			}
		}

		public async Task<bool> CheckIfDisclaimerConfigurationRdoExistsAsync(int workspaceId)
		{
			string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceId.ToString());
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			int objectTypeId = await GetDisclaimerSolutionConfigurationObjectTypeIdAsync(workspaceId);
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
			HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
			if (!queryResponse.IsSuccessStatusCode)
			{
				string responseContent = await queryResponse.Content.ReadAsStringAsync();
				throw new Exception($"Failed to Query for Disclaimer Configuration RDO. [{nameof(responseContent)}: {responseContent}]");
			}
			string resultString = await queryResponse.Content.ReadAsStringAsync();
			dynamic result = JObject.Parse(resultString) as JObject;
			int totalCount = result.TotalCount;
			return totalCount > 0;
		}

		public async Task AddDisclaimerAsync(string workspaceName)
		{
			try
			{
				int workspaceId = await WorkspaceHelper.GetFirstWorkspaceArtifactIdQueryAsync(workspaceName);
				int objectTypeId = await GetDisclaimerObjectTypeIdAsync(workspaceId);
				int layoutId = await GetDisclaimerLayoutIdAsync(workspaceId);
				await CreateDisclaimerRdoAsync(objectTypeId, layoutId, workspaceId);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Adding Disclaimer", ex);
			}
		}

		public async Task<bool> CheckIfDisclaimerRdoExistsAsync(int workspaceId)
		{
			string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1",workspaceId.ToString());
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
			int objectTypeId = await GetDisclaimerObjectTypeIdAsync(workspaceId);
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
			HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
			if (!queryResponse.IsSuccessStatusCode)
			{
				string responseContent = await queryResponse.Content.ReadAsStringAsync();
				throw new Exception($"Failed to Query for Disclaimer RDO. [{nameof(responseContent)}: {responseContent}]");
			}
			string resultString = await queryResponse.Content.ReadAsStringAsync();
			dynamic result = JObject.Parse(resultString) as JObject;
			int totalCount = result.TotalCount;
			return totalCount > 0;
		}

		private async Task CreateDisclaimerConfigurationRdoAsync(int objectTypeId, int layoutId, int workspaceId)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
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
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, url, request);
			if (!response.IsSuccessStatusCode)
			{
				string responseContent = await response.Content.ReadAsStringAsync();
				throw new Exception($"Failed to Create for Disclaimer Configuration RDO. [{nameof(responseContent)}: {responseContent}]");
			}
		}

		private async Task CreateDisclaimerRdoAsync(int objectTypeId, int layoutId, int workspaceId)
		{
			HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
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
			HttpResponseMessage response = await RestHelper.MakePostAsync(httpClient, url, request);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to Create for Disclaimer Configuration RDO.");
			}
		}

		private async Task<int> GetDisclaimerObjectTypeIdAsync(int workspaceId)
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
						Condition = $"'Name' == '{Constants.DisclaimerAcceptance.ObjectNames.Disclaimer}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Disclaimer Configuration RDO. [{nameof(responseContent)}: {responseContent}]");
				}

				string resultString = queryResponse.Content.ReadAsStringAsync().Result;
				dynamic result = JObject.Parse(resultString) as JObject;
				objectTypeId = result.Objects[0]["Values"][1];
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Reading the {Constants.DisclaimerAcceptance.ObjectNames.Disclaimer} ObjectType", ex);
			}

			return objectTypeId;
		}

		private async Task<int> GetDisclaimerSolutionConfigurationObjectTypeIdAsync(int workspaceId)
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
						Condition = $"'Name' == '{Constants.DisclaimerAcceptance.ObjectNames.DisclaimerSolutionConfiguration}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Disclaimer Solution Configuration Object Type. [{nameof(responseContent)}: {responseContent}]");
				}

				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				objectTypeId = result.Objects[0]["Values"][1];
			}
			catch (Exception ex)
			{
				throw new Exception(
					$"Error Reading the {Constants.DisclaimerAcceptance.ObjectNames.DisclaimerSolutionConfiguration} ObjectType", ex);
			}

			return objectTypeId;
		}

		private async Task<int> GetDisclaimerLayoutIdAsync(int workspaceId)
		{
			int layoutId;
			try
			{
				string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceId.ToString());
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = Constants.LAYOUT_TYPE_ARTIFACT_ID },
						fields = new[]
						{
							new { Name = "Name" },
						},
						Condition = $"'Name' == '{Constants.DisclaimerAcceptance.LayoutNames.DisclaimerLayout}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Disclaimer Layout. [{nameof(responseContent)}: {responseContent}]");
				}

				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				layoutId = result.Objects[0]["ArtifactID"];
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Reading {Constants.DisclaimerAcceptance.LayoutNames.DisclaimerSolutionConfigurationLayout} Id", ex);
			}

			return layoutId;
		}

		private async Task<int> GetDisclaimerSolutionConfigurationLayoutIdAsync(int workspaceId)
		{
			int layoutId;
			try
			{
				string url = Constants.Connection.RestUrlEndpoints.ObjectManager.QuerySlimUrl.Replace("-1", workspaceId.ToString());
				HttpClient httpClient = RestHelper.GetHttpClient(ConnectionHelper.RelativityInstanceName, ConnectionHelper.RelativityAdminUserName, ConnectionHelper.RelativityAdminPassword);
				var queryPayloadObject = new
				{
					request = new
					{
						objectType = new { artifactTypeId = Constants.LAYOUT_TYPE_ARTIFACT_ID },
						fields = new[]
						{
							new { Name = "Name" },
						},
						Condition = $"'Name' == '{Constants.DisclaimerAcceptance.LayoutNames.DisclaimerSolutionConfigurationLayout}'",
					},
					start = 1,
					length = 25
				};

				string queryPayload = JsonConvert.SerializeObject(queryPayloadObject);
				HttpResponseMessage queryResponse = await RestHelper.MakePostAsync(httpClient, url, queryPayload);
				if (!queryResponse.IsSuccessStatusCode)
				{
					string responseContent = await queryResponse.Content.ReadAsStringAsync();
					throw new Exception($"Failed to Query for Disclaimer Configuration Layout. [{nameof(responseContent)}: {responseContent}]");
				}

				string resultString = await queryResponse.Content.ReadAsStringAsync();
				dynamic result = JObject.Parse(resultString) as JObject;
				layoutId = result.Objects[0]["ArtifactID"];
			}
			catch (Exception ex)
			{
				throw new Exception($"Error Reading {Constants.DisclaimerAcceptance.LayoutNames.DisclaimerSolutionConfigurationLayout} Id", ex);
			}

			return layoutId;
		}
	}
}
