
using FileManager.WPF.Command;
using FileManager.WPF.Model;
using FileManager.WPF.Services.WorkWithFiles;
using NLog;
using System.Collections.ObjectModel;

namespace FileManager.WPF.ViewModel
{
    public class FileManagerViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        private DirectoryControl _directoryControl;
        private FileControl _fileControl;

        private BaseFile _selectedFile;

        private DirectoryModel _currentDirectory;
        //private FileModel _currentFile;

        #region props

        public RelayCommand CreateCommand { get; private set; }

        public BaseFile SelectedFile { get; set; }
        public ObservableCollection<DirectoryModel> Directoryes
        {
            get => _directoryControl.Directoryes;
            private set
            {
                _directoryControl.Directoryes = value;
                OnPropertyChanged(nameof(Directoryes));
            }
        }

        public DirectoryModel CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;

                Directoryes = new ObservableCollection<DirectoryModel>();
                foreach (string dir in CurrentDirectory.Directoryes)
                {
                    Directoryes.Add(new DirectoryModel(dir));
                }
                OnPropertyChanged(nameof(CurrentDirectory));
            }
        }

        #endregion

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
        }

        #region commands

        public void CreateFileCommand_Execute()
        {
            _fileControl.Create("C:\\temp\\tt.txt");
        }

        public bool CreateFileCommand_CanExecute()
        {
            return true;
        }

        #endregion

        #region methods

        

        #endregion
    }
}
