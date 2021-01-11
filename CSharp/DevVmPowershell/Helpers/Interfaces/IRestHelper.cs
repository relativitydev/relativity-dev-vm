using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Interfaces
{
	public interface IRestHelper
	{
		HttpClient GetHttpClient(string instanceAddress, string adminUsername, string adminPassword);
		Task<HttpResponseMessage> MakePostAsync(HttpClient httpClient, string url, string request);
		Task<HttpResponseMessage> MakePutAsync(HttpClient httpClient, string url, string request);
		Task<HttpResponseMessage> MakeDeleteAsync(HttpClient httpClient, string url);
	}
}
