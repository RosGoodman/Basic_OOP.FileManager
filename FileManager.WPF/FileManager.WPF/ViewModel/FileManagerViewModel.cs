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

        #endregion

        #region props

        #region commands props

        /// <summary> Комангда "создать файл" </summary>
        public RelayCommand CreateCommand { get; private set; }
        /// <summary> Комангда "Открыть." </summary>
        public RelayCommand ListBoxItemEnterCommand { get; private set; }
        /// <summary> Комангда "Перейти к родительской директории." </summary>
        public RelayCommand GoToPreviousDirCommand { get; private set; }
        /// <summary> Комангда "Вырезать." </summary>
        public RelayCommand ListBoxItem_Ctrl_X_Command { get; private set; }
        /// <summary> Комангда "Копировать." </summary>
        public RelayCommand ListBoxItem_Ctrl_C_Command { get; private set; }
        /// <summary> Комангда "Вставтиь." </summary>
        public RelayCommand ListBoxItem_Ctrl_V_Command { get; private set; }
        /// <summary> Комангда "Переименовать." </summary>
        public RelayCommand RenameFile_Command { get; private set; }
        /// <summary> Комангда "Получить подробную информацию о фале." </summary>
        public RelayCommand GetFileInfoCommand { get; private set; }
        /// <summary> Комангда "Найти." </summary>
        public RelayCommand FindCommand { get;private set; }

        #endregion

        #region any props

        /// <summary> Новое имя для переименования файла. </summary>
        public string NewFileName { get; set; }

        /// <summary> Строка поиска. </summary>
        public string SearchLine { get; set; }
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
            DirectoryReaderFromJSON fileReader = new DirectoryReaderFromJSON(_logger);
            CurrentDirectory = fileReader.GetLastDirectory();
            RefreshList();

            //подключение команд
            CreateCommand = new RelayCommand(CreateFileCommand_Execute);
            ListBoxItemEnterCommand = new RelayCommand(OpenDir_Execute);
            GoToPreviousDirCommand = new RelayCommand(GoToPreviousDirectory_Command);
            ListBoxItem_Ctrl_X_Command = new RelayCommand(CutFile_Command);
            ListBoxItem_Ctrl_C_Command = new RelayCommand(CopyFile_Command);
            ListBoxItem_Ctrl_V_Command = new RelayCommand(PastFile_Command);
            RenameFile_Command = new RelayCommand(MoveTo_Command);
            GetFileInfoCommand = new RelayCommand(GetFileInfo_Command);
            FindCommand = new RelayCommand(Find_Command);
        }

        #region commands

        /// <summary> Создать файл. </summary>
        private void CreateFileCommand_Execute()
        {
            _logger.Info($"Зпауск команды {nameof(CreateFileCommand_Execute)}");
            _fileControl.Create("C:\\temp\\tt.txt");
        }

        /// <summary> Открыть текущую выделенную директорию. </summary>
        private void OpenDir_Execute()
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
        private void GoToPreviousDirectory_Command()
        {
            _logger.Info($"Зпауск команды {nameof(GoToPreviousDirectory_Command)}");

            BaseFile parent = CurrentDirectory.GetParent();
            
            CurrentDirectory = (DirectoryModel)parent;
            RefreshList();
        }

        /// <summary> Вырезать файл. </summary>
        private void CutFile_Command()
        {
            RemamberMovableFile();
            _movingFileCut = true;
        }

        /// <summary> Копировать файл. </summary>
        private void CopyFile_Command()
        {
            RemamberMovableFile();
            _movingFileCut = false;
        }

        /// <summary> Вставить файл. </summary>
        private async void PastFile_Command()
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
        private void MoveTo_Command()
        {
            if (SelectedFile.IsDirectory)
                _directoryControl.MoveTo(SelectedFile.FullPath, $"{SelectedFile.GetParent().FullPath}\\{NewFileName}");
            else
                _fileControl.MoveTo(SelectedFile.FullPath, $"{SelectedFile.GetParent().FullPath}\\{NewFileName}");

            RefreshList();
        }

        /// <summary> Получить подробную информацию о фале. </summary>
        private async void GetFileInfo_Command()
        {
            SelectFileInfo = SelectedFile.GetInfo();
            await Task.Run(() =>
            {
                decimal sizeByte = SelectedFile.GetSizeKByte();
                SelectFileSize = ConvertByteSizeToString(sizeByte);
            });
        }

        /// <summary> Найти файл. </summary>
        private async void Find_Command()
        {

        }

        #endregion

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

            string info = $" Создан: {infoArr[2]}, Изменен: {infoArr[3]}{sizeString}";
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
