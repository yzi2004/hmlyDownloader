using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<GetAlbumInfoResponse> Get(string url)
        {
            url = $"http://mobile.ximalaya.com/mobile-album/album/page/ts-{Utils.GetUnixTime()}?ac=WIFI&albumId=2780581&device=android&pageId=1&pageSize=0";
            HttpResponseMessage resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var byteData = await resp.Content.ReadAsByteArrayAsync();

            string json = Encoding.UTF8.GetString(byteData);

            var data = JsonSerializer.Deserialize<GetAlbumInfoResponse>(json);

            return data;
        }
    }
}
