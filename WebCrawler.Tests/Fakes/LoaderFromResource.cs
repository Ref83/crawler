using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebCrawler;

namespace WebCrawler.Tests.Fakes
{
    class LoaderFromResource : IWebLoader
    {
        #region Helpers
        private string GetResourcePathFor(string uri)
        {
            var baseUri = new Uri(uri);
            return baseUri.Host + (baseUri.LocalPath == "/" ? "/index.html" : baseUri.LocalPath).Replace("/", ".");
        }
        #endregion

        #region IWebLoader
        public async Task DownloadAsync(string uri, Stream stream)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourcePath = assembly.GetName().Name + ".ResourcesForTests." + GetResourcePathFor(uri);

            var realResourcePath = assembly.GetManifestResourceNames().SingleOrDefault(n => n.ToLower() == resourcePath.ToLower());
            if (String.IsNullOrWhiteSpace(realResourcePath))
                throw new InvalidOperationException("Не удалось найти ресурс " + resourcePath);

            using (Stream resourceStream = assembly.GetManifestResourceStream(realResourcePath))
            {
                await resourceStream.CopyToAsync(stream);
            }
        }
        #endregion
    }
}
