

using System;

namespace FileManager.WPF.ViewModel
{
    internal abstract class AbstrctFileControl<T> : BaseViewModel, IFileControl<T> where T : class
    {
        public virtual void Copy(string name, string copyDir, string newPath)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Create(string path)
        {
            throw new System.NotImplementedException();
        }

        public virtual void Delete(T dir)
        {
            throw new System.NotImplementedException();
        }

        public virtual void MoveTo(T dir, string newPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
