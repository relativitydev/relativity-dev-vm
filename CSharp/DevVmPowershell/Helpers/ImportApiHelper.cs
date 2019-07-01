using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using QueryResult = Relativity.Services.Objects.DataContracts.QueryResult;

namespace Helpers
{
	public class ImportApiHelper : IImportApiHelper
	{
		private ImportAPI importApi;
		private ServiceFactory ServiceFactory { get; }
		private string resourceFolderPath;

		public ImportApiHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();

			importApi = connectionHelper.GetImportApi();
		}

		public async Task<int> AddDocumentsToWorkspace(int workspaceId, string fileType, int fileCount, string resourceFolderPath)
		{
			int numDocsBefore = await GetNumberOfDocumentsAsync(ServiceFactory, workspaceId, fileType);

			CreateAndExecuteJob(workspaceId, fileType, fileCount, numDocsBefore, resourceFolderPath);

			int numDocsAfter = await GetNumberOfDocumentsAsync(ServiceFactory, workspaceId, fileType);

			return numDocsAfter - numDocsBefore;
		}

		protected static DataTable GenerateDocumentDataTable(string fileType, int fileCount, int currentFileCount, string resourceFolderPath)
		{
			DataTable dataSource = new DataTable();

			switch (fileType.ToLower())
			{
				case Constants.FileType.Document:
					dataSource.Columns.Add(Constants.DocumentCommonFields.ControlNumber, typeof(string));
					dataSource.Columns.Add(Constants.DocumentCommonFields.FilePath, typeof(string));
					dataSource.Columns.Add(Constants.DocumentCommonFields.ParentDocId, typeof(string));
					dataSource.Columns.Add(Constants.DocumentCommonFields.FileName, typeof(string));
					break;
				case Constants.FileType.Image:
					dataSource.Columns.Add(Constants.DocumentCommonFields.Bates, typeof(string));
					dataSource.Columns.Add(Constants.DocumentCommonFields.Doc, typeof(string));
					dataSource.Columns.Add(Constants.DocumentCommonFields.File, typeof(string));
					break;
			}

			dataSource.Columns.Add(Constants.DocumentCommonFields.ExtractedText, typeof(string));

			AddFilesToDataTable(dataSource, fileType, fileCount, currentFileCount, resourceFolderPath);

			return dataSource;
		}

		public static void AddFilesToDataTable(DataTable dataSource, string fileType, int fileCount, int currentFileCount, string resourceFolderPath)
		{
			string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string resourcePath = Path.Combine((string.IsNullOrEmpty(resourceFolderPath)) ? executableLocation : resourceFolderPath, $@"Resources\{fileType}s");
			string[] files = Directory.GetFiles(resourcePath);

			List<string> nonExtractedTextFiles = files.ToList();
			nonExtractedTextFiles.RemoveAll(x => x.ToUpper().Contains("DOCTXT_")
																					 || (fileType.ToLower().Equals(Constants.FileType.Image) && x.ToLower().Contains(".txt")));

			for (int i = 0; i < fileCount;)
			{
				int startingIndex = i;
				foreach (string filePath in nonExtractedTextFiles)
				{
					switch (fileType.ToLower())
					{
						case Constants.FileType.Document:
							dataSource.Rows.Add($"DOC_{currentFileCount + i}", filePath, "", Path.GetFileName(filePath), $@"{Path.GetDirectoryName(filePath)}\{Path.GetFileNameWithoutExtension(filePath)}.txt");
							break;
						case Constants.FileType.Image:
							dataSource.Rows.Add($"IMG_{currentFileCount + i}", $"IMG_{currentFileCount + i}", filePath, filePath.Replace(".tiff", ".txt"));
							break;
					}
					i++;
					if (i >= fileCount)
					{
						break;
					}
				}

				if (i == startingIndex)
				{
					break;
				}
			}
		}

		static void ImportJobOnMessage(Status status)
		{
			Console.WriteLine($"Message: {status.Message}");
		}

		static void ImportJobOnFatalException(JobReport jobReport)
		{
			Console.WriteLine($"Fatal Error: {jobReport.FatalException}");
		}

		static void ImportJobOnComplete(JobReport jobReport)
		{
			Console.WriteLine($"Job Finished With {jobReport.ErrorRowCount} Errors: ");
		}

		static async Task<int> GetNumberOfDocumentsAsync(ServiceFactory serviceFactory, int workspaceId, string fileType)
		{
			using (IObjectManager objectManager = serviceFactory.CreateProxy<IObjectManager>())
			{
				QueryRequest queryRequest = new QueryRequest();
				queryRequest.ObjectType = new ObjectTypeRef() { Name = Constants.DocumentCommonFields.DocumentTypeRef };
				queryRequest.Fields = new List<FieldRef>()
				{
					new FieldRef() {
						Name = Constants.DocumentCommonFields.ControlNumber
					},
					new FieldRef() {
						Name = Constants.DocumentCommonFields.HasImages
					},
					new FieldRef() {
						Name = Constants.DocumentCommonFields.HasNative
					}
				};

				switch (fileType.ToLower())
				{
					case Constants.FileType.Document:
						queryRequest.Condition = $"(('{Constants.DocumentCommonFields.ControlNumber}' LIKE 'DOC_'))";
						break;
					case Constants.FileType.Image:
						queryRequest.Condition = $"(('{Constants.DocumentCommonFields.ControlNumber}' LIKE 'IMG_'))";
						break;
				}

				QueryResult results = await objectManager.QueryAsync(workspaceId, queryRequest, 1, 100);

				return results.TotalCount;
			}
		}

