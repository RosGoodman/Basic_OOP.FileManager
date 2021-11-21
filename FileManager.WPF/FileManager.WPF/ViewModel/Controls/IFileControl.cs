
namespace FileManager.WPF.ViewModel
{
    internal interface IFileControl
    { 
        public void Create(string path);

        public void Copy(string name, string copyDir, string newPath);

        public void Delete(string dir);

        public void MoveTo(string dir, string newPath);
    }
}
