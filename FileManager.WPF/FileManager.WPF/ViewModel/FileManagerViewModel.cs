using FileManager.WPF.Command;
using FileManager.WPF.Model;
using FileManager.WPF.Services.WorkWithFiles;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FileManager.WPF.ViewModel
{
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

        private BaseFile _movableFile;
        private bool _movingFileCut;    //вырезан / скопирован

        #endregion

        #region props

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand ListBoxItemEnterCommand { get; private set; }
        public RelayCommand GoToPreviousDirCommand { get; private set; }
        public RelayCommand ListBoxItemBackspaceCommand { get; private set; }
        public RelayCommand ListBoxItem_Ctrl_X_Command { get; private set; }
        public RelayCommand ListBoxItem_Ctrl_C_Command { get; private set; }
        public RelayCommand ListBoxItem_Ctrl_V_Command { get; private set; }

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
                if (SelectedFile != null && SelectedFile.FileInfo != null) FileInfo = GetFileInfo();
            }
        }

        public ObservableCollection<BaseFile> AllFilesInCurrentDir
        {
            get => _directoryControl.AllFilesInDirectoiy;
            set
            {
                _directoryControl.AllFilesInDirectoiy = value;
                OnPropertyChanged("AllFilesInCurrentDir");
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

            //чтение последнего файла
            JSONFileReader fileReader = new JSONFileReader(_logger);
            CurrentDirectory = fileReader.GetLastDirectory();
            RefreshList();

            //подключение команд
            CreateCommand = new RelayCommand(CreateFileCommand_Execute);
            ListBoxItemEnterCommand = new RelayCommand(OpenDir_Execute);
            GoToPreviousDirCommand = new RelayCommand(GoToPreviousDirectory_Command);
            ListBoxItemBackspaceCommand = new RelayCommand(GoToPreviousDirectory_Command);
            ListBoxItem_Ctrl_X_Command = new RelayCommand(CutFile_Command);
            ListBoxItem_Ctrl_C_Command = new RelayCommand(CopyFile_Command);
            ListBoxItem_Ctrl_V_Command = new RelayCommand(PastFile_Command);
        }

        #region commands

        private void CreateFileCommand_Execute()
        {
            _logger.Info($"Зпауск команды {nameof(CreateFileCommand_Execute)}");
            _fileControl.Create("C:\\temp\\tt.txt");
        }

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
                
            //else
                // try open
        }

        private void GoToPreviousDirectory_Command()
        {
            _logger.Info($"Зпауск команды {nameof(GoToPreviousDirectory_Command)}");

            BaseFile parent = CurrentDirectory.GetParent();
            
            CurrentDirectory = (DirectoryModel)CurrentDirectory.GetParent();
            CurrentDirectory.LoadSubDirectoryes();
            AllFilesInCurrentDir = CurrentDirectory.SubFiles;
            SelectedFile = AllFilesInCurrentDir[0];
        }

        private void CutFile_Command()
        {
            RemamberMovableFile();
            _movingFileCut = true;
        }

        private void CopyFile_Command()
        {
            RemamberMovableFile();
            _movingFileCut = false;
        }

        private async void PastFile_Command()
        {
            await Task.Run(() =>
            {
                if (CurrentDirectory.Name == "MyComputer") return;

                if (_movableFile.IsDirectory)
                    _directoryControl.Copy(_movableFile.Name, _movableFile.FullPath, CurrentDirectory.FullPath);
                else
                    _fileControl.Copy(_movableFile.Name, _movableFile.FullPath, CurrentDirectory.FullPath);

                RefreshList();
                _movingFileCut = false;
            });
        }

        #endregion

        #region methods

        private void RefreshList()
        {
            CurrentDirectory.LoadSubDirectoryes();
            AllFilesInCurrentDir = CurrentDirectory.SubFiles;
            SelectedFile = AllFilesInCurrentDir[0];
        }

        private void RemamberMovableFile()
        {
            if (SelectedFile == null) return;
            _movableFile = SelectedFile;
        }

        private string GetFileInfo()
        {
            string[] infoArr = SelectedFile.FileInfo;
            if (infoArr == null) return null;

            decimal size = 0;
            if (!SelectedFile.IsDirectory) size = SelectedFile.GetSize();
            string sizeString = ConvertByteSizeToString(size);

            string info = $" Создан: {infoArr[2]}, Изменен: {infoArr[3]}{sizeString}";
            return info;
        }

        private string ConvertByteSizeToString(decimal sizeInByte)
        {
            string sizeString = string.Empty;
            string postfix = "Byte";

            for (int i = 0; i < 4; i++)
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
                sizeString = $", Размер: {sizeInByte} {postfix}";
            }
            return sizeString;
        }

        #endregion
    }
}
