
using NLog;
using System.IO;

namespace FileManager.WPF.Model
{
    internal class FileModel : BaseFile
    {
        private FileInfo _fileInfo;
        public FileModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
            IsDirectory = false;
        }

        public override string[] GetInfo()
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

        public override long GetSize()
        {
            _fileInfo = new FileInfo(_fullPath);
            if (_fileInfo.Exists)
            {
                return _fileInfo.Length;
            }
            else { _logger.Error($"{_fileInfo.Exists} - файл не найден при попытке получения рего размера."); }

            return 0;
        }

        public override BaseFile GetParent()
        {
            _fileInfo = new FileInfo(FullPath);
            var parent = _fileInfo.Directory;
            return new BaseFile(parent.FullName);
        }

        public override string ToString()
        {
            _fileInfo = new FileInfo(FullPath);
            return _fileInfo.Name;
        }
    }
}
