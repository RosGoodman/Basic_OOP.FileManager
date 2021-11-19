
using FileManager.WPF.Model;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    internal class DirectoryControl : AbstrctBaseViewModel<DirectoryModel>
    {
        private static ILogger _logger;
        private DirectoryInfo _dirInfo;
        private ObservableCollection<BaseFile> _directories;
        private ObservableCollection<FileModel> _files;

        public ObservableCollection<BaseFile> Directoryes
        {
            get => _directories;
            set
            {
                _directories = value;
            }
        }
        public ObservableCollection<FileModel> Files { get => _files; }

        public DirectoryControl(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("Создание экземпляра класса DirectoryControl.");

            Directoryes = new ObservableCollection<BaseFile>();
        }

        #region methods

        public static string[] GetDirectoryes(string dirName)
        {
            _logger.Debug("Получение списка директорий.");
            return Directory.GetDirectories(dirName);
        }

        public override void Create(string path)
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

        public override void Delete(DirectoryModel dir)
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

        public override void MoveTo(DirectoryModel dir, string newPath)
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

        public override void Copy(DirectoryModel directory, string newDir)
        {
            try
            {
                _dirInfo = new DirectoryInfo(directory.FullPath);
                if (_dirInfo.Exists && !Directory.Exists(newDir + _dirInfo.FullName))
                {
                    var parent = directory.GetParent();
                    Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(parent.FullPath, newDir);
                }
            }
            catch(Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке копирования директории.");
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
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания дочерней директории.");
            }
        }

        #endregion
    }
}
