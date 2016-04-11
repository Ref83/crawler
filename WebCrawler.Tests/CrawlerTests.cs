using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using WebCrawler.Tests.Fakes;
using WebCrawler.Implimentations;
using Moq;
using System.IO;
using WebCrawler.Tests.Helpers;
using System.Linq;
using System;
using System.Collections.Generic;
using WebCrawler.HtmlParser.Implementation;

namespace WebCrawler.Tests
{
    [TestClass]
    public class CrawlerTests
    {
        #region Members
        private IWebLoader loader;
        private Mock<IWebLoader> loaderMoq;
        private Mock<IFileSystemWrapper> fileSystemMoq;
        #endregion

        #region Fixture
        [TestInitialize]
        public void Initialize()
        {
            loader = new LoaderFromResource();
            loaderMoq = new Mock<IWebLoader>();
            loaderMoq.Setup(m => m.DownloadAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns<string, Stream>((uri, stream) => loader.DownloadAsync(uri, stream));
            fileSystemMoq = new Mock<IFileSystemWrapper>();
            fileSystemMoq.Setup(m => m.OpenStreamFor(It.IsAny<string>())).Returns(() => new MemoryStream());
        }
        #endregion

        #region Helpers
        private async Task<IDictionary<string, string>>CrawlAsync(string uri, int nestedLevel, bool isNeedUploadOtherDomens)
        {
            return await Crawler.CrawlAsync(uri, nestedLevel, isNeedUploadOtherDomens, loaderMoq.Object, fileSystemMoq.Object, new HtmlParserFactory(), 10);
        }
        #endregion

        [TestMethod]
        public async Task Can_Upload_One_Page_Site()
        {
            var siteMap = await CrawlAsync("http://OnePage.com", 1, false);
            AssertExtensions.AreListEqual(new[] { "http://OnePage.com" }, siteMap.Keys);
        }

        [TestMethod]
        public async Task Can_Upload_Site_With_Referencies()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com", 2, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com",
                    "http://sitewithreferencies.com/pages/page1.html",
                    "http://sitewithreferencies.com/pages/page2.html"
                    }, 
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Can_Handle_Cicle_Referencies()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithCicleReferencies.html", 5, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com/pages/ciclereferencepage.html",
                    "http://sitewithreferencies.com/pagewithciclereferencies.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Can_Handle_Nested_Referencies()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithNestedReferencies.html", 5, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com/pages/page1.html",
                    "http://sitewithreferencies.com/pages/page2.html",
                    "http://sitewithreferencies.com/pagewithnestedreferencies.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Skip_Equal_Referencies()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithEqualReferencies.html", 5, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com/pages/page1.html",
                    "http://sitewithreferencies.com/pages/page2.html",
                    "http://sitewithreferencies.com/pagewithequalreferencies.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));

            loaderMoq.Verify(m => m.DownloadAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Exactly(3));
        }

        [TestMethod]
        public async Task Skip_Referencies_From_Other_Domain_If_Flag_Is_Reset()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithReferenciesToOtherDomain.html", 5, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com/pages/page2.html",
                    "http://sitewithreferencies.com/pagewithreferenciestootherdomain.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Upload_Referencies_From_Other_Domain_If_Flag_Is_Set()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithReferenciesToOtherDomain.html", 5, true);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://onepage.com/index.html",
                    "http://sitewithreferencies.com/pages/page2.html",
                    "http://sitewithreferencies.com/pagewithreferenciestootherdomain.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Can_Restrict_Nested_Level()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithFourNestedReferensies.html", 4, true);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com/pages/firstlevelpage.html",
                    "http://sitewithreferencies.com/pages/secondlevelpage.html",
                    "http://sitewithreferencies.com/pages/thirdlevelpage.html",
                    "http://sitewithreferencies.com/pagewithfournestedreferensies.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Can_Upload_Site_Resources()
        {
            var siteMap = await CrawlAsync("http://complexsite.com", 2, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://complexsite.com",
                    "http://complexsite.com/content/other.css",
                    "http://complexsite.com/content/test.css",
                    "http://complexsite.com/images/parovoz.jpg",
                    "http://complexsite.com/pages/page1.html",
                    "http://complexsite.com/scripts/otherscript.js",
                    "http://complexsite.com/scripts/script.js",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));
        }

        [TestMethod]
        public async Task Skip_Anchors()
        {
            var siteMap = await CrawlAsync("http://sitewithreferencies.com/pageWithAnchors.html", 5, false);
            AssertExtensions.AreListEqual(
                new[] {
                    "http://sitewithreferencies.com/pages/page1.html",
                    "http://sitewithreferencies.com/pagewithanchors.html",
                    },
                siteMap.Keys.Select(v => v.ToLower()).OrderBy(s => s));

            loaderMoq.Verify(m => m.DownloadAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Exactly(2));
        }

        #region Exception Tests
        [TestMethod]
        public async Task Error_When_Pass_Negative_Or_Zerro_Nested_Level()
        {
            await AssertExtensions.IsExceptionAsync(
                async () => await CrawlAsync("http://test.com", -4, false), 
                typeof(ArgumentOutOfRangeException)
                );

            await AssertExtensions.IsExceptionAsync(
                async () => await CrawlAsync("http://test.com", 0, false),
                typeof(ArgumentOutOfRangeException)
                );
        }

        [TestMethod]
        public async Task Error_When_Pass_Incorrect_Uri()
        {
            await AssertExtensions.IsExceptionAsync(
                async () => await CrawlAsync("|{}!", 4, false),
                typeof(ArgumentException)
                );
        }
        #endregion

    }
}
