
using FileManager.WPF.Model;
using NLog;
using System.Collections.ObjectModel;
using System.IO;

namespace FileManager.WPF.ViewModel.Controls
{
    internal class DriveControl : AbstrctBaseViewModel<DriveModel>
    {
        private ILogger _logger;
        private ObservableCollection<BaseFile> _drives = new ObservableCollection<BaseFile>();

        public ObservableCollection<BaseFile> Drives { get => _drives; private set => _drives = value; }

        public DriveControl(ILogger logger)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра класса DriveInfo.");
            Drives = GetDrives();
        }

        private ObservableCollection<BaseFile> GetDrives()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            ObservableCollection<BaseFile> drives = new ObservableCollection<BaseFile>();

            foreach (var drive in allDrives)
            {
                drives.Add(new BaseFile(drive.Name));
            }

            return drives;
        }
    }
}
