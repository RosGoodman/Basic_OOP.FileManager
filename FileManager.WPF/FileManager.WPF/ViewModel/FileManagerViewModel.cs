using FileManager.WPF.Command;
using FileManager.WPF.Model;
using FileManager.WPF.Services.WorkWithFiles;
using FileManager.WPF.ViewModel.Controls;
using NLog;
using System;
using System.Collections.ObjectModel;

namespace FileManager.WPF.ViewModel
{
    public class FileManagerViewModel : BaseViewModel
    {
        #region feilds

        private readonly ILogger _logger;

        private DirectoryControl _directoryControl;
        private FileControl _fileControl;
        private DriveControl _driveControl;

        private DirectoryModel _currentDirectory;
        private BaseFile _selectedFile;
        private string _currentPath;
        private string _fileInfo;

        #endregion

        #region props

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand ListBoxItemEnterCommand { get; private set; }
        public RelayCommand GoToPreviousDirCommand { get; private set; }
        public RelayCommand ListBoxItemBackspaceCommand { get; private set; }

        public string FileInfo
        {
            get => _fileInfo;
            set
            {
                _fileInfo = value;
                OnPropertyChanged("FileInfo");
            }
        }

        public BaseFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
                CurrentPath = _currentDirectory.FullPath;
                if (SelectedFile != null) FileInfo = GetFileInfo();
            }
        }

        public ObservableCollection<BaseFile> Directoryes
        {
            get => _directoryControl.Directoryes;
            set
            {
                _directoryControl.Directoryes = value;
                OnPropertyChanged("Directoryes");
            }
        }

        public string CurrentPath
        {
            get => _currentPath;
            set
            {
                _currentPath = value;
                OnPropertyChanged(nameof(CurrentPath));
            }
        }

        public DirectoryModel CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;

                Directoryes = new ObservableCollection<BaseFile>();
                //добавление директорий в список
                if(CurrentDirectory.Directoryes != null)
                {
                    foreach (string dir in CurrentDirectory.Directoryes)
                    {
                        Directoryes.Add(new DirectoryModel(dir));
                    }
                }
                
                //добавление файлов в список
                if(CurrentDirectory.Files != null)
                {
                    foreach (string file in CurrentDirectory.Files)
                    {
                        Directoryes.Add(new FileModel(_logger, file));
                    }
                }
                OnPropertyChanged(nameof(CurrentDirectory));
            }
        }

        #endregion

        /// <summary> CTOR </summary>
        /// <param name="logger"></param>
        public FileManagerViewModel(ILogger logger)
        {
            //логгер
            _logger = logger;
            _logger.Info("Создание экземпляра класса FileManagerViewModel.");

            //создание экземпляров контроллеров
            _directoryControl = new DirectoryControl(_logger);
            _fileControl = new FileControl(_logger);
            _driveControl = new DriveControl(_logger);

            //чтение последнего файла
            JSONFileReader fileReader = new JSONFileReader(_logger);
            CurrentDirectory = fileReader.GetLastDirectory();
            SelectedFile = Directoryes[0];

            //подключение команд
            CreateCommand = new RelayCommand(CreateFileCommand_Execute);
            ListBoxItemEnterCommand = new RelayCommand(OpenDir_Execute);
            GoToPreviousDirCommand = new RelayCommand(GoToPreviousDirectory_Command);
            ListBoxItemBackspaceCommand = new RelayCommand(GoToPreviousDirectory_Command);
        }

        #region commands

        public void CreateFileCommand_Execute()
        {
            _fileControl.Create("C:\\temp\\tt.txt");
        }

        public void OpenDir_Execute()
        {
            if (SelectedFile.IsDirectory)
            {
                CurrentDirectory = (DirectoryModel)SelectedFile;
                if(Directoryes.Count>0)
                    SelectedFile = Directoryes[0];
            }
                
            //else
                // try open
        }

        public void GoToPreviousDirectory_Command()
        {
            BaseFile parent = CurrentDirectory.GetParent();

            var prevDir = CurrentDirectory.FullPath;
            
            if (Directoryes.Count > 0)
            {
                CurrentDirectory = (DirectoryModel)CurrentDirectory.GetParent();
                SelectedFile = Directoryes[0];
            }
            
            if(CurrentDirectory.FullPath == prevDir)
            {
                DirectoryModel curDir = new DirectoryModel(_logger, "");
                curDir.SetDirectoryes(_driveControl.Drives);
                CurrentDirectory = curDir;
            }
        }

        #endregion

        #region methods

        private string GetFileInfo()
        {
            string[] infoArr = SelectedFile.GetInfo();

            string sizeString = string.Empty;
            decimal size = 0;
            if (!SelectedFile.IsDirectory) size = SelectedFile.GetSize();
            string postfix = "Byte";

            for (int i = 0; i < 4; i++)
            {
                if (size > 1024)
                {
                    size /= 1024;
                    if (Enum.IsDefined(typeof(SystemOfUnits), i+1))
                        postfix = ((SystemOfUnits)i+1).ToString();
                }
                else break;
            }

            if (size > 0)
            {
                size = Math.Round(size, 2);
                sizeString = $", Размер: {size} {postfix}";
            }

            string info = $" Создан: {infoArr[2]}, Изменен: {infoArr[3]}{sizeString}";
            return info;
        }

        #endregion
    }
}
