using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebCrawler.Helpers;
using WebCrawler.Implimentations;
using WebCrawler.HtmlParser;
using WebCrawler.HtmlParser.Implementation;
using System.Collections.Concurrent;

namespace WebCrawler
{
    /// <summary>
    /// Загрузчик сайтов в локальную директорию
    /// </summary>
    public class Crawler
    {
        #region Members
        private IWebLoader loader;
        private IFileSystemWrapper fileSystem;
        private bool isNeedUploadOtherDomens;
        private Uri baseUri;
        private ConcurrentDictionary<string, string> map = new ConcurrentDictionary<string, string>();
        private IHtmlParserFactory parserFactory;
        #endregion

        #region Constructors
        public Crawler(IWebLoader loader, IFileSystemWrapper fileSystem, IHtmlParserFactory parserFactory, string uri, bool isNeedUploadOtherDomens = false, int parallelDownloadFactor = 10)
        {
            if (loader == null)
                throw new ArgumentNullException("loader");
            if (fileSystem == null)
                throw new ArgumentNullException("fileSystem");
            if (parserFactory == null)
                throw new ArgumentNullException("parserFactory");
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                throw new ArgumentException("Не верный формат uri " + uri);

            this.loader = new RestrictedWebLoader(loader, parallelDownloadFactor);
            this.fileSystem = fileSystem;
            this.parserFactory = parserFactory;
            this.isNeedUploadOtherDomens = isNeedUploadOtherDomens;
            this.baseUri = new Uri(uri);
        }
        #endregion

        #region Helpers
        private async Task ReferenceHandler(IHtmlNode referenceNode, int nestedLevel)
        {
            var referenceUri = new Uri(baseUri, referenceNode.Attributes["href"].Value);

            if ((referenceUri.Host.ToLower() == baseUri.Host.ToLower()) || (referenceUri.Host.ToLower() != baseUri.Host.ToLower() && isNeedUploadOtherDomens == true))
            {
                var localReference = Path.Combine("pages", "page" + referenceUri.ToString().GetHashCode().ToString() + ".html");
                if (nestedLevel > 1)
                    referenceNode.Attributes["href"].Value = localReference;

                await CrawlItAsync(referenceUri.ToString(), localReference, nestedLevel - 1);
            }
        }

        private async Task ResourceHandler(IHtmlNode referenceNode, string attributeName, string localPath, string prefix)
        {
            var referenceUri = new Uri(baseUri, referenceNode.Attributes[attributeName].Value);
            var localReference = Path.Combine(localPath, prefix + referenceUri.ToString().GetHashCode().ToString() + Path.GetExtension(referenceUri.LocalPath.Replace(@"/", @"\")));

            referenceNode.Attributes[attributeName].Value = localReference;

            if (map.TryAdd(referenceUri.ToString(), localReference))
            {
                using (var stream = fileSystem.OpenStreamFor(localReference))
                {
                    await loader.DownloadAsync(referenceUri.ToString(), stream);
                }
            }            
        }

        private async Task CrawlItAsync(string uri, string localPath, int nestedLevel)
        {
            if (nestedLevel == 0)
                return;

            if (map.TryAdd(uri, localPath))
            {
                try
                {
                    IHtmlParser parser;
                    using (var stream = new MemoryStream())
                    {
                        await loader.DownloadAsync(uri, stream);
                        stream.Position = 0;
                        parser = parserFactory.MakeHtmlManagerFor(stream);
                    }

                    IList<Task> tasks = new List<Task>();                    
                    
                    parser.DocumentNode.SelectNodes(@"//a[@href]").Where(n => !n.Attributes["href"].Value.Contains("#")).ToList().ForEach(node => tasks.Add(ReferenceHandler(node, nestedLevel)));
                    parser.DocumentNode.SelectNodes(@"//link[@href]").Where(n => n.Attributes["rel"].Value.Contains("stylesheet")).ToList().ForEach(node => tasks.Add(ResourceHandler(node, "href", "content", "stylesheet")));
                    parser.DocumentNode.SelectNodes(@"//script[@src]").ToList().ForEach(node => tasks.Add(ResourceHandler(node, "src", "scripts", "script")));
                    parser.DocumentNode.SelectNodes(@"//img[@src]").ToList().ForEach(node => tasks.Add(ResourceHandler(node, "src", "images", "image")));

                    using (var stream = fileSystem.OpenStreamFor(localPath))
                    {
                        try
                        {
                            await parser.SaveAsync(stream);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException("Ошибка сохранения файла " + localPath + ". " + ex.Message, ex);
                        }
                    }

                    Task.WaitAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    map[uri] = ex.Message;
                }
            }
        }

        internal static async Task<IDictionary<string, string>> CrawlAsync(string uri, int nestedLevel, bool isNeedUploadOtherDomens, IWebLoader loader, IFileSystemWrapper fileSystem, IHtmlParserFactory parserFactory, int parallelDownloadFactor = 10)
        {
            if (nestedLevel <= 0)
                throw new ArgumentOutOfRangeException("Параметр nestedLevel должен быть больше нуля");

            var crawler = new Crawler(loader, fileSystem, parserFactory, uri, isNeedUploadOtherDomens, parallelDownloadFactor);

            await crawler.CrawlItAsync(uri, "index.html", nestedLevel);

            return crawler.map;

        }
        #endregion

        #region Public Interface
        /// <summary>
        /// Загрузка сайта
        /// </summary>
        /// <param name="uri">uri сайта</param>
        /// <param name="nestedLevel">до какого уровня загружать ссылки</param>
        /// <param name="isNeedUploadOtherDomens">загружать ли ссылки с других дометнов</param>
        /// <param name="localPath">путь к локальной директории</param>
        /// <returns>Список загруженых файлов</returns>                 
        public static async Task<IDictionary<string, string>> CrawlAsync(string uri, string localPath, int nestedLevel, bool isNeedUploadOtherDomens = false, int parallelDownloadFactor = 10)
        {
            return await CrawlAsync(uri, nestedLevel, isNeedUploadOtherDomens, new WebLoader(), new FileSystemWrapper(localPath), new HtmlParserFactory(), parallelDownloadFactor);
        }
        #endregion
    }
}

