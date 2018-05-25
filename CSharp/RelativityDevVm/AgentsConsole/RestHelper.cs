using AgentsConsole.CustomExceptions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AgentsConsole
{
	public class RestHelper
	{
		private readonly string _relativityAdminUsername;
		private readonly string _relativityAdminPassword;

		public RestHelper(string relativityAdminUsername, string relativityAdminPassword)
		{
			_relativityAdminUsername = relativityAdminUsername;
			_relativityAdminPassword = relativityAdminPassword;
		}

		private string GetBearerTokenString()
		{
			byte[] authByteArray = Encoding.ASCII.GetBytes($"{_relativityAdminUsername}:{_relativityAdminPassword}");
			string authBase64String = Convert.ToBase64String(authByteArray);
			return authBase64String;
		}

		public async Task<string> PerformRestPostCallAsync(string url, string requestJsonString)
		{
			try
			{
				using (HttpClient httpClient = new HttpClient())
				{
					string bearerTokenString = GetBearerTokenString();
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", bearerTokenString);
					httpClient.DefaultRequestHeaders.Add("x-csrf-header", " ");
					Uri requestUri = new Uri(url);
					StringContent requestJsonStringContent = new StringContent(requestJsonString, Encoding.UTF8, "application/json");

					using (HttpResponseMessage httpResponseMessage = await httpClient.PostAsync(requestUri, requestJsonStringContent))
					{
						using (HttpContent httpContent = httpResponseMessage.Content)
						{
							string resultString;
							try
							{
								resultString = await httpContent.ReadAsStringAsync();
							}
							catch (Exception ex)
							{
								throw new AgentsConsoleException($"{Constants.ErrorMessages.Rest.RestPostCallError}. ReadAsStringAsync.", ex);
							}

							if (resultString == null)
							{
								throw new AgentsConsoleException(Constants.ErrorMessages.Rest.RestPostCallNullResultError);
							}

							Console.WriteLine($"REST Result: {resultString}");
							return resultString;
						}
					}
				}
			}
			catch (Exception ex)
			{
				throw new AgentsConsoleException(Constants.ErrorMessages.Rest.RestPostCallError, ex);
			}
		}
	}
}
