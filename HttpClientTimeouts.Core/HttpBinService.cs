using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientTimeouts.Core
{
    public class HttpBinService : IExternalService
    {
        private readonly HttpClient _httpClient;

        public HttpBinService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendAsync(byte[] payload)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://httpbin.org/post")))
            {
                request.Content = new ByteArrayContent(payload);

                using (HttpResponseMessage response = await _httpClient.SendAsync(request))
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    response.EnsureSuccessStatusCode();

                    return responseString;
                }
            }
        }
    }
}
