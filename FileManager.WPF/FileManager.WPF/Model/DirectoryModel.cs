
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace FileManager.WPF.Model
{
    public class DirectoryModel : BaseFile
    {
        private DirectoryInfo _directoryFileInfo;
        private ObservableCollection<BaseFile> _subFiles;

        public ObservableCollection<BaseFile> SubFiles
        {
            get => _subFiles;
            set => _subFiles = value;
        }

        public DirectoryModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
            LoadMainData(filePath);
        }

        public DirectoryModel(string filePath)
            : base(filePath)
        {
            LoadMainData(filePath);
        }

        private void LoadMainData(string filePath)
        {
            IsDirectory = true;
            if (filePath != "MyComputer")
            {
                ChangeName(new DirectoryInfo(filePath).Name);
                SetFileInfo();
            }

            ImagePath = Path.GetFullPath("Images/folder.png");
        }

        public void LoadSubDirectoryes()
        {
            string[] directoryes;
            string[] files;
            _subFiles = new ObservableCollection<BaseFile>();

            if (FullPath == "MyComputer")
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                directoryes = new string[drives.Length];

                for(int i = 0; i < drives.Length; i++)
                {
                    directoryes[i] = drives[i].Name;
                }
                LoadDirectoryesToSubFiles(directoryes);
            }

            try
            {
                directoryes = Directory.GetDirectories(FullPath);
                LoadDirectoryesToSubFiles(directoryes);
            }
            catch { }
            try
            {
                files = Directory.GetFiles(FullPath);
                LoadFilesToSubFiles(files);
            }
            catch { }
        }

        public override BaseFile GetParent()
        {
            if (FullPath == "MyComputer") return this;
            _directoryFileInfo = new DirectoryInfo(FullPath);
            var parent = _directoryFileInfo.Parent;

            if (parent == null) return GetDriverParent();

            return (BaseFile)new DirectoryModel(parent.FullName);
        }

        public override decimal GetSize()
        {
            decimal byteSize = SafeEnumerateFiles(_fullPath, "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length);
            decimal kByteSize = Math.Round(byteSize / 1024);
            return kByteSize;
        }

        private void SetFileInfo()
        {
            string[] info = new string[5];
            _directoryFileInfo = new DirectoryInfo(_fullPath);

            if (_directoryFileInfo.Exists)
            {
                info[0] = _directoryFileInfo.Name;
                info[1] = _directoryFileInfo.FullName;
                info[2] = _directoryFileInfo.CreationTime.ToString();
                info[3] = _directoryFileInfo.LastWriteTime.ToString();
                if (this.IsDirectory) info[4] = "Папка с файлами";
            }
            else { _logger.Error($"{_directoryFileInfo.Exists} - файл не найден при попытке получения информации о нем."); }

            FileInfo = info;
        }

        public override string[] GetInfo()
        {
            return FileInfo;
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

        public override string ToString()
        {
            _directoryFileInfo = new DirectoryInfo(FullPath);
            return _directoryFileInfo.Name;
        }

        public void SetDirectoryes(ObservableCollection<BaseFile> dirs)
        {
            if (_subFiles != null)
                return;   //на всякий случай (метод нужен только для drives)
            else
                _subFiles = new ObservableCollection<BaseFile>();

            foreach (var dir in dirs)
            {
                _subFiles.Add(dir);
            }
        }

        public void LoadFilesToSubFiles(string[] files)
        {
            foreach (string file in files)
            {
                _subFiles.Add((BaseFile)new FileModel(file) { IsDirectory = false, Name = new FileInfo(file).Name });
            }
        }

        public void LoadDirectoryesToSubFiles(string[] directoryes)
        {
            foreach (string dir in directoryes)
            {
                string name = new DirectoryInfo(dir).Name;
                if (name == "") name = dir;

                BaseFile baseFile = (BaseFile)new DirectoryModel(dir) { IsDirectory = true, Name = name };
                _subFiles.Add(baseFile);
            }
        }

        private BaseFile GetDriverParent()
        {
            DirectoryModel directory = new DirectoryModel("MyComputer")
            {
                FullPath = "MyComputer",
                Name = "MyComputer"
            };
            directory.LoadSubDirectoryes();
            return directory;
        }
    }
}
