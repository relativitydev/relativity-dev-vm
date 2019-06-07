using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using Relativity.Services.ServiceProxy;

namespace Helpers
{
	public class ApplicationInstallHelper : IApplicationInstallHelper
	{
		private ServiceFactory ServiceFactory { get; }
		public ApplicationInstallHelper(IConnectionHelper connectionHelper)
		{
			ServiceFactory = connectionHelper.GetServiceFactory();
		}

		public bool InstallApplicationFromRapFile(int workspaceId, string filePath)
		{
			using (IRSAPIClient proxy = ServiceFactory.CreateProxy<IRSAPIClient>())
			{
				proxy.APIOptions.WorkspaceID = workspaceId;
				AppInstallRequest appInstallRequest = new AppInstallRequest();
				appInstallRequest.FullFilePath = filePath;
				appInstallRequest.ForceFlag = true;
				try
				{
					bool installationResult = true;
					ProcessOperationResult processOperationResult = proxy.InstallApplication(proxy.APIOptions, appInstallRequest);
					if (!processOperationResult.Success)
					{
						installationResult = false;
						Console.WriteLine("An error occured Installing the Application");
					}
					else
					{
						ProcessInformation state;
						do
						{
							Thread.Sleep(10);
							state = proxy.GetProcessState(proxy.APIOptions, processOperationResult.ProcessID);

						} while (state.State == ProcessStateValue.Running);

						if (state.State == ProcessStateValue.CompletedWithError)
						{
							installationResult = false;
							Console.WriteLine("Application Installation completed with Errors");
						}
						else if (state.State == ProcessStateValue.HandledException || state.State == ProcessStateValue.UnhandledException)
						{
							installationResult = false;
							Console.WriteLine("Application Installation failed with an unknown Error");
						}
						else
						{
							Console.WriteLine("Application Installation Succeeded");
						}
					}

					return installationResult;
				}
				catch (Exception ex)
				{
					throw new Exception("An error occured installing an application from a RAP file", ex);
				}
			}
		}
	}
}
