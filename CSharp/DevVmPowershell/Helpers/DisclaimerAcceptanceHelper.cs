using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using Layout = Relativity.Services.Objects.DataContracts.Layout;
using ObjectType = kCura.Relativity.Client.DTOs.ObjectType;

namespace Helpers
{
	public class DisclaimerAcceptanceHelper : IDisclaimerAcceptanceHelper
	{
		private IRSAPIClient RsapiClient { get; }
		private IObjectManager ObjectManager { get; }
		private ServiceFactory serviceFactory { get; }

		public DisclaimerAcceptanceHelper(IConnectionHelper connectionHelper)
		{
			serviceFactory = connectionHelper.GetServiceFactory();
			RsapiClient = serviceFactory.CreateProxy<IRSAPIClient>();
			ObjectManager = serviceFactory.CreateProxy<IObjectManager>();
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

		private void CreateDisclaimerConfigurationRDO(int objectTypeId, int layoutId, int workspaceId)
		{
			var createRequest = new CreateRequest();
			createRequest.ObjectType = new ObjectTypeRef { ArtifactTypeID = objectTypeId };
			createRequest.FieldValues = new FieldRefValuePair[]
			{
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerSolutionConfigurationFieldGuids.Name)
					},
					Value = "Sample Configuration"
				},
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerSolutionConfigurationFieldGuids.Enabled)
					},
					Value = true
				},
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerSolutionConfigurationFieldGuids.AllowAccessOnError)
					},
					Value = true
				},
			};

			var callingContext = new CallingContext
			{
				Layout = new LayoutRef { ArtifactID = layoutId },
				PageMode = PageMode.Edit
			};
			var createOptions = new OperationOptions
			{
				CallingContext = callingContext
			};

			CreateResult createResult = ObjectManager.CreateAsync(workspaceId, createRequest, createOptions).Result;
		}

		private void CreateDisclaimerRDO(int objectTypeId, int layoutId, int workspaceId)
		{
			var createRequest = new CreateRequest();
			createRequest.ObjectType = new ObjectTypeRef {ArtifactTypeID = objectTypeId};
			createRequest.FieldValues = new FieldRefValuePair[]
			{
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Title)
					},
					Value = "DevVm Disclaimer"
				},
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Text)
					},
					Value = Constants.DisclaimerAcceptance.DisclaimerValue
				},
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Order)
					},
					Value = 10
				},
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerFieldGuids.Enabled)
					},
					Value = true
				},
				new FieldRefValuePair
				{
					Field = new FieldRef
					{
						Guid = new Guid(Constants.DisclaimerAcceptance.DisclaimerFieldGuids.AllUsers)
					},
					Value = true
				},
			};

			var callingContext = new CallingContext
			{
				Layout = new LayoutRef {ArtifactID = layoutId},
				PageMode = PageMode.Edit
			};
			var createOptions = new OperationOptions
			{
				CallingContext = callingContext
			};

			CreateResult createResult = ObjectManager.CreateAsync(workspaceId, createRequest, createOptions).Result;
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
						Fields = FieldValue.AllFields
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
			query.Fields = FieldValue.AllFields;
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
						Fields = FieldValue.AllFields
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
			layoutQuery.Fields = FieldValue.AllFields;
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
			layoutQuery.Fields = FieldValue.AllFields;
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
