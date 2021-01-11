using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Helpers.RequestModels;

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

		public static HttpResponseMessage MakeGet(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = httpClient.GetAsync(url).Result;
			return response;
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

		public static HttpResponseMessage MakePut(HttpClient httpClient, string url, HttpContent content)
		{
			HttpResponseMessage response = httpClient.PutAsync(url, content).Result;
			return response;
		}

		public static HttpResponseMessage MakeDelete(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
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
