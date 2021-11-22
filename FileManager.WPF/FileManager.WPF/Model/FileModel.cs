
using NLog;
using System.IO;

namespace FileManager.WPF.Model
{
    /// <summary> Класс, описывающий файл. </summary>
    internal class FileModel : BaseFile
    {
        private const string ImageIconFilePath = "Images/file.png";
        private FileInfo _fileInfo;

        /// <summary> Создание экземпляра класса FileModel. </summary>
        /// <param name="logger"> Логгер. </param>
        /// <param name="filePath"> Полный путь к файлу. </param>
        public FileModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
            IsDirectory = false;
            SetFileInfo();
            ImagePath = Path.GetFullPath(ImageIconFilePath);
        }

        /// <summary> Получить список информацию о файле. </summary>
        /// <returns> Массив с информацией. </returns>
        public override string[] GetInfo() => FileInfo;

        /// <summary> Получить размер файла. </summary>
        /// <returns> Получить размер файла KByte. </returns>
        public override decimal GetSizeKByte()
        {
            _fileInfo = new FileInfo(_fullPath);
            if (_fileInfo.Exists)
            {
                return _fileInfo.Length / 1024;
            }
            else { _logger.Error($"{_fileInfo.Exists} - файл не найден при попытке получения рего размера."); }

            return 0;
        }

        /// <summary> Получить родительскую директорию. </summary>
        /// <returns> Родительская директория. </returns>
        public override BaseFile GetParent()
        {
            _fileInfo = new FileInfo(FullPath);
            var parent = _fileInfo.Directory;
            return new BaseFile(parent.FullName);
        }

        /// <summary> Строчное представление файла класса. </summary>
        /// <returns> Имя файла. </returns>
        public override string ToString()
        {
            _fileInfo = new FileInfo(FullPath);
            return _fileInfo.Name;
        }

        /// <summary> Установить информацию о файле. </summary>
        private void SetFileInfo()
        {
            _fileInfo = new FileInfo(_fullPath);
            string[] info = new string[5];
            if (_fileInfo.Exists)
            {
                info[0] = _fileInfo.Name;
                info[1] = _fileInfo.FullName;
                info[2] = _fileInfo.CreationTime.ToString();
                info[3] = _fileInfo.LastWriteTime.ToString();
                if (!this.IsDirectory) info[4] = "Файл";

            }
            else { _logger.Error($"{_fileInfo.Exists} - файл не найден при попытке получения информации о нем."); }

            FileInfo = info;
        }
    }
}
