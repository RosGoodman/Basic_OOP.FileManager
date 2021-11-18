
using FileManager.WPF.Model;
using System.Collections.ObjectModel;

namespace FileManager.WPF.ViewModel
{
    internal class FileControl
    {
        private ObservableCollection<FileModel> _files;

        public ObservableCollection<FileModel> Files { get => _files; }
    }
}
