using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.HtmlParser
{
    /// <summary>
    /// Html аттрибут
    /// </summary>
    public interface IHtmlAttribute
    {
        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; }

       /// <summary>
       /// Значение
       /// </summary>
        string Value { get; set; }
    }
}
