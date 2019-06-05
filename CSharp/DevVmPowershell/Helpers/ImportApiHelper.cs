using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Relativity.Services.ServiceProxy;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Helpers
{
	public class ImportApiHelper : IImportApiHelper
	{
		private ImportAPI importApi;
		private ServiceFactory ServiceFactory { get; }

		public ImportApiHelper(IConnectionHelper connectionHelper, string userName, string password, string webServiceUrl)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
			importApi = new ImportAPI(userName, password, webServiceUrl);
		}

		public async Task<int> AddDocumentsToWorkspace(int workspaceId, string fileType, int fileCount)
		{
			int numDocsBefore = await GetNumberOfDocumentsAsync(ServiceFactory, workspaceId, fileType);

			CreateAndExecuteJob(workspaceId, fileType, fileCount, numDocsBefore);

			int numDocsAfter = await GetNumberOfDocumentsAsync(ServiceFactory, workspaceId, fileType);

			return numDocsAfter - numDocsBefore;
		}

		protected static DataTable GenerateDocumentDataTable(string fileType, int fileCount, int currentFileCount)
		{
			DataTable dataSource = new DataTable();

			switch (fileType.ToLower())
			{
				case Constants.FileTypes.Documents:
					dataSource.Columns.Add(Constants.CommonFields.ControlNumber, typeof(string));
					dataSource.Columns.Add(Constants.CommonFields.FilePath, typeof(string));
					dataSource.Columns.Add(Constants.CommonFields.ParentDocId, typeof(string));
					dataSource.Columns.Add(Constants.CommonFields.FileName, typeof(string));
					break;
				case Constants.FileTypes.Images:
					dataSource.Columns.Add(Constants.CommonFields.Bates, typeof(string));
					dataSource.Columns.Add(Constants.CommonFields.Doc, typeof(string));
					dataSource.Columns.Add(Constants.CommonFields.File, typeof(string));
					break;
			}

			AddFilesToDataTable(dataSource, fileType, fileCount, currentFileCount);

			return dataSource;
		}

		public static void AddFilesToDataTable(DataTable dataSource, string fileType, int fileCount, int currentFileCount)
		{
			string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string resourcePath = Path.Combine(executableLocation, $@"Resources\{fileType}");
			string[] files = Directory.GetFiles(resourcePath);

			for (int i = 0; i < fileCount;)
			{
				int startingIndex = i;
				foreach (string filePath in files)
				{
					switch (fileType.ToLower())
					{
						case Constants.FileTypes.Documents:
							dataSource.Rows.Add($"DOC_{currentFileCount + i}", filePath, "", Path.GetFileName(filePath));
							break;
						case Constants.FileTypes.Images:
							dataSource.Rows.Add($"IMG_{currentFileCount + i}", $"IMG_{currentFileCount + i}", filePath);
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
				queryRequest.ObjectType = new ObjectTypeRef() { Name = "Document" };
				queryRequest.Fields = new List<FieldRef>()
				{
					new FieldRef() {
						Name = "Control Number"
					},
					new FieldRef() {
						Name = "Has Images"
					},
					new FieldRef() {
						Name = "Has Native"
					}
				};

				switch (fileType.ToLower())
				{
					case Constants.FileTypes.Documents:
						queryRequest.Condition = "(('Control Number' LIKE 'DOC_'))";
						break;
					case Constants.FileTypes.Images:
						queryRequest.Condition = "(('Control Number' LIKE 'IMG_'))";
						break;
				}

				QueryResult results = await objectManager.QueryAsync(workspaceId, queryRequest, 1, 100);

				return results.TotalCount;
			}
		}

		public void CreateAndExecuteJob(int workspaceId, string fileType, int fileCount, int currentFileCount)
		{
			switch (fileType.ToLower())
			{
				case Constants.FileTypes.Documents:
					ImportBulkArtifactJob documentJob = importApi.NewNativeDocumentImportJob();

					documentJob.OnMessage += ImportJobOnMessage;
					documentJob.OnComplete += ImportJobOnComplete;
					documentJob.OnFatalException += ImportJobOnFatalException;

					documentJob.Settings.CaseArtifactId = workspaceId;
					documentJob.Settings.ExtractedTextFieldContainsFilePath = false;

					// Indicates file path for the native file.
					documentJob.Settings.NativeFilePathSourceFieldName = Constants.CommonFields.FilePath;//"Native File";

					// Indicates the column containing the ID of the parent document.
					documentJob.Settings.ParentObjectIdSourceFieldName = Constants.CommonFields.ParentDocId;

					// The name of the document identifier column must match the name of the document identifier field
					// in the workspace.
					documentJob.Settings.SelectedIdentifierFieldName = Constants.CommonFields.ControlNumber;//"Doc ID Beg";
					documentJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
					documentJob.Settings.OverwriteMode = OverwriteModeEnum.Append;

					// Specify the ArtifactID of the document identifier field, such as a control number.
					documentJob.Settings.IdentityFieldId = Constants.CommonArtifactIds.ControlNumber;

					// Add the files to the data source.
					documentJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount, currentFileCount).CreateDataReader();
					documentJob.Execute();

					break;
				case Constants.FileTypes.Images:
					ImageImportBulkArtifactJob imageJob = importApi.NewImageImportJob();

					imageJob.OnMessage += ImportJobOnMessage;
					imageJob.OnComplete += ImportJobOnComplete;
					imageJob.OnFatalException += ImportJobOnFatalException;

					imageJob.Settings.AutoNumberImages = false;

					// You can use the Bates number as an identifier for an image.
					imageJob.Settings.BatesNumberField = Constants.CommonFields.Bates;
					imageJob.Settings.CaseArtifactId = workspaceId;

					// Use this code for grouping images associated with a document.
					imageJob.Settings.DocumentIdentifierField = Constants.CommonFields.Doc;

					// Indicates filepath for an image.
					imageJob.Settings.FileLocationField = Constants.CommonFields.File;

					//Indicates that the images must be copied to the document repository
					imageJob.Settings.CopyFilesToDocumentRepository = true;

					// Specifies the ArtifactID of a document identifier field, such as a control number.
					imageJob.Settings.IdentityFieldId = Constants.CommonArtifactIds.ControlNumber;
					imageJob.Settings.OverwriteMode = OverwriteModeEnum.AppendOverlay;

					// Add the files to the data source.
					imageJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount, currentFileCount);
					imageJob.Execute();

					break;
				default:
					throw new Exception("Job must be either for documents or images");
			}
		}
	}
}
