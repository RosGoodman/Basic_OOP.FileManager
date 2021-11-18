
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager.WPF.Model
{
    public class DirectoryModel : BaseFile
    {
        private DirectoryInfo _fileInfo;

        public DirectoryModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
        }

        public DirectoryModel(string filePath)
            : base (filePath)
        {

        }

        public override BaseFile GetParent(BaseFile file)
        {
            var parent = Directory.GetParent(file.FullPath);
            return base.GetParent(file);
        }

        public override long GetSize()
        {
            return SafeEnumerateFiles(_fullPath, "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length);
        }

        public override string[] GetInfo()
        {
            string[] info = new string[4];
            _fileInfo = new DirectoryInfo(_fullPath);

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
