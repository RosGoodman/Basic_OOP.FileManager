
namespace FileManager.WPF.ViewModel
{
    /// <summary> Абстрактное представление контроллера файлов. </summary>
    internal abstract class AbstrctFileControl : BaseViewModel, IFileControl
    {
        /// <summary> Копировать файл. </summary>
        /// <param name="name"> Наименование файла. </param>
        /// <param name="copyDir"> Начальная директория. </param>
        /// <param name="newPath"> Конечная директория. </param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Copy(string name, string copyDir, string newPath)
        {
            throw new System.NotImplementedException();
        }

        /// <summary> Создать файл. </summary>
        /// <param name="path"> Полный путь файла. </param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Create(string path)
        {
            throw new System.NotImplementedException();
        }

        /// <summary> Удалить файл. </summary>
        /// <param name="dir"> Полный путь к файлу. </param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void Delete(string dir)
        {
            throw new System.NotImplementedException();
        }

        /// <summary> Переместить / переименовать файл. </summary>
        /// <param name="dir"> Текущий полный путь к файлу. </param>
        /// <param name="newPath"> Новый полный путь к файлу. </param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void MoveTo(string dir, string newPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
