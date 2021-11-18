
using NLog;
using System.IO;

namespace FileManager.WPF.Model
{
    internal class FileModel : IFile
    {
        private static ILogger _logger;
        private FileInfo _fileInfo;

        private string _fullPath;
        private string _name;
        private DirectoryModel _parent;

        public string FullPath
        {
            get => _fullPath;
            private set => _fullPath = value;
        }

        public FileModel(ILogger logger, string filePath, string name, DirectoryModel parent)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра объекта FileModel.");

            _fullPath = filePath;
            _name = name;
            _parent = parent;
        }

        public string[] GetInfo()
        {
            _fileInfo = new FileInfo(_fullPath);
            string[] info = new string[4];
            if (_fileInfo.Exists)
            {
                info[0] = _fileInfo.Name;
                info[1] = _fileInfo.FullName;
                info[2] = _fileInfo.CreationTime.ToString();
                info[3] = _fileInfo.LastWriteTime.ToString();
            }
            else { _logger.Error($"{_fileInfo.Exists} - файл не найден при попытке получения информации о нем."); }

            return info;
        }

        public long GetSize()
        {
            _fileInfo = new FileInfo(_fullPath);
            if (_fileInfo.Exists)
            {
                return _fileInfo.Length;
            }
            else { _logger.Error($"{_fileInfo.Exists} - файл не найден при попытке получения рего размера."); }

            return 0;
        }
    }
}
