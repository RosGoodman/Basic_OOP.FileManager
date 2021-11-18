
using FileManager.WPF.Model;

namespace FileManager.WPF.ViewModel
{
    internal class FileManagerViewModel : BaseViewModel
    {
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
