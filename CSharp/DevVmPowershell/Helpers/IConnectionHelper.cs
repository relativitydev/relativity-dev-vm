using Relativity.Services.ServiceProxy;

namespace Helpers
{
	public interface IConnectionHelper
	{
		ServiceFactory GetServiceFactory(string protocol = Constants.Connection.PROTOCOL);

		string GetUserName();
		string GetPassword();
		string GetInstanceName();
	}
}