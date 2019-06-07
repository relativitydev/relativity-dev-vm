using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using Relativity.Services.ResourcePool;
using Relativity.Services.ServiceProxy;
using Relativity.Services;
using Relativity.Services.ApplicationInstallManager;
using Relativity.Services.ApplicationInstallManager.Models;
using Relativity.Services.LibraryApplicationsManager;

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
							Console.WriteLine("Application Installation completed with errors");
						}
						else if (state.State == ProcessStateValue.HandledException || state.State == ProcessStateValue.UnhandledException)
						{
							installationResult = false;
							Console.WriteLine("Application Installation failed with an unknown error");
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

		public bool InstallApplicationFromApplicationLibrary(int workspaceId, string applicationGuid)
		{
			Guid appGuid = new Guid(applicationGuid);
			// ILibraryApplicationsManager is a private kepler service
			using (ILibraryApplicationsManager libraryApplicationsManager = ServiceFactory.CreateProxy<ILibraryApplicationsManager>())
			{
				List<LibraryApplication> libraryApplications = libraryApplicationsManager.GetAllLibraryApplicationsAsync().Result.ToList();
				if (libraryApplications.Any(x => x.GUID == appGuid))
				{
					Console.WriteLine("Application is installed in the Application Library");
				}
				else
				{
					Console.WriteLine("Application is not installed in the Application Library");
					return false;
				}
			}

			// IApplicationInstallManager is a private kepler service
			using (IApplicationInstallManager applicationInstallManager = ServiceFactory.CreateProxy<IApplicationInstallManager>())
			{
				try
				{
					// Check if the application is already installed in the workspace
					ApplicationInstallStatus applicationInstallStatus = applicationInstallManager.GetApplicationInstallStatusAsync(workspaceId, appGuid).Result;
					if (applicationInstallStatus != ApplicationInstallStatus.Installed)
					{
						applicationInstallManager.InstallLibraryApplicationByGuid(workspaceId, appGuid).Wait();
						Console.WriteLine($"Application is successfully installed in the workspace from the Application Library");
						return true;
					}
					else
					{
						Console.WriteLine("Application is already installed in the workspace");
						return true;
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error Installing Application From the Application Library", ex);
				}
			}
		}
	}
}
