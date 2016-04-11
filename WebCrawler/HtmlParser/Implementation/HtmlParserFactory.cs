using System.IO;
using WebCrawler.HtmlParser;

namespace WebCrawler.HtmlParser.Implementation
{
    class HtmlParserFactory : IHtmlParserFactory
    {
        #region IHtmlParserFactory
        public IHtmlParser MakeHtmlManagerFor(Stream stream)
        {
            return new HtmlParser(stream);
        }
        #endregion
    }
}