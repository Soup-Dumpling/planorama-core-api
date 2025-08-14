using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Planorama.User.Core.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient httpClient;

        public HttpService(HttpClient httpClient) 
        {
            this.httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string url, string token)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            var getUserRequest = await httpClient.GetAsync(url);
            getUserRequest.EnsureSuccessStatusCode();
            var responseString = await getUserRequest.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<T>(responseString);
            return response;
        }
    }
}
