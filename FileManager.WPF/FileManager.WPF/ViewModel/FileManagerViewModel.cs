﻿using FileManager.WPF.Command;
using FileManager.WPF.Model;
using FileManager.WPF.Services.WorkWithFiles;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.IO;
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
        private string _selectFileSize;

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
        public RelayCommand RenameFile_Command { get; private set; }
        public RelayCommand GetFileInfoCommand { get; private set; }
        public RelayCommand FindCommand { get;private set; }

        /// <summary> Новое имя для переименования файла. </summary>
        public string NewFileName { get; set; }

        public string SearchLine { get; set; }

        public string BackImagePath { get => Path.GetFullPath("Images/previous.png"); }

        public string[] SelectFileInfo { get; private set; }
        public string SelectFileSize
        {
            get => _selectFileSize;
            private set
            {
                _selectFileSize = value;
                OnPropertyChanged("SelectFileSize");
            }
        }

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
                CurrentPath = _currentDirectory.FullPath;
                if (SelectedFile != null && SelectedFile.FileInfo != null) FileInfo = GetFileInfo();

                OnPropertyChanged("SelectedFile");
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
            DirectoryReaderFromJSON fileReader = new DirectoryReaderFromJSON(_logger);
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
            RenameFile_Command = new RelayCommand(MoveTo_Command);
            GetFileInfoCommand = new RelayCommand(GetFileInfo_Command);
            FindCommand = new RelayCommand(Find_Command);
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
        }

        private void GoToPreviousDirectory_Command()
        {
            _logger.Info($"Зпауск команды {nameof(GoToPreviousDirectory_Command)}");

            BaseFile parent = CurrentDirectory.GetParent();
            
            CurrentDirectory = (DirectoryModel)parent;
            RefreshList();
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

        private void MoveTo_Command()
        {
            if (SelectedFile.IsDirectory)
                _directoryControl.MoveTo(SelectedFile.FullPath, $"{SelectedFile.GetParent().FullPath}\\{NewFileName}");
            else
                _fileControl.MoveTo(SelectedFile.FullPath, $"{SelectedFile.GetParent().FullPath}\\{NewFileName}");

            RefreshList();
        }

        private async void GetFileInfo_Command()
        {
            SelectFileInfo = SelectedFile.GetInfo();
            await Task.Run(() =>
            {
                decimal sizeByte = SelectedFile.GetSizeKByte();
                SelectFileSize = ConvertByteSizeToString(sizeByte);
            });
        }

        private async void Find_Command()
        {

        }

        #endregion

        #region methods

        private void Copy(IFileControl fileControl)
        {
            fileControl.Copy(_movableFile.Name, _movableFile.FullPath, CurrentDirectory.FullPath);
            if (_movingFileCut) 
                fileControl.Delete(_movableFile.FullPath);
        }

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
            if (!SelectedFile.IsDirectory) size = SelectedFile.GetSizeKByte();
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
                sizeString = $"Размер: {sizeInByte} {postfix}";
            }
            return sizeString;
        }

        #endregion
    }
}
