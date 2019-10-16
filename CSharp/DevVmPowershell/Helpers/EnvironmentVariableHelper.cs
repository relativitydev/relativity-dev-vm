using System;
using System.IO;
using System.Linq;

namespace Helpers
{
	public class EnvironmentVariableHelper : IEnvironmentVariableHelper
	{
		public void UpdateJavaEnvironmentVariables()
		{
			try
			{
				// Get the full Java Path
				string[] directories = Directory.GetDirectories(Constants.EnvironmentVariables.JAVA_INSTALL_PATH);
				if (directories.Length == 0)
				{
					throw new Exception("Java Path is invalid");
				}
				string fullJavaPath = directories.First();

				// Update Java Environment Variables
				Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.KCURA_JAVA_HOME, fullJavaPath, EnvironmentVariableTarget.Machine);
				Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.JAVA_HOME, fullJavaPath, EnvironmentVariableTarget.Machine);

				string kcuraJavaVariableValue = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.KCURA_JAVA_HOME, EnvironmentVariableTarget.Machine);
				string javaVariableValue = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.JAVA_HOME, EnvironmentVariableTarget.Machine);
				if (kcuraJavaVariableValue != fullJavaPath || javaVariableValue != fullJavaPath)
				{
					throw new Exception("Did not successfully update the Java Environment Variables");
				}

				Console.WriteLine("Successfully Updated the Java Environment Variables");
			}
			catch (Exception)
			{
				throw new Exception("Error Updating the Java Environment Variables");
			}
		}
	}
}
