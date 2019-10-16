using System;
using System.IO;
using System.IO.Compression;

namespace Helpers
{
	public class ZipHelper : IZipHelper
	{
		public void ZipFolder(string sourceFolderPath, string destinationZipFilePath)
		{
			try
			{
				if (Directory.Exists(sourceFolderPath))
				{
					if (File.Exists(destinationZipFilePath))
					{
						Console.WriteLine($"Previous zip file exists. [{nameof(destinationZipFilePath)}: {destinationZipFilePath}]");
						File.Delete(destinationZipFilePath);
						Console.WriteLine($"Deleted Previous zip file. [{nameof(destinationZipFilePath)}: {destinationZipFilePath}]");
					}

					Console.WriteLine($"Compressing File... [{nameof(destinationZipFilePath)}: {destinationZipFilePath}]");
					ZipFile.CreateFromDirectory(sourceFolderPath, destinationZipFilePath);
					Console.WriteLine($"Compressed File! [{nameof(destinationZipFilePath)}: {destinationZipFilePath}]");

					if (File.Exists(destinationZipFilePath))
					{
						Console.WriteLine("Successfully Zipped Folder");
					}
					else
					{
						throw new Exception("An error occurred zipping the folder.");
					}
				}
				else
				{
					Console.WriteLine($"Source folder doesn't exist. [{nameof(sourceFolderPath)}: {sourceFolderPath}]");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating a zip file for a folder.", ex);
			}
		}
	}
}
