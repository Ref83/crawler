using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebCrawler.HtmlParser
{
    /// <summary>
    /// Парсер HTML
    /// </summary>
    public interface IHtmlParser
    {
        /// <summary>
        /// Головной элемент Html
        /// </summary>
        IHtmlNode DocumentNode { get; }

        /// <summary>
        /// Сохранение данных в потоке
        /// </summary>
        /// <param name="stream">Поток для сохранения данных</param>
        Task SaveAsync(Stream stream);
    }
}
