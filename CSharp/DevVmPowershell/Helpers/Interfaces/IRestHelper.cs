using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IRestHelper
	{
		HttpClient GetHttpClient(string instanceAddress, string adminUsername, string adminPassword);
		Task<HttpResponseMessage> MakeGetAsync(HttpClient httpClient, string url);
		Task<HttpResponseMessage> MakePostAsync(HttpClient httpClient, string url, string request);
		Task<HttpResponseMessage> MakePutAsync(HttpClient httpClient, string url, string request);
		Task<HttpResponseMessage> MakePutAsync(HttpClient httpClient, string url, HttpContent content);
		Task<HttpResponseMessage> MakeDeleteAsync(HttpClient httpClient, string url);
		StreamContent CreateFileContent(Stream stream, string fileName, string contentType);
	}
}
