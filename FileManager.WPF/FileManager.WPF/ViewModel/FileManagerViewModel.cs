
using FileManager.WPF.Model;
using NLog;
using System.Windows.Input;

namespace FileManager.WPF.ViewModel
{
    internal class FileManagerViewModel : BaseViewModel
    {
        private readonly ILogger _logger;

        private DirectoryControl _directoryControl;
        private FileControl _fileControl;

        private BaseFile _selectedFile;

        private DirectoryModel _currentDirectory;
        private FileModel _currentFile;

        #region props

        public ICommand CreateCommand { get; set; }

        public BaseFile SelectedFile{get;set;}

        public DirectoryModel CurrentDirectory
        {
            get => _currentDirectory;
            set
            {
                _currentDirectory = value;
                OnPropertyChanged("CurrentDirectory");
            }
        }

        public FileModel CurrentFile
        {
            get => _currentFile;
            set
            {
                _currentFile = value;
                OnPropertyChanged("CurrentDirectory");
            }
        }

        #endregion

        public FileManagerViewModel(ILogger logger)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра класса FileManagerViewModel.");

            _directoryControl = new DirectoryControl(_logger);
            _fileControl = new FileControl(_logger);
        }

        #region commands

        public void CreateCommand_Execute(object param)
        {
            //if(param.)
        }

        #endregion
    }
}
