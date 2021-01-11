using System;
using System.IO;
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

		public static async Task<HttpResponseMessage> MakeGetAsync(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = await httpClient.GetAsync(url);
			return response;
		}

		public static async Task<HttpResponseMessage> MakePostAsync(HttpClient httpClient, string url, string request)
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

		public static async Task<HttpResponseMessage> MakePutAsync(HttpClient httpClient, string url, HttpContent content)
		{
			HttpResponseMessage response = await httpClient.PutAsync(url, content);
			return response;
		}

		public static async Task<HttpResponseMessage> MakeDeleteAsync(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = await httpClient.DeleteAsync(url);
			return response;
		}

		public static StreamContent CreateFileContent(Stream stream, string fileName, string contentType)
		{
			var fileContent = new StreamContent(stream);
			fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
			{
				Name = "\"" + Constants.Connection.RestUrlEndpoints.ApplicationInstall.uploadFileKeyName + "\"",
				FileName = "\"" + fileName + "\""
			}; // the extra quotes are key here
			fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
			return fileContent;
		}
	}
}
