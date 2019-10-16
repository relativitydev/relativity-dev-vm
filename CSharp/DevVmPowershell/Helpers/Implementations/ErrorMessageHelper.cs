using System;

namespace Helpers.Implementations
{
	public class ErrorMessageHelper
	{
		public static string FormatErrorMessage(Exception exception)
		{
			string message = string.Empty;

			if (exception != null)
			{
				message = exception.Message;
				Exception innerException = exception.InnerException;
				while (innerException != null)
				{
					message = message + ", " + innerException.Message;
					innerException = innerException.InnerException;
				}
			}

			return message;
		}
	}
}
