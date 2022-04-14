using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task<GetAlbumInfoResponse> GetAlbumTrackList(string albumID, bool ascFlg = true)
        {
            string url = $"https://mobile.ximalaya.com/mobile/v1/album/track/ts-{Utils.GetUnixTime()}?ac=WIFI&albumId={albumID}&device=android&isAsc={ascFlg}&pageId=1&pageSize=200";

            return await Get(url);
        }

        private async Task<GetAlbumInfoResponse> Get(string url)
        {
            HttpResponseMessage resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var byteData = await resp.Content.ReadAsByteArrayAsync();

            string json = Encoding.UTF8.GetString(byteData);
            File.AppendAllText("c:\\temp\\aaa.json", json);
            var data = JsonSerializer.Deserialize<GetAlbumInfoResponse>(json);

            return data;
        }
    }
}
