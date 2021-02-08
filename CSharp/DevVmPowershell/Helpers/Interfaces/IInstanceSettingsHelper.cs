using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IInstanceSettingsHelper
	{
		Task<int> CreateInstanceSettingAsync(string name, string section, string description, string value);
		Task<bool> UpdateInstanceSettingValueAsync(string name, string section, string newValue);
		Task DeleteInstanceSettingAsync(int instanceSettingArtifactId);
		string GetInstanceSettingValue(string name, string section);
		int GetInstanceSettingArtifactIdByName(string name, string section);
	}
}
