
namespace FileManager.WPF.ViewModel
{
    /// <summary> Интерфейс контроллеров. </summary>
    internal interface IFileControl
    { 
        /// <summary> Создать файл. </summary>
        /// <param name="path"></param>
        public void Create(string path);

        /// <summary> Копировать файл. </summary>
        /// <param name="name"> Имя файла. </param>
        /// <param name="copyDir"> Полный путь текущей директории. </param>
        /// <param name="newPath"> Новй путь текущей директории. </param>
        public void Copy(string name, string copyDir, string newPath);

        /// <summary> Удалить указанный файл. </summary>
        /// <param name="dir"> Полный путь удаляемого файла. </param>
        public void Delete(string dir);

        /// <summary> Переместить / переименовать файл. </summary>
        /// <param name="dir"> Текущий полный путьк файлу. </param>
        /// <param name="newPath"> Новй полный путь к файлу. </param>
        public void MoveTo(string dir, string newPath);
    }
}
