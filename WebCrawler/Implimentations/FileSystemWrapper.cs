using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Implimentations
{
    internal class FileSystemWrapper : IFileSystemWrapper
    {
        #region Members
        private string basePath;
        #endregion

        #region Contructors
        public FileSystemWrapper(string basePath)
        {
            this.basePath = basePath;
        }
        #endregion

        #region IFileSystemWrapper
        public Stream OpenStreamFor(string localPath)
        {
            var absolutePath = Path.Combine(basePath, localPath);
            var absoluteDirectory = Path.GetDirectoryName(absolutePath);

            if (!Directory.Exists(absoluteDirectory))
                Directory.CreateDirectory(absoluteDirectory);

            return new FileStream(absolutePath, FileMode.OpenOrCreate);        
        }
        #endregion
    }
}
