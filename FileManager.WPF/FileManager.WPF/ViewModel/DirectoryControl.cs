
using FileManager.WPF.Model;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    internal class DirectoryControl : BaseViewModel
    {
        private static ILogger _logger;
        private DirectoryInfo _dirInfo;
        private ObservableCollection<DirectoryModel> _directories;

        public ObservableCollection<DirectoryModel> Directoryes { get => _directories; }

        public DirectoryControl(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("Создание экземпляра класса DirectoryControl.");
        }

        public static string[] GetDirectoryes(string dirName)
        {
            _logger.Debug("Получение списка директорий.");
            return Directory.GetDirectories(dirName);
        }

        public void Create(string path)
        {
            try
            {
                _dirInfo = new DirectoryInfo(path);
                if (_dirInfo.Exists)
                    _dirInfo.Create();
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания директории.");
            }
        }

        public void CreateSubDirectory(string subPath)
        {
            try
            {
                _dirInfo = new DirectoryInfo(subPath);
                if (_dirInfo.Exists)
                    _dirInfo.CreateSubdirectory(subPath);
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания дочерней директории.");
            }
        }

        public void Delete(DirectoryModel dir)
        {
            try
            {
                Directory.Delete(dir.FullPath, true);
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
                _dirInfo = new DirectoryInfo(dir.FullPath);
                if (_dirInfo.Exists && !Directory.Exists(newPath))
                    _dirInfo.MoveTo(newPath);
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке перемещения директории.");
            }
        }
    }
}
