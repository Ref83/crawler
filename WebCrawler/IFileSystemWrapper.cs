using System.IO;
using System.Threading.Tasks;

namespace WebCrawler
{
    /// <summary>
    /// Обертка файловой системы для сохранения сайта в локальной директории
    /// </summary>
    public interface IFileSystemWrapper
    {
        /// <summary>
        /// Открытие файлового потока для указанного пути
        /// </summary>
        /// <param name="localPath">Путь к локальному файлу</param>
        /// <returns>Поток куда можно сохранить данные</returns>
        Stream OpenStreamFor(string localPath);
    }
}
