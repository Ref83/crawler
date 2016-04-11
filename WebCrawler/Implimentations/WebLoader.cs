using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Implimentations
{
    internal class WebLoader : IWebLoader
    {
        #region IWebLoader
        public async Task DownloadAsync(string uri, Stream stream)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var buffer = await client.DownloadDataTaskAsync(uri);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка загрузки ресурса " + uri, ex);
            }           
        }
        #endregion
    }
}
