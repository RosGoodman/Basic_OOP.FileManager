using FileManager.WPF.Command;
using FileManager.WPF.Model;
using FileManager.WPF.Services.WorkWithFiles;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace FileManager.WPF.ViewModel
{
    /// <summary> Класс описывающий ViewModel. </summary>
    public class FileManagerViewModel : BaseViewModel
    {
        #region feilds

        private readonly ILogger _logger;

        private DirectoryControl _directoryControl;
        private FileControl _fileControl;

        private DirectoryModel _currentDirectory;
        private BaseFile _selectedFile;
        private string _currentPath;
        private string _fileInfo;
        private string _selectFileSize;

        private BaseFile _movableFile;
        private bool _movingFileCut;    //вырезан / скопирован
        private string _searchLine;

        #endregion
        /////////////////////////////////////////////////////////////////////
        #region props
        /////////////////////////////////////////////////////////////////////
        #region commands props

        /// <summary> Команда "Создать файл". </summary>
        public RelayCommand CreateFileCommand { get; private set; }
        /// <summary> Команда "Создать директорию". </summary>
        public RelayCommand CreateDirectoryCommand { get; private set; }
        /// <summary> Команда "Открыть". </summary>
        public RelayCommand ListBoxItemEnterCommand { get; private set; }
        /// <summary> Команда "Перейти к родительской директории". </summary>
        public RelayCommand GoToPreviousDirCommand { get; private set; }
        /// <summary> Команда "Вырезать". </summary>
        public RelayCommand ListBoxItem_Ctrl_X_Command { get; private set; }
        /// <summary> Команда "Копировать". </summary>
        public RelayCommand ListBoxItem_Ctrl_C_Command { get; private set; }
        /// <summary> Команда "Вставтиь". </summary>
        public RelayCommand ListBoxItem_Ctrl_V_Command { get; private set; }
        /// <summary> Команда "Переименовать". </summary>
        public RelayCommand RenameFile_Command { get; private set; }
        /// <summary> Команда "Получить подробную информацию о фале". </summary>
        public RelayCommand GetFileInfoCommand { get; private set; }
        /// <summary> Команда "Найти". </summary>
        public RelayCommand FindCommand { get;private set; }
        /// <summary> Команда "удалить выбранный файл". </summary>
        public RelayCommand DeleteCommand { get;private set; }
        /// <summary> Команда "Сохранить текущий путь директории." </summary>
        public RelayCommand SavePathAndCloseAppCommand { get; private set; }

        #endregion
        /////////////////////////////////////////////////////////////////////
        #region any props

        /// <summary> Новое имя для переименования файла. </summary>
        public string NewFileName { get; set; }

        /// <summary> Строка поиска. </summary>
        public string SearchLine
        {
            get => _searchLine;
            set
            {
                _searchLine = value;
                //OnPropertyChanged("SerchLine");
            }
        }
        public string InfoImagePath { get => Path.GetFullPath("Images/question.png"); }
        /// <summary> Путь к изображению кнопки "назад". </summary>
        public string BackImagePath { get => Path.GetFullPath("Images/previous.png"); }
        /// <summary> подробная информация о текущем выбранном файле. </summary>
        public string[] SelectFileInfo { get; private set; }
        /// <summary> Размер выбранного файла. </summary>
        public string SelectFileSize
        {
            get => _selectFileSize;
            private set
            {
                _selectFileSize = value;
                OnPropertyChanged("SelectFileSize");
            }
        }

        /// <summary> Строка краткой информащии о файле(внизу окна). </summary>
        public string FileInfo
        {
            get => _fileInfo;
            set
            {
                _fileInfo = value;
                OnPropertyChanged("FileInfo");
            }
        }

        /// <summary> Текущий выделенный файл. </summary>
        public BaseFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                CurrentPath = _currentDirectory.FullPath;
                if (SelectedFile != null && SelectedFile.FileInfo != null) FileInfo = GetFileInfo();

                OnPropertyChanged("SelectedFile");
            }
        }

        public string CreationTime { get; private set; }
        public string LastWriteTime { get; private set; }

        /// <summary> Коллекция файлов текущей открытой директории. </summary>
        public ObservableCollection<BaseFile> AllFilesInCurrentDir
        {
            get => _directoryControl.AllFilesInDirectoiy;
            set
            {
                _directoryControl.AllFilesInDirectoiy = value;
                OnPropertyChanged("AllFilesInCurrentDir");
            }
        }

        /// <summary> Строка навигации. Полный путь текущей директории. </summary>
        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
            }
        }

        /// <summary> Экземпляр текущей директории. </summary>
        public DirectoryModel CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;
                OnPropertyChanged(nameof(CurrentDirectory));
            }
        }

        #endregion

        #endregion
        /////////////////////////////////////////////////////////////////////
        /// <summary> Конструктор FileManagerViewModel. </summary>
        /// <param name="logger"> Логгер. </param>
        public FileManagerViewModel(ILogger logger)
        {
            //логгер
            _logger = logger;
            _logger.Info("Создание экземпляра класса FileManagerViewModel.");

            //создание экземпляров контроллеров
            _directoryControl = new DirectoryControl(_logger);
            _fileControl = new FileControl(_logger);

            //чтение последнего файла
            CurrentDirectory = new DirectoryModel(_logger, DirectoryReaderFromTXT.ReadDirectoryPath(_logger));
            RefreshList();

            //подключение команд
            CreateFileCommand = new RelayCommand(CreateFileCommand_Execute);
            CreateDirectoryCommand = new RelayCommand(CreateDirectoryCommand_Execute);
            ListBoxItemEnterCommand = new RelayCommand(OpenDir_Execute);
            GoToPreviousDirCommand = new RelayCommand(GoToPreviousDirectory_Command);
            ListBoxItem_Ctrl_X_Command = new RelayCommand(CutFile_Command);
            ListBoxItem_Ctrl_C_Command = new RelayCommand(CopyFile_Command);
            ListBoxItem_Ctrl_V_Command = new RelayCommand(PastFile_Command);
            RenameFile_Command = new RelayCommand(MoveTo_Command);
            GetFileInfoCommand = new RelayCommand(GetFileInfo_Command);
            FindCommand = new RelayCommand(Find_Command);
            DeleteCommand = new RelayCommand(Delete_Command);
            SavePathAndCloseAppCommand = new RelayCommand(SavePathAndCloseApp_Command);
        }

        ///////////////////////////////////////////////////////////////////////
        #region commands

        /// <summary> Создать файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void CreateFileCommand_Execute(object param)
        {
            _logger.Info($"Зпауск команды {nameof(CreateFileCommand_Execute)}");
            _fileControl.Create($"{CurrentDirectory.FullPath}\\{NewFileName}");

            RefreshList();
        }

        /// <summary> Создать директорию. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void CreateDirectoryCommand_Execute(object param)
        {
            _logger.Info($"Зпауск команды {nameof(CreateDirectoryCommand_Execute)}");
            string delimeter = "";

            if (CurrentDirectory.FullPath[CurrentDirectory.FullPath.Length - 1] != '\\')
                delimeter = "\\";

            _directoryControl.Create($@"{CurrentDirectory.FullPath}{delimeter}{NewFileName}");

            RefreshList();
        }

        /// <summary> Открыть текущую выделенную директорию. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void OpenDir_Execute(object param)
        {
            _logger.Info($"Зпауск команды {nameof(OpenDir_Execute)}");

            if (SelectedFile != null && SelectedFile.IsDirectory)
            {
                CurrentDirectory = (DirectoryModel)SelectedFile;

                CurrentDirectory.LoadSubDirectoryes();
                AllFilesInCurrentDir = CurrentDirectory.SubFiles;

                if (AllFilesInCurrentDir.Count > 0)
                    SelectedFile = AllFilesInCurrentDir[0];
            }
        }

        /// <summary> Перейти к родительской директории. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void GoToPreviousDirectory_Command(object param)
        {
            _logger.Info($"Зпауск команды {nameof(GoToPreviousDirectory_Command)}");

            BaseFile parent = CurrentDirectory.GetParent();
            
            CurrentDirectory = (DirectoryModel)parent;
            RefreshList();
        }

        /// <summary> Вырезать файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void CutFile_Command(object param)
        {
            RemamberMovableFile();
            _movingFileCut = true;
        }

        /// <summary> Копировать файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void CopyFile_Command(object param)
        {
            RemamberMovableFile();
            _movingFileCut = false;
        }

        /// <summary> Вставить файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private async void PastFile_Command(object param)
        {
            await Task.Run(() =>
            {
                if (CurrentDirectory.Name == DirectoryModel.MainDriveDirectory) return;

                if (_movableFile.IsDirectory)
                {
                    Copy(_directoryControl);
                }
                else
                {
                    Copy(_fileControl);
                }

                RefreshList();
                _movingFileCut = false;
            });
        }

        /// <summary> Переименовать / переместить файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void MoveTo_Command(object param)
        {
            if (SelectedFile.IsDirectory)
                _directoryControl.MoveTo(SelectedFile.FullPath, $"{SelectedFile.GetParent().FullPath}\\{NewFileName}");
            else
                _fileControl.MoveTo(SelectedFile.FullPath, $"{SelectedFile.GetParent().FullPath}\\{NewFileName}");

            RefreshList();
        }

        /// <summary> Получить подробную информацию о фале. </summary>
        /// <param name="param"> Параметр команды. </param>
        private async void GetFileInfo_Command(object param)
        {
            SelectFileInfo = SelectedFile.GetInfo();
            await Task.Run(() =>
            {
                string[] info = SelectedFile.GetInfo();
                CreationTime = info[2];
                LastWriteTime = info[3];

                decimal sizeByte = SelectedFile.GetSizeKByte();
                SelectFileSize = ConvertByteSizeToString(sizeByte);
            });
        }

        /// <summary> Найти файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private async void Find_Command(object param)
        {
            await Task.Run(() => 
            {
                CurrentDirectory = _directoryControl.FileSearchByMask(CurrentDirectory.FullPath, SearchLine);

                AllFilesInCurrentDir = CurrentDirectory.SubFiles;
                if(AllFilesInCurrentDir.Count > 0) SelectedFile = AllFilesInCurrentDir[0];
            });
        }

        /// <summary> Удалить выбранный файл. </summary>
        /// <param name="param"> Параметр команды. </param>
        private async void Delete_Command(object param)
        {
            await Task.Run(() =>
            {
                if (SelectedFile.IsDirectory)
                    _directoryControl.Delete(SelectedFile.FullPath);
                else
                    _fileControl?.Delete(SelectedFile.FullPath);
            });
            
            RefreshList();
        }

        /// <summary> Записать текущую директорию в файл JSON. </summary>
        /// <param name="param"> Параметр команды. </param>
        private void SavePathAndCloseApp_Command(object param)
        {
            DirectoryWriterToTXT.WriteLastPath(_logger, CurrentDirectory.FullPath);
        }

        #endregion
        ///////////////////////////////////////////////////////////////////////
        #region methods

        /// <summary> Копировать файл. </summary>
        /// <param name="fileControl"> Контроллер файлов. </param>
        private void Copy(IFileControl fileControl)
        {
            fileControl.Copy(_movableFile.Name, _movableFile.FullPath, CurrentDirectory.FullPath);
            if (_movingFileCut) 
                fileControl.Delete(_movableFile.FullPath);
        }

        /// <summary> Обновить текущую открытую директорию. </summary>
        private void RefreshList()
        {
            //если директория с дисками, то переход в метод с driveInfo
            if(CurrentDirectory.Name == DirectoryModel.MainDriveDirectory)
                CurrentDirectory.LoadDrivesInSubFiles();
            else
                CurrentDirectory.LoadSubDirectoryes();

            AllFilesInCurrentDir = CurrentDirectory.SubFiles;
            if(AllFilesInCurrentDir.Count > 0)
                SelectedFile = AllFilesInCurrentDir[0];
        }

        /// <summary> Запомнить купируемый/вырезанный файл. </summary>
        private void RemamberMovableFile()
        {
            if (SelectedFile == null) return;
            _movableFile = SelectedFile;
        }

        /// <summary> Получить строку краткой информации о фале. </summary>
        /// <returns> Строка с краткой информацей. </returns>
        private string GetFileInfo()
        {
            string[] infoArr = SelectedFile.FileInfo;
            if (infoArr == null) return null;

            decimal size = 0;
            if (!SelectedFile.IsDirectory) size = SelectedFile.GetSizeKByte();
            string sizeString = ConvertByteSizeToString(size);

            string info = $" Создан: {infoArr[2]}, Изменен: {infoArr[3]}{sizeString} ";
            return info;
        }

        /// <summary> Конвертировать размер файла (KByte) в моксимально возможную величину
        /// и вернуть в строчном виде с указанием размерности. </summary>
        /// <param name="sizeInByte"> Размер в KByte. </param>
        /// <returns> Строка с указанием величины и размерности (KByte/MB/GB/TB) </returns>
        private string ConvertByteSizeToString(decimal sizeInByte)
        {
            string sizeString = string.Empty;
            string postfix = "Byte";

            for (int i = 0; i < 5; i++)
            {
                if (sizeInByte > 1024)
                {
                    sizeInByte /= 1024;
                    if (Enum.IsDefined(typeof(SystemOfUnits), i + 2))
                        postfix = ((SystemOfUnits)i + 2).ToString();
                }
                else break;
            }

            if (sizeInByte > 0)
            {
                sizeInByte = Math.Round(sizeInByte, 2);
                sizeString = $"Размер: {sizeInByte} {postfix}";
            }
            return sizeString;
        }

        #endregion
    }
}
