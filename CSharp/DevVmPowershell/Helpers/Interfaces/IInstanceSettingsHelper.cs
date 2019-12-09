namespace Helpers.Interfaces
{
	public interface IInstanceSettingsHelper
	{
		int CreateInstanceSetting(string name, string section, string description, string value);
		bool UpdateInstanceSettingValue(string name, string section, string newValue);
		void DeleteInstanceSetting(int instanceSettingArtifactId);
		string GetInstanceSettingValue(string name, string section);
		int GetInstanceSettingArtifactIdByName(string name, string section);
	}
}
