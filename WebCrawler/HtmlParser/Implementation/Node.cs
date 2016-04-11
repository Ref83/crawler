using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebCrawler.HtmlParser.Implementation
{
    internal class Node : IHtmlNode
    {
        #region Members
        private HtmlNode node;
        private IDictionary<string, IHtmlAttribute> attributes;
        #endregion

        #region Constructors
        public Node(HtmlNode node)
        {
            this.node = node;
        }
        #endregion    

        #region IHtmlNode
        public IDictionary<string, IHtmlAttribute> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    attributes = node.Attributes.ToDictionary(a => a.Name, a => new Attribute(a) as IHtmlAttribute);
                }
                return attributes;
            }
        }

        public string Name
        {
            get
            {
                return node.Name;
            }
        }

        public IEnumerable<IHtmlNode> SelectNodes(string xpath)
        {
            var nodes = node.SelectNodes(xpath);            
            var nativeNodes = nodes != null ? nodes.Select(n => new Node(n)).ToList() : new List<Node>();

            return nativeNodes;
        }
        #endregion
    }
}
