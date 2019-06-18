using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Helpers;
using kCura.Vendor.Castle.Core.Internal;

namespace DevVmPsModules
{
	[Cmdlet(VerbsCommon.Add, "ZipFolder")]
	public class AddZipFolderModule : BaseModule
	{
		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Path of the Folder to be Zipped")]
		public string SourceFolderPath { get; set; }

		[Parameter(
			Mandatory = true,
			ValueFromPipelineByPropertyName = true,
			ValueFromPipeline = true,
			Position = 0,
			HelpMessage = "Path of the where the Zip File should be created")]
		public string DestinationZipFilePath { get; set; }

		protected override void ProcessRecordCode()
		{
			//Validate Input arguments
			ValidateInputArguments();

			IZipHelper zipHelper = new ZipHelper();

			// Zip Designated Folder
			zipHelper.ZipFolder(SourceFolderPath, DestinationZipFilePath);
		}

		private void ValidateInputArguments()
		{
			if (string.IsNullOrWhiteSpace(SourceFolderPath))
			{
				throw new ArgumentNullException(nameof(SourceFolderPath), $"{nameof(SourceFolderPath)} cannot be NULL or Empty.");
			}

			if (string.IsNullOrWhiteSpace(DestinationZipFilePath))
			{
				throw new ArgumentNullException(nameof(DestinationZipFilePath), $"{nameof(DestinationZipFilePath)} cannot be NULL or Empty.");
			}

			FileInfo destinationZipFileInfo = new FileInfo(DestinationZipFilePath);
			if (destinationZipFileInfo.Directory != null)
			{ 
				if (SourceFolderPath == destinationZipFileInfo.Directory.FullName)
				{
					throw new ArgumentException(nameof(DestinationZipFilePath), $"{nameof(DestinationZipFilePath)} cannot be in the same folder as {nameof(SourceFolderPath)}.");
				}

				if (destinationZipFileInfo.Exists)
				{
					throw new ArgumentException(nameof(DestinationZipFilePath), $"{nameof(DestinationZipFilePath)} already exists.");
				}
			}
			else
			{
				throw new ArgumentException(nameof(DestinationZipFilePath), $"{nameof(DestinationZipFilePath)} must be a valid path.");
			}

			DirectoryInfo sourceFolderPathInfo = new DirectoryInfo(SourceFolderPath);
			if (!sourceFolderPathInfo.Exists)
			{
				throw new ArgumentException(nameof(SourceFolderPath), $"{nameof(SourceFolderPath)} must be a valid folder path");
			}
		}
	}
}
