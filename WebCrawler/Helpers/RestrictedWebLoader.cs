using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebCrawler.Helpers
{
    internal class RestrictedWebLoader : IWebLoader
    {
        #region Members
        private AutoResetEvent synchronizer = new AutoResetEvent(false);
        private int parallelRestrictNumber;
        private int threadCount = 0;
        private int activeThreadCount = 0;
        private IWebLoader loader;
        #endregion

        #region Constructor
        public RestrictedWebLoader(IWebLoader loader, int parallelRestrictNumber)
        {
            this.loader = loader;
            this.parallelRestrictNumber = parallelRestrictNumber;
        }
        #endregion

        #region IWebLoader
        public async Task DownloadAsync(string uri, Stream stream)
        {

            if (Interlocked.Increment(ref threadCount) >= parallelRestrictNumber)
                synchronizer.WaitOne();

            try
            {
                Interlocked.Increment(ref activeThreadCount);
                await loader.DownloadAsync(uri, stream);
                
            }
            finally
            {
                Interlocked.Decrement(ref activeThreadCount);
                Interlocked.Decrement(ref threadCount);
                synchronizer.Set();
            }
        }
        #endregion
    }
}
