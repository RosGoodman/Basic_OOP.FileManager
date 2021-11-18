
using FileManager.WPF.Model;
using NLog;
using System.Collections.ObjectModel;

namespace FileManager.WPF.ViewModel
{
    internal class FileControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<FileModel> _files;

        public ObservableCollection<FileModel> Files { get => _files; }
    }
}
