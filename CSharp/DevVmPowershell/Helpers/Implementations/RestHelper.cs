using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Implementations
{
	public class RestHelper
	{
		public HttpClient GetHttpClient(string instanceAddress, string adminUsername, string adminPassword)
		{
			//Set up the client
			HttpClient httpClient = new HttpClient
			{
				BaseAddress = new Uri(Constants.Connection.PROTOCOL + "://" + instanceAddress + "/Relativity")
			};

			string encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(adminUsername + ":" + adminPassword));

			//Set the required headers.
			httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {encoded}");

			return httpClient;
		}

		public async Task<HttpResponseMessage> MakePostAsync(HttpClient httpClient, string url, string request)
		{
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await httpClient.PostAsync(url, content);
			return response;
		}

		public static async Task<HttpResponseMessage> MakePutAsync(HttpClient httpClient, string url, string request)
		{
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await httpClient.PutAsync(url, content);
			return response;
		}

		public async Task<HttpResponseMessage> MakeDeleteAsync(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = await httpClient.DeleteAsync(url);
			return response;
		}
	}
}
