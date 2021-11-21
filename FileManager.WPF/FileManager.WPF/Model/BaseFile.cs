
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileManager.WPF.Model
{
    public class BaseFile : INotifyPropertyChanged
    {
        protected internal static ILogger _logger;
        protected internal string _fullPath;
        protected internal string _name;
        private string[] _fileInfo;
        private string _imagePath;

        public string ImagePath { get => _imagePath; set => _imagePath = value; }

        public string[] FileInfo
        {
            get => _fileInfo;
            set => _fileInfo = value;
        }

        public string Name { get => _name; set => _name = value; }
        
        internal bool IsDirectory { get; set; }

        public string FullPath
        {
            get => _fullPath;
            //propertyChanged!.
            set => _fullPath = value;
        }

        public BaseFile(ILogger logger, string filePath)
        {
            _logger = logger;
            //_logger.Info("Создание экземпляра объекта AbstractModel.");

            _fullPath = filePath;
        }

        public BaseFile(string filePath)
        {
            //if(_logger != null) _logger.Info("Создание экземпляра объекта AbstractModel.");

            _fullPath = filePath;
        }

        public void ChangeName(string newName)
        {
            _name = newName;
        }

        public virtual decimal GetSize()
        {
            return 0;
        }

        public virtual string[] GetInfo()
        {
            return _fileInfo;
        }

        public virtual BaseFile GetParent()
        {
            throw new System.NotImplementedException();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
