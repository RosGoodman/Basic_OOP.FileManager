
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManager.WPF.Model
{
    internal class DirectoryModel : IFile
    {
        private string _fullPath;
        private string _name;
        private DirectoryModel _parent;

        public string FullPath
        {
            get => _fullPath;
            private set => _fullPath = value;
        }

        public DirectoryModel(string filePath, string name, DirectoryModel parent)
        {
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
            DirectoryInfo dirInfo = new DirectoryInfo(_fullPath);

            info[0] = dirInfo.Name;
            info[1] = dirInfo.FullName;
            info[2] = dirInfo.CreationTime.ToString();
            info[3] = dirInfo.LastWriteTime.ToString();

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
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDirPath, searchPattern);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                catch (DirectoryNotFoundException)
                {
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
