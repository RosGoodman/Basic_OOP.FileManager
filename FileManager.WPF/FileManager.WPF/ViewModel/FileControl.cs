
using FileManager.WPF.Model;
using NLog;
using System;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    internal class FileControl : AbstrctBaseViewModel<FileModel>
    {
        private static ILogger _logger;
        private FileInfo _fileInfo;

        public FileControl(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("Создание экземпляра класса FileControl.");
        }

        public override void Create(string path)
        {
            try
            {
                _fileInfo = new FileInfo(path);
                if (_fileInfo.Exists)
                    _fileInfo.Create();
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания файла.");
            }
        }

        public override void Copy(FileModel file, string newPath)
        {
            try
            {
                _fileInfo = new FileInfo(file.FullPath);
                if (_fileInfo.Exists)
                    _fileInfo.CopyTo(newPath, true);
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке копирования");
            }
        }

        public override void Delete(FileModel file)
        {
            try
            {
                _fileInfo = new FileInfo(file.FullPath);
                if (_fileInfo.Exists)
                    File.Delete(file.FullPath);
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке удаления файла");
            }
        }

        public override void MoveTo(FileModel file, string newPath)
        {
            try
            {
                _fileInfo = new FileInfo(file.FullPath);
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
