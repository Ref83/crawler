using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace WebCrawler.HtmlParser.Implementation
{
    internal class Attribute : IHtmlAttribute
    {
        #region Members
        private HtmlAttribute attribute;
        #endregion

        #region Constructors
        public Attribute(HtmlAttribute attribute)
        {
            this.attribute = attribute;
        }
        #endregion

        #region IHtmlAttribute
        public string Name
        {
            get
            {
                return attribute.Name;
            }
        }

        public string Value
        {
            get
            {
                return attribute.Value;
            }

            set
            {
                attribute.Value = value;
            }
        }
        #endregion
    }
}
