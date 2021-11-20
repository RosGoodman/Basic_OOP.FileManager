
using NLog;
using System.IO;

namespace FileManager.WPF.Model
{
    internal class DriveModel : BaseFile
    {
        public DriveInfo _driveInfo;

        private string[] _directoryes;
        private string[] _files;

        public string[] Directoryes
        {
            get => _directoryes;
            private set => _directoryes = value;
        }

        public string[] Files
        {
            get => _files;
            private set => _files = value;
        }

        public DriveModel(ILogger logger, string filePath)
            : base(logger, filePath)
        {
            IsDirectory = false;
        }

        public override string ToString()
        {
            _driveInfo = new DriveInfo(FullPath);
            return _driveInfo.Name;
        }

        private void GetSubDirectoryes()
        {
            try
            {
                Directoryes = Directory.GetDirectories(FullPath);
                Files = Directory.GetFiles(FullPath);
            }
            catch { }
        }
    }
}
