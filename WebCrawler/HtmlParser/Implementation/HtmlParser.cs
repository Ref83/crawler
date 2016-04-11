using System.IO;
using System.Collections.Generic;
using System.Linq;
using WebCrawler.HtmlParser;
using HtmlAgilityPack;
using System;
using System.Threading.Tasks;

namespace WebCrawler.HtmlParser.Implementation
{
    class HtmlParser : IHtmlParser
    {
        #region Members
        private HtmlDocument document = new HtmlDocument();
        private IHtmlNode documentNode;
        #endregion

        #region Constructors 
        public HtmlParser(Stream stream)
        {
            document.Load(stream);
        }
        #endregion

        #region IHtmlParser
        public IHtmlNode DocumentNode
        {
            get
            {
                if (documentNode == null)
                {
                    documentNode = new Node(document.DocumentNode);
                }

                return documentNode;
            }
        }


        public async Task SaveAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            await Task.Factory.StartNew(() => document.Save(stream));
        }
        #endregion
    }
}