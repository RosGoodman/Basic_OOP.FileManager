
using FileManager.WPF.Command;
using FileManager.WPF.Model;
using NLog;

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

        public DirectoryModel CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;
                OnPropertyChanged("CurrentDirectory");
            }
        }

        //public FileModel CurrentFile
        //{
        //    get => _currentFile;
        //    set
        //    {
        //        _currentFile = value;
        //        OnPropertyChanged("CurrentDirectory");
        //    }
        //}

        #endregion

        public FileManagerViewModel(ILogger logger)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра класса FileManagerViewModel.");

            this.CreateCommand = new RelayCommand(CreateFileCommand_Execute);

            _directoryControl = new DirectoryControl(_logger);
            _fileControl = new FileControl(_logger);
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
    }
}
