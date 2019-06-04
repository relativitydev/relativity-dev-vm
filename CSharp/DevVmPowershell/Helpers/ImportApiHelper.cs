using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace Helpers
{
	public class ImportApiHelper : IImportApiHelper
	{
		private ImportAPI importApi;
		public ImportApiHelper(string userName, string password, string webServiceUrl)
		{
			importApi = new ImportAPI(userName, password, webServiceUrl);
		}

		public int /*Task<int>*/ AddDocumentsToWorkspace(int workspaceId, string fileType, int fileCount)
		{
			CreateAndExecuteJob(workspaceId, fileType, fileCount);

			int totalDocuments = 0;//job.SourceData.SourceData.RecordsAffected;

			return totalDocuments;
		}



		protected static string GenerateControlNumber()
		{
			return $"REL-{Guid.NewGuid()}";
		}

		protected static DataTable GenerateDocumentDataTable(string fileType, int fileCount)
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

			AddFilesToDataTable(dataSource, fileType, fileCount);

			return dataSource;
		}

		public static void AddFilesToDataTable(DataTable dataSource, string fileType, int fileCount)
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
							dataSource.Rows.Add(GenerateControlNumber(), filePath, "", Path.GetFileName(filePath));
							break;
						case Constants.FileTypes.Images:
							dataSource.Rows.Add($"A_{i}", $"A_{i}", filePath);
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

		//static Task<int> GetNumberOfDocumentsAsync()
		//{

		//}

		public void CreateAndExecuteJob(int workspaceId, string fileType, int fileCount)
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
					documentJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount).CreateDataReader();
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
					imageJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount);

					// Add the files to the data source.
					imageJob.SourceData.SourceData = GenerateDocumentDataTable(fileType, fileCount);
					imageJob.Execute();

					break;
				default:
					throw new Exception("Job must be either for documents or images");
			}
		}
	}
}
