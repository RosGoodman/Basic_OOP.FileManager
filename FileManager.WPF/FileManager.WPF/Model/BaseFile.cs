
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileManager.WPF.Model
{
    /// <summary> Класс описывающий базовую модель файлов и дерикторий. </summary>
    public class BaseFile : INotifyPropertyChanged
    {
        protected internal static ILogger _logger;
        protected internal string _fullPath;
        protected internal string _name;
        private string[] _fileInfo;
        private string _imagePath;

        #region feilds

        /// <summary> Путь к иконке файла/папки. </summary>
        public string ImagePath { get => _imagePath; set => _imagePath = value; }

        /// <summary> Массив с информацией о файле. </summary>
        public string[] FileInfo { get => _fileInfo; set => _fileInfo = value; }

        /// <summary> Краткое наименование файла (имя.расширение). </summary>
        public string Name { get => _name; set => _name = value; }
        
        /// <summary> Является ли файл папкой. </summary>
        internal bool IsDirectory { get; set; }

        /// <summary> Полный путь к файлу. </summary>
        public string FullPath { get => _fullPath; set => _fullPath = value; }

        #endregion

        /// <summary> Создать экземпляр класса BaseFile. </summary>
        /// <param name="logger"> Логгер. </param>
        /// <param name="filePath"> Полный путь к файлу. </param>
        public BaseFile(ILogger logger, string filePath)
        {
            _logger = logger;
            _fullPath = filePath;
        }

        #region methods

        /// <summary> Создать экземпляр класса BaseFile. </summary>
        /// <param name="filePath"> Полный путь к файлу. </param>
        public BaseFile(string filePath) => _fullPath = filePath;

        /// <summary>  </summary>
        /// <param name="newName"></param>
        public void ChangeName(string newName) => _name = newName;

        /// <summary> Получить размер файла. </summary>
        /// <returns> Размер файла KByte. </returns>
        public virtual decimal GetSizeKByte() => 0;

        /// <summary> Получить информацию о файле. </summary>
        /// <returns> Массив строк с данными о файле. </returns>
        public virtual string[] GetInfo() => _fileInfo;

        /// <summary> Получить родительскую директорию файла. </summary>
        /// <returns> Экземпляр BaseFile. </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual BaseFile GetParent() => throw new System.NotImplementedException();

        #endregion


        public event PropertyChangedEventHandler? PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
