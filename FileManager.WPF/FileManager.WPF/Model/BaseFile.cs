
using NLog;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace FileManager.WPF.Model
{
    public class BaseFile : INotifyPropertyChanged
    {
        protected internal static ILogger _logger;
        protected internal string _fullPath;

        public bool IsDirectory { get; set; }

        public string FullPath
        {
            get => _fullPath;
            //propertyChanged!.
            set => _fullPath = value;
        }

        public BaseFile(ILogger logger, string filePath)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра объекта AbstractModel.");

            _fullPath = filePath;
        }

        public BaseFile(string filePath)
        {
            if(_logger != null) _logger.Info("Создание экземпляра объекта AbstractModel.");

            _fullPath = filePath;
        }

        public virtual long GetSize()
        {
            throw new System.NotImplementedException();
        }

        public virtual string[] GetInfo()
        {
            throw new System.NotImplementedException();
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
