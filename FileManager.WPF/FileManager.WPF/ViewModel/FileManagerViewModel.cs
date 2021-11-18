
using FileManager.WPF.Model;
using NLog;

namespace FileManager.WPF.ViewModel
{
    internal class FileManagerViewModel : BaseViewModel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DirectoryModel _currentDirectory;
        private FileModel _currentFile;

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

        public FileManagerViewModel()
        {

        }
    }
}
