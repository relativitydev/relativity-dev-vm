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
		public static HttpClient GetHttpClient(string instanceAddress, string adminUsername, string adminPassword)
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

		public static HttpResponseMessage MakePost(HttpClient httpClient, string url, string request)
		{
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
			return response;
		}

		public static HttpResponseMessage MakePut(HttpClient httpClient, string url, string request)
		{
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = httpClient.PutAsync(url, content).Result;
			return response;
		}
	}
}
