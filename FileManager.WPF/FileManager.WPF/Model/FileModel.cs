
using NLog;

namespace FileManager.WPF.Model
{
    internal class FileModel : IFile
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string _fullPath;
        private string _name;
        private DirectoryModel _parent;

        public string FullPath
        {
            get => _fullPath;
            private set => _fullPath = value;
        }

        public FileModel(string filePath, string name, DirectoryModel parent)
        {
            _fullPath = filePath;
            _name = name;
            _parent = parent;
        }

        public string[] GetInfo()
        {
            throw new System.NotImplementedException();
        }

        public long GetSize()
        {
            throw new System.NotImplementedException();
        }
    }
}
