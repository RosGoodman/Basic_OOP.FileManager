using FileManager.WPF.Command;
using FileManager.WPF.Model;
using FileManager.WPF.Services.WorkWithFiles;
using NLog;
using System.Collections.ObjectModel;

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

        #endregion

        #region props

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand ListBoxItemDoubleClick { get; private set; }

        public BaseFile SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
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

        //public string CurrentPath
        //{
        //    get => _currentPath;
        //    private set
        //    {
        //        _currentPath = value;
        //        OnPropertyChanged(nameof(CurrentPath));
        //    }
        //}

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
                    OnPropertyChanged(nameof(CurrentDirectory));
                }
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
            SelectedFile = Directoryes[0];

            //подключение команд
            this.CreateCommand = new RelayCommand(CreateFileCommand_Execute);
            this.ListBoxItemDoubleClick = new RelayCommand(OpenDir_Execute);
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

        #endregion
    }
}
