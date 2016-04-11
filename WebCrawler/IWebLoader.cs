using System.IO;
using System.Threading.Tasks;

namespace WebCrawler
{
    /// <summary>
    /// Загрузчик данных
    /// </summary>
    public interface IWebLoader
    {
        /// <summary>
        /// Загрузка ресурса
        /// </summary>
        /// <param name="uri">uri ресурса</param>
        /// <param name="stream">Поток для получения данных</param>
        Task DownloadAsync(string uri, Stream stream);
    }
}
