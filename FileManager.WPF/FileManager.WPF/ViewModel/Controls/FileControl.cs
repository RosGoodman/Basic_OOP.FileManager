
using NLog;
using System;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    /// <summary> Класс, описывающий контроллер файлов. </summary>
    internal class FileControl : AbstrctFileControl
    {
        private static ILogger _logger;
        private FileInfo _fileInfo;

        /// <summary> Создание экземпляра класса. </summary>
        /// <param name="logger"> Логгер. </param>
        public FileControl(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("Создание экземпляра класса FileControl.");
        }

        /// <summary> Создать файл. </summary>
        /// <param name="path"> Полный путь создаваемого файла. </param>
        public override void Create(string path)
        {
            try
            {
                _fileInfo = new FileInfo(path);
                if (!_fileInfo.Exists)
                    _fileInfo.Create();
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания файла.");
            }
        }

        /// <summary> Копировать указанный файл. </summary>
        /// <param name="fileName"> Имя копируемого файла. </param>
        /// <param name="currentDir"> Полный путь текущей директории. </param>
        /// <param name="newPath"> Полный путь новой директории. </param>
        public override void Copy(string fileName, string currentDir, string newPath)
        {
            try
            {
                _fileInfo = new FileInfo(currentDir);
                if (_fileInfo.Exists)
                    File.Copy(currentDir, newPath + "\\" + fileName, true);
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке копирования");
            }
        }


        /// <summary> Удалить указанный файл. </summary>
        /// <param name="filePath"> Полный путь удаляемого файла. </param>
        public override void Delete(string filePath)
        {
            try
            {
                _fileInfo = new FileInfo(filePath);
                if (_fileInfo.Exists)
                    File.Delete(filePath);
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке удаления файла");
            }
        }

        /// <summary> Переместить / переименовать файл. </summary>
        /// <param name="file"> Текущий полный путь файла. </param>
        /// <param name="newPath"> Новй полный путь файла. </param>
        public override void MoveTo(string file, string newPath)
        {
            try
            {
                _fileInfo = new FileInfo(file);
                if (_fileInfo.Exists && !File.Exists(newPath))
                    _fileInfo.MoveTo(newPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке перемещения файла.");
            }
        }
    }
}
