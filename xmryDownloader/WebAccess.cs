using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace xmryDownloader
{
    internal class WebAccess
    {
        private HttpClient _httpClient = new HttpClient();
        private const int BUF_SIZE = 8192;

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
                return new GetAlbumInfoResponse() { msg = $"Noops，系统出错了。{ex.Message}", ret = 99 };
            }
        }

        internal async Task<long> GetContentsSize(string url)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Accept.TryParseAdd("application/octet-stream");

                HttpResponseMessage resp = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();

                var totalSize = resp.Content.Headers.ContentLength;

                return totalSize ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception("不好意思，取不到下载内容的size。", ex);
            }
        }

        internal async Task DownloadFile(string url, string destFile, Action<long, long, bool> ReportProgress)
        {
            try
            {
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                requestMessage.Headers.Accept.TryParseAdd("application/octet-stream");

                HttpResponseMessage resp = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();

                var totalSize = resp.Content.Headers.ContentLength;
                var totalRead = 0;

                byte[] buf = new byte[BUF_SIZE];
                FileStream fs = new FileStream(destFile, FileMode.CreateNew);

                using (var contentStream = await resp.Content.ReadAsStreamAsync())
                {
                    int bytesRead = await contentStream.ReadAsync(buf, 0, BUF_SIZE);
                    while (bytesRead > 0)
                    {
                        totalRead += bytesRead;
                        ReportProgress?.Invoke(totalSize ?? 0, totalRead, false);
                        fs.Write(buf, 0, bytesRead);
                        bytesRead = await contentStream.ReadAsync(buf, 0, BUF_SIZE);
                    }

                    ReportProgress(totalSize ?? 0, totalRead, true);
                    fs.Flush(true);
                    fs.Close();

                    contentStream.Close();
                }

                return;
            }
            catch (Exception ex)
            {
                throw new Exception("文件下载出错了。", ex);
            }
        }
    }
}
