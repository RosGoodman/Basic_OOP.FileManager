
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager.WPF.Model
{
    internal class DirectoryModel : IFile
    {
        private static ILogger _logger;
        private DirectoryInfo _dirInfo;

        private string _fullPath;
        private string _name;
        private DirectoryModel _parent;

        public string FullPath
        {
            get => _fullPath;
            private set => _fullPath = value;
        }

        public DirectoryModel(ILogger logger, string filePath, string name, DirectoryModel parent)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра DirectoryModel.");

            _fullPath = filePath;
            _name = name;
            _parent = parent;
        }

        public long GetSize()
        {
            return SafeEnumerateFiles(_fullPath, "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length);
        }

        public string[] GetInfo()
        {
            string[] info = new string[4];
            _dirInfo = new DirectoryInfo(_fullPath);

            if (_dirInfo.Exists)
            {
                info[0] = _dirInfo.Name;
                info[1] = _dirInfo.FullName;
                info[2] = _dirInfo.CreationTime.ToString();
                info[3] = _dirInfo.LastWriteTime.ToString();
            }
            else { _logger.Error($"{_dirInfo.Exists} - файл не найден при попытке получения информации о нем."); }

            return info;
        }

        /// <summary>
        /// Возвращает перечисляемую коллекцию имен файлов которые соответствуют шаблону в указанном каталоге, с дополнительным просмотром вложенных каталогов
        /// </summary>
        /// <param name="path">Полный или относительный путь катага в котором выполняется поиск</param>
        /// <param name="searchPattern">Шаблон поиска файлов</param>
        /// <param name="searchOption">Одно из значений перечисления SearchOption указывающее нужно ли выполнять поиск во вложенных каталогах или только в указанном каталоге</param>
        /// <returns>Возвращает перечисляемую коллекцию полных имен файлов</returns>
        private static IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var dirs = new Stack<string>();
            dirs.Push(path);

            while (dirs.Count > 0)
            {
                string currentDirPath = dirs.Pop();
                if (searchOption == SearchOption.AllDirectories)
                {
                    try
                    {
                        string[] subDirs = Directory.GetDirectories(currentDirPath);
                        foreach (string subDirPath in subDirs)
                        {
                            dirs.Push(subDirPath);
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        _logger.Error($"{ex} - нет доступа к директории.");
                        continue;
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        _logger.Error($"{ex} - директория не найдена.");
                        continue;
                    }
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDirPath, searchPattern);
                }
                catch (UnauthorizedAccessException ex)
                {
                    _logger.Error($"{ex} - нет доступа к файлу.");
                    continue;
                }
                catch (DirectoryNotFoundException ex)
                {
                    _logger.Error($"{ex} - файл не найден.");
                    continue;
                }

                foreach (string filePath in files)
                {
                    yield return filePath;
                }
            }
        }
    }
}
