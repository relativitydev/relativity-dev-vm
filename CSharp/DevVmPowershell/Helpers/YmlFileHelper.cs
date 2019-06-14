using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public class YmlFileHelper : IYmlFileHelper
	{
		public void UpdateElasticSearchYml()
		{
			try
			{
				if (!File.Exists(Constants.YmlFile.YmlFilePath))
				{
					throw new Exception("File does not exist");
				}

				// Create Copy of File
				File.Copy(Constants.YmlFile.YmlFilePath, Constants.YmlFile.OriginalYmlFilePath);

				string[] lines = File.ReadAllLines(Constants.YmlFile.YmlFilePath);
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (line.Contains(Constants.YmlFile.DiscoveryZenPingUnicastHosts))
					{
						lines[i] = Constants.YmlFile.DiscoveryZenPingUnicastHostsValue;
					}

					if (line.Contains(Constants.YmlFile.ActionDestructiveRequiresName))
					{
						lines[i] = Constants.YmlFile.ActionDestructiveRequiresNameValue;
					}

					if (line.Contains(Constants.YmlFile.NetworkHost))
					{
						lines[i] = Constants.YmlFile.NetworkHostValue;
					}

					if (line.Contains(Constants.YmlFile.ShieldEnabled))
					{
						lines[i] = Constants.YmlFile.ShieldEnabledValue;
					}

					if (line.Contains(Constants.YmlFile.PublicJWKsUrl) && line.Contains("http") && !line.Contains("https"))
					{
						lines[i] = line.Replace("http", "https");
					}
				}

				// Move Yml File to Temporary Path
				File.Move(Constants.YmlFile.YmlFilePath, Constants.YmlFile.TempYmlFilePath);

				// Create Yml File
				FileStream stream = File.Create(Constants.YmlFile.YmlFilePath);
				stream.Close();
				
				// Write Updated Contents to Yml File
				File.WriteAllLines(Constants.YmlFile.YmlFilePath, lines);

				// Delete Temp Yml File
				File.Delete(Constants.YmlFile.TempYmlFilePath);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Updating Elastic Search Yml File", ex);
			}
		}
	}
}
