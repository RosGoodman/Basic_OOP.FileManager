

using System;

namespace FileManager.WPF.ViewModel
{
    internal abstract class AbstrctFileControl : BaseViewModel, IFileControl
    {
        public virtual void Copy(string name, string copyDir, string newPath)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Create(string path)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Delete(string dir)
        {
            throw new System.NotImplementedException();
        }

        public virtual void MoveTo(string dir, string newPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
