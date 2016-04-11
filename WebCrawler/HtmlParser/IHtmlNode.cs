using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.HtmlParser
{
    /// <summary>
    /// Элемент Html
    /// </summary>
    public interface IHtmlNode
    {
        /// <summary>
        /// Наименование
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Список аттрибутов элемента
        /// </summary>
        IDictionary<string, IHtmlAttribute> Attributes { get; }

        /// <summary>
        /// Выборка элементов по xpath
        /// </summary>
        /// <param name="xpath">xpath путь для выборки</param>
        /// <returns>Список элементов</returns>
        IEnumerable<IHtmlNode> SelectNodes(string xpath);
    }
}
