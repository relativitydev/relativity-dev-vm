using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.Relativity.Client.DTOs;
using Relativity.API;
using Relativity.Services;
using Relativity.Services.InstanceSetting;
using Relativity.Services.ServiceProxy;

namespace Helpers
{
	public class InstanceSettingsHelper : IInstanceSettingsHelper
	{
		private ServiceFactory ServiceFactory { get; }

		public InstanceSettingsHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		public int CreateInstanceSetting(string section, string name, string description, string value)
		{
			try
			{
				using (IInstanceSettingManager instanceSettingManager = ServiceFactory.CreateProxy<IInstanceSettingManager>())
				{
					InstanceSetting newInstanceSetting = new InstanceSetting
					{
						Section = section,
						Name = name,
						Description = description,
						Value = value
					};
					int instanceSettingArtifactId = instanceSettingManager.CreateSingleAsync(newInstanceSetting).Result;
					Console.WriteLine("Successfully Created Instance Setting");
					return instanceSettingArtifactId;
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error Creating Instance Setting", ex);
			}
		}

		public bool UpdateInstanceSettingValue(string name, string section, string newValue)
		{
			try
			{
				if (newValue == null)
				{
					newValue = string.Empty;
				}
				using (IInstanceSettingManager instanceSettingManager = ServiceFactory.CreateProxy<IInstanceSettingManager>())
				{
					Query query = new Query();
					query.Condition = $"'Section' == '{section}' AND 'Name' == '{name}'";
					InstanceSettingQueryResultSet instanceSettingQueryResultSet = instanceSettingManager.QueryAsync(query).Result;
					if (instanceSettingQueryResultSet.Success)
					{
						Relativity.Services.Result<InstanceSetting> result = instanceSettingQueryResultSet.Results.First();
						if (result.Artifact != null)
						{
							Console.WriteLine("Successfully found the existing Instance Setting");
							InstanceSetting instanceSetting = result.Artifact;
							if (instanceSetting.Name == "ESIndexCreationSettings")
							{
								string previousValue = instanceSetting.Value;
								newValue = previousValue.Replace("\"number_of_shards\": 12", "\"number_of_shards\": 2");
								newValue = newValue.Replace("\"number_of_replicas\": 2", "\"number_of_replicas\": 0");
							}
							instanceSetting.Value = newValue;
							instanceSettingManager.UpdateSingleAsync(instanceSetting).Wait();
							Console.WriteLine("Successfully updated the Instance Setting");
							return true;
						}
					}
					Console.WriteLine("Failed to find the existing Instance Setting");
				}

				return false;
			}
			catch (Exception ex)
			{
				throw new Exception("Error Updating Instance Setting", ex);
			}
		}

		public void DeleteInstanceSetting(int instanceSettingArtifactId)
		{
			try
			{
				using (IInstanceSettingManager instanceSettingManager = ServiceFactory.CreateProxy<IInstanceSettingManager>())
				{
					instanceSettingManager.DeleteSingleAsync(instanceSettingArtifactId).Wait();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to Delete Instance Setting");
			}
		}
	}
}
