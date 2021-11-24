
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace FileManager.WPF.Model
{
    /// <summary> Класс, описывающий директриию. </summary>
    public class DirectoryModel : BaseFile , INotifyPropertyChanged
    {
        //не уверен, что так хорошо, но БЫСТРО другого способа отделить обычную директорию от списка с дисками не придумал
        /// <summary> Найменование назначающееся директории с дисками. </summary>
        public const string MainDriveDirectory = "MyComputer";
        private const string ImageIconFilePath = "Images/folder.png";

        private DirectoryInfo _directoryFileInfo;
        private ObservableCollection<BaseFile> _subFiles;

        private static ILogger _logger;

        /// <summary> Список файлов и дерикторий находящихся в текущей директории. </summary>
        public ObservableCollection<BaseFile> SubFiles { get => _subFiles; set => _subFiles = value; }

        /// <summary> Создние экземпляра класса DirectoryModel. </summary>
        /// <param name="logger"> Логгер. </param>
        /// <param name="filePath"> Полный путь директории. </param>
        public DirectoryModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
            _logger = logger;
            LoadMainData(filePath);
            ImagePath = Path.GetFullPath(ImageIconFilePath);
        }

        /// <summary> Загрузить основные данные для текущей директории. </summary>
        /// <param name="filePath"> Полный путь к директории. </param>
        private void LoadMainData(string filePath)
        {
            IsDirectory = true;
            if (filePath != MainDriveDirectory)
            {
                ChangeName(new DirectoryInfo(filePath).Name);
                SetFileInfo();
            }
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
                try 
                {
                    directoryes = Directory.GetDirectories(FullPath);
                    LoadDirectoryesToSubFiles(directoryes);
                }
                catch(UnauthorizedAccessException ex) { _logger.Error($"{ex} - ошибка при попытке достпа к директории."); }
                catch(Exception ex) { _logger.Error($"{ex} - ошибка при попытке достпа к директории."); }

                try 
                {
                    files = Directory.GetFiles(FullPath);
                    LoadFilesToSubFiles(files);
                }
                catch (UnauthorizedAccessException ex) { _logger.Error($"{ex} - ошибка при попытке достпа к файлу."); }
                catch (Exception ex) { _logger.Error($"{ex} - ошибка при попытке достпа к файлу."); }
            }
        }

        /// <summary> Загрузить списко дисков в SubFiles. </summary>
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

        /// <summary> Получить родительскую директорию. </summary>
        /// <returns> Родительская директория. </returns>
        public override BaseFile GetParent()
        {
            if (FullPath == MainDriveDirectory) return this;
            _directoryFileInfo = new DirectoryInfo(FullPath);
            var parent = _directoryFileInfo.Parent;

            if (parent == null) return GetDirectoryMyComputerFromDrives();

            return new DirectoryModel(_logger, parent.FullName);
        }

        /// <summary> Получить размер текущей директории. </summary>
        /// <returns> Размер директории KByte. </returns>
        public override decimal GetSizeKByte()
        {
            decimal byteSize = SafeEnumerateFiles(_fullPath, "*.*", SearchOption.AllDirectories).Sum(n => new FileInfo(n).Length);
            decimal kByteSize = Math.Round(byteSize / 1024);
            return kByteSize;
        }

        /// <summary> Установить информацию о файле. </summary>
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

        /// <summary> Получить информацию о директории. </summary>
        /// <returns> Массив строк с информацией о директории. </returns>
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
        private IEnumerable<string> SafeEnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
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
                    catch(UnauthorizedAccessException ex)
                    {
                        _logger.Error($"{ex} - нет доступа к директории.");
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

        /// <summary> Строчное представление класса. </summary>
        /// <returns> Имя файла. </returns>
        public override string ToString()
        {
            _directoryFileInfo = new DirectoryInfo(FullPath);
            return _directoryFileInfo.Name;
        }

        /// <summary> Добавить список файлов содержащихся в текущей директории. </summary>
        /// <param name="files"> Список файлов. </param>
        public void LoadFilesToSubFiles(string[] files)
        {
            if (_subFiles == null) _subFiles = new ObservableCollection<BaseFile>();
            foreach (string file in files)
            {
                try
                {
                    _subFiles.Add(new FileModel(_logger, file)
                    { 
                        IsDirectory = false, 
                        Name = new FileInfo(file).Name 
                    });
                }
                catch (UnauthorizedAccessException ex) { _logger.Error($"{ex} - ошибка при попытке достпа к файлу."); }
                catch (Exception ex) { _logger.Error($"{ex} - ошибка при попытке достпа к файлу."); }
            }
        }

        /// <summary> Добавить список директорий в SubFiles. </summary>
        /// <param name="directoryes"> Список директорий. </param>
        public void LoadDirectoryesToSubFiles(string[] directoryes)
        {
            if(_subFiles == null) _subFiles= new ObservableCollection<BaseFile>();

            foreach (string dir in directoryes)
            {
                try
                {
                    string name = new DirectoryInfo(dir).Name;
                    if (name == "") name = dir;

                    BaseFile baseFile = new DirectoryModel(_logger, dir)
                    { 
                        IsDirectory = true,
                        Name = name 
                    };

                    _subFiles.Add(baseFile);
                }
                catch (UnauthorizedAccessException ex) { _logger.Error($"{ex} - ошибка при попытке достпа к директории."); }
                catch (Exception ex) { _logger.Error($"{ex} - ошибка при попытке достпа к директории."); }
            }
        }

        /// <summary> Получение директории "MyComputer" со списком дисков в виде обычной директории. </summary>
        /// <returns> Директория "MyComputer". </returns>
        private BaseFile GetDirectoryMyComputerFromDrives()
        {
            DirectoryModel directory = new DirectoryModel(_logger, MainDriveDirectory)
            {
                FullPath = MainDriveDirectory,
                Name = MainDriveDirectory
            };
            directory.LoadDrivesInSubFiles();
            return directory;
        }
    }
}
