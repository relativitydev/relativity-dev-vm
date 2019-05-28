namespace Helpers
{
	public interface IApplicationHelper
	{
		int GetRelativityApplicationArtifactId(kCura.Relativity.Client.IRSAPIClient rsapiClient, int applicationName);
	}
}