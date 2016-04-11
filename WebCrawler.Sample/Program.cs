using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler;

namespace WebCrawler.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //var crawler = new Crawler(@"C:\Temp");
            var task = Crawler.CrawlAsync("http://www.se-radio.net/", @"C:\Temp", 2, false);
            task.Wait();

            foreach (var uri in task.Result)
            {
                Console.WriteLine(uri.Key + " - " + uri.Value);
            }

            Console.ReadLine();
        }
    }
}
