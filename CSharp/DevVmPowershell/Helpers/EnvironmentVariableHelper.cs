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
				string javaPath = @"C:\Program Files\Java";
				string[] directories = Directory.GetDirectories(javaPath);
				if (directories.Length == 0)
				{
					throw new Exception("Java Path is invalid");
				}

				string fullJavaPath = directories.First();
				Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.KcuraJavaHome, fullJavaPath, EnvironmentVariableTarget.Machine);
				Environment.SetEnvironmentVariable(Constants.EnvironmentVariables.JavaHome, fullJavaPath, EnvironmentVariableTarget.Machine);
			}
			catch (Exception)
			{
				throw new Exception("Error Updating the Java Environment Variables");
			}
		}
	}
}
