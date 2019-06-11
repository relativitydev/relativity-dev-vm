using kCura.Relativity.ImportAPI;
using Relativity.API;
using Relativity.Services.ServiceProxy;

namespace Helpers
{
	public interface IConnectionHelper
	{
		ServiceFactory GetServiceFactory(string protocol = Constants.Connection.PROTOCOL);

		ImportAPI GetImportApi();

		IDBContext GetDbContext(int workspaceId);
	}
}