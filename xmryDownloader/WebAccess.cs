using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace xmryDownloader
{
    internal class WebAccess
    {
        private HttpClient _httpClient = new HttpClient();

        public WebAccess()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd("ting_6.3.60(sdk,Android16)");
            _httpClient.Timeout = new TimeSpan(0, 1, 30); //90秒
        }

        internal async Task<GetAlbumInfoResponse> Get(string url)
        {
            try
            {
                HttpResponseMessage resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                var byteData = await resp.Content.ReadAsByteArrayAsync();

                string json = Encoding.UTF8.GetString(byteData);
                var data = JsonSerializer.Deserialize<GetAlbumInfoResponse>(json);

                return data;
            }
            catch (Exception ex)
            {
                return new GetAlbumInfoResponse() { msg = $"Noops，系统出错了。{ex.Message}", ret= 99 };
            }
        }

        internal async Task DownloadFile(string url, string path,Action<int> Progress)
        {
            try
            {
                HttpResponseMessage resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();

                var cnt = resp.Headers.Server;

                //var byteData = await resp.Content.ReadAsByteArrayAsync();

                //string json = Encoding.UTF8.GetString(byteData);
                //var data = JsonSerializer.Deserialize<GetAlbumInfoResponse>(json);

                return ;
            }
            catch (Exception ex)
            {
                return ;
            }
        }
    }
}
