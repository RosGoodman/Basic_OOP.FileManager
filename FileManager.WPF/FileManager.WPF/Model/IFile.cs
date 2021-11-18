
namespace FileManager.WPF.Model
{
    internal interface IFile
    {
        public long GetSize();

        public string[] GetInfo();
    }
}
