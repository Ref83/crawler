using System.IO;

namespace WebCrawler.HtmlParser
{
    /// <summary>
    /// Фабрика HTML парсера
    /// </summary>
    public interface IHtmlParserFactory
    {
        /// <summary>
        /// Сделать Html парсер из потока
        /// </summary>
        /// <param name="stream">поток</param>
        /// <returns>Парсер HTML</returns>
        IHtmlParser MakeHtmlManagerFor(Stream stream);
    }
}
