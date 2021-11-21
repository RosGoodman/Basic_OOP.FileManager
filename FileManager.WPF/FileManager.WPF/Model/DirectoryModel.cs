
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
        //не уверен, что так хорошо, но быстро другого способа отделить обычную директорию от списка с дисками не придумал
        /// <summary> Найменование назначающееся директории с дисками. </summary>
        public const string MainDriveDirectory = "MyComputer";

        private DirectoryInfo _directoryFileInfo;
        private ObservableCollection<BaseFile> _subFiles;

        private static ILogger _logger;

        /// <summary> Список файлов и дерикторий находящихся в текущей директории. </summary>
        public ObservableCollection<BaseFile> SubFiles { get => _subFiles; set => _subFiles = value; }

        #region ctors

        /// <summary> Создние экземпляра класса DirectoryModel. </summary>
        /// <param name="logger"> Логгер. </param>
        /// <param name="filePath"> Полный путь директории. </param>
        public DirectoryModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
            logger = _logger;
            LoadMainData(filePath);
        }

        /// <summary> Создние экземпляра класса DirectoryModel. </summary>
        /// <param name="filePath"> Полный путь директории. </param>
        public DirectoryModel(string filePath)
            : base(filePath)
        {
            LoadMainData(filePath);
        }

        #endregion

        private void LoadMainData(string filePath)
        {
            IsDirectory = true;
            if (filePath != MainDriveDirectory)
            {
                ChangeName(new DirectoryInfo(filePath).Name);
                SetFileInfo();
            }

            ImagePath = Path.GetFullPath("Images/folder.png");
        }

        /// <summary> Загрузить файлы и дериктории в коллекцию SubFiles. </summary>
        public void LoadSubDirectoryes()
        {
            string[] files;
            _subFiles = new ObservableCollection<BaseFile>();

            DriveInfo[] drives = DriveInfo.GetDrives();
            string[] directoryes = new string[drives.Length];

            if(FullPath != MainDriveDirectory)
            {
                directoryes = Directory.GetDirectories(FullPath);
                LoadDirectoryesToSubFiles(directoryes);

                files = Directory.GetFiles(FullPath);
                LoadFilesToSubFiles(files);
            }
        }

        public void LoadDrivesInSubFiles()
        {
            string[] directoryes;
            _subFiles = new ObservableCollection<BaseFile>();

            if (FullPath == MainDriveDirectory)
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                directoryes = new string[drives.Length];

                for (int i = 0; i < drives.Length; i++)
                {
                    directoryes[i] = drives[i].Name;
                }
                LoadDirectoryesToSubFiles(directoryes);
            }
        }

        public override BaseFile GetParent()
        {
            if (FullPath == MainDriveDirectory) return this;
            _directoryFileInfo = new DirectoryInfo(FullPath);
            var parent = _directoryFileInfo.Parent;

            if (parent == null) return GetDirectoryMyComputerFromDrives();

            return new DirectoryModel(parent.FullName);
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
                    catch(Exception ex)
                    {
                        //_logger.Error($"{ex} -нет доступа к директории.");
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
                try
                {
                    _subFiles.Add(new FileModel(file)
                    { 
                        IsDirectory = false, 
                        Name = new FileInfo(file).Name 
                    });
                }
                catch { }
            }
        }

        public void LoadDirectoryesToSubFiles(string[] directoryes)
        {
            foreach (string dir in directoryes)
            {
                try
                {
                    string name = new DirectoryInfo(dir).Name;
                    if (name == "") name = dir;

                    BaseFile baseFile = new DirectoryModel(dir)
                    { 
                        IsDirectory = true,
                        Name = name 
                    };

                    _subFiles.Add(baseFile);
                }
                catch { }
            }
        }

        private BaseFile GetDirectoryMyComputerFromDrives()
        {
            DirectoryModel directory = new DirectoryModel(MainDriveDirectory)
            {
                FullPath = MainDriveDirectory,
                Name = MainDriveDirectory
            };
            directory.LoadDrivesInSubFiles();
            return directory;
        }
    }
}
