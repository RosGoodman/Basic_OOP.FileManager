
namespace FileManager.WPF.ViewModel
{
    internal interface IFileControl<T> where T : class
    {
        public void Create(string path);

        public void Copy(string name, string copyDir, string newPath);

        public void Delete(T dir);

        public void MoveTo(T dir, string newPath);
    }
}