		public void CreateAndExecuteJob(int workspaceId, string fileType, int fileCount, int currentFileCount, string resourceFolderPath)
		{
			switch (fileType.ToLower())
			{
				case Constants.FileType.Document:
					ImportBulkArtifactJob documentJob = importApi.NewNativeDocumentImportJob();

					documentJob.OnMessage += ImportJobOnMessage;
					documentJob.OnComplete += ImportJobOnComplete;
					documentJob.OnFatalException += ImportJobOnFatalException;

					documentJob.Settings.CaseArtifactId = workspaceId;
					documentJob.Settings.ExtractedTextFieldContainsFilePath = true;
					documentJob.Settings.ExtractedTextEncoding = Encoding.UTF8;


					// Indicates file path for the native file.
					documentJob.Settings.NativeFilePathSourceFieldName = Constants.DocumentCommonFields.FilePath;//"Native File";

					// Indicates the column containing the ID of the parent document.
					documentJob.Settings.ParentObjectIdSourceFieldName = Constants.DocumentCommonFields.ParentDocId;

					// The name of the document identifier column must match the name of the document identifier field
					// in the workspace.
					documentJob.Settings.SelectedIdentifierFieldName = Constants.DocumentCommonFields.ControlNumber;//"Doc ID Beg";
					documentJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
					documentJob.Settings.OverwriteMode = OverwriteModeEnum.Append;

					// Specify the ArtifactID of the document identifier field, such as a control number.
					documentJob.Settings.IdentityFieldId = Constants.CommonArtifactIds.ControlNumber;

					// Add the files to the data source.
					documentJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount, currentFileCount, resourceFolderPath).CreateDataReader();
					documentJob.Execute();

					break;
				case Constants.FileType.Image:
					ImageImportBulkArtifactJob imageJob = importApi.NewImageImportJob();

					imageJob.OnMessage += ImportJobOnMessage;
					imageJob.OnComplete += ImportJobOnComplete;
					imageJob.OnFatalException += ImportJobOnFatalException;

					imageJob.Settings.AutoNumberImages = false;
					imageJob.Settings.ExtractedTextFieldContainsFilePath = true;
					imageJob.Settings.ExtractedTextEncoding = Encoding.UTF8;

					// You can use the Bates number as an identifier for an image.
					imageJob.Settings.BatesNumberField = Constants.DocumentCommonFields.Bates;
					imageJob.Settings.CaseArtifactId = workspaceId;

					// Use this code for grouping images associated with a document.
					imageJob.Settings.DocumentIdentifierField = Constants.DocumentCommonFields.Doc;

					// Indicates filepath for an image.
					imageJob.Settings.FileLocationField = Constants.DocumentCommonFields.File;

					//Indicates that the images must be copied to the document repository
					imageJob.Settings.CopyFilesToDocumentRepository = true;

					// Specifies the ArtifactID of a document identifier field, such as a control number.
					imageJob.Settings.IdentityFieldId = Constants.CommonArtifactIds.ControlNumber;
					imageJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;

					// Add the files to the data source.
					imageJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount, currentFileCount, resourceFolderPath);
					imageJob.Execute();

					break;
				default:
					throw new Exception($"Job must be either for {Constants.FileType.Document} or {Constants.FileType.Image}");
			}
		}

		public async Task<int> GetFirstWorkspaceIdQueryAsync(string workspaceName)
		{
			Console.WriteLine($"Querying for Workspaces [Name: {workspaceName}]");

			try
			{
				using (IRSAPIClient rsapiClient = ServiceFactory.CreateProxy<IRSAPIClient>())
				{
					rsapiClient.APIOptions.WorkspaceID = Constants.EDDS_WORKSPACE_ARTIFACT_ID;

					TextCondition textCondition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
					Query<Workspace> workspaceQuery = new Query<Workspace>
					{
						Fields = FieldValue.AllFields,
						Condition = textCondition
					};

					QueryResultSet<Workspace> workspaceQueryResultSet = await Task.Run(() => rsapiClient.Repositories.Workspace.Query(workspaceQuery));

					if (!workspaceQueryResultSet.Success || workspaceQueryResultSet.Results == null)
					{
						throw new Exception("Failed to query Workspaces");
					}

					List<int> workspaceArtifactIds = workspaceQueryResultSet.Results.Select(x => x.Artifact.ArtifactID).ToList();

					Console.WriteLine($"Queried for Workspaces! [Count: {workspaceArtifactIds.Count}]");

					return workspaceArtifactIds.First();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when querying Workspaces", ex);
			}
		}
	}
}