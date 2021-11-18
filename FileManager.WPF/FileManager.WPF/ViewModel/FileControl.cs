
using FileManager.WPF.Model;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    internal class FileControlprivate : BaseViewModel
    {
        static ILogger _logger;
        private FileInfo _fileInfo;
        private ObservableCollection<FileModel> _files;

        public ObservableCollection<FileModel> Directoryes { get => _files; }

        public FileControl(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("Создание экземпляра класса FileControl.");
        }

        public void Create(string path)
        {
            try
            {
                _fileInfo = new FileInfo(path);
                if (_fileInfo.Exists)
                    _fileInfo.Create();
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания директории.");
            }
        }

        public void Delete(DirectoryModel dir)
        {
            try
            {
                File.Delete(dir.FullPath, true);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке удаления директории.");
            }
        }

        public void MoveTo(DirectoryModel dir, string newPath)
        {
            try
            {
                _fileInfo = new FileInfo(dir.FullPath);
                if (_fileInfo.Exists && !File.Exists(newPath))
                    _fileInfo.MoveTo(newPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке перемещения директории.");
            }
        }
    }
}
