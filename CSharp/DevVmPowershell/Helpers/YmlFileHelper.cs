using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Helpers
{
	public class YmlFileHelper : IYmlFileHelper
	{
		public void UpdateElasticSearchYml()
		{
			try
			{
				string pathToYmlFile = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch.yml";
				string tempYmlFilePath = @"C:\RelativityDataGrid\elasticsearch-main\config\elasticsearch2.yml";
				if (!File.Exists(pathToYmlFile))
				{
					throw new Exception("File does not exist");
				}

				string[] lines = File.ReadAllLines(pathToYmlFile);
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					if (line.Contains("discovery.zen.ping.unicast.hosts"))
					{
						lines[i] = "discovery.zen.ping.unicast.hosts: [\"RELATIVITYDEVVM\"]";
					}

					if (line.Contains("action.destructive_requires_name"))
					{
						lines[i] = "action.destructive_requires_name: false";
					}

					if (line.Contains("network.host"))
					{
						lines[i] = "network.host: RELATIVITYDEVVM";
					}

					if (line.Contains("shield.enabled"))
					{
						lines[i] = "shield.enabled: false";
					}

					if (line.Contains("publicJWKsUrl") && line.Contains("http") && !line.Contains("https"))
					{
						lines[i] = line.Replace("http", "https");
					}
				}

				// Move Yml File to Temporary Path
				File.Move(pathToYmlFile, tempYmlFilePath);

				// Create Yml File
				FileStream stream = File.Create(pathToYmlFile);
				stream.Close();
				
				// Write update contents to Yml File
				File.WriteAllLines(pathToYmlFile, lines);

				// Delete Temp Yml File
				File.Delete(tempYmlFilePath);
			}
			catch (Exception ex)
			{
				throw new Exception("Error Updating Elastic Search Yml File", ex);
			}
		}
	}
}
