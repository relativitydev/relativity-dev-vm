using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
	public class EnvironmentVariableHelper : IEnvironmentVariableHelper
	{
		public void UpdateJavaEnvironmentVariables()
		{
			try
			{
				// Get the full Java Path
				string[] directories = Directory.GetDirectories(Constants.EnvironmentVariables.JavaPath);
				if (directories.Length == 0)
				{
					throw new Exception("Java Path is invalid");
				}
				string fullJavaPath = directories.First();

				// Update Java Environment Variables
				Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.KcuraJavaHome, fullJavaPath, EnvironmentVariableTarget.Machine);
				Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.JavaHome, fullJavaPath, EnvironmentVariableTarget.Machine);

				string kcuraJavaVariableValue = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.KcuraJavaHome, EnvironmentVariableTarget.Machine);
				string javaVariableValue = Environment.GetEnvironmentVariable(Constants.EnvironmentVariables.JavaHome, EnvironmentVariableTarget.Machine);
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
