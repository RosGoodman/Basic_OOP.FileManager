
using FileManager.WPF.Model;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    internal class DirectoryControl : BaseViewModel
    {
        private ObservableCollection<DirectoryModel> _directories;

        public ObservableCollection<DirectoryModel> Directoryes { get => _directories; }

        public static string[] GetDirectoryes(string dirName)
        {
            return Directory.GetDirectories(dirName);
        }

        public void Create(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (directoryInfo.Exists)
                    directoryInfo.Create();
            }
            catch (Exception ex)
            {
                //заменить на логгер
                Console.WriteLine(ex);
            }
        }

        public void CreateSubDirectory(string subPath)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(subPath);
                if (directoryInfo.Exists)
                    directoryInfo.CreateSubdirectory(subPath);
            }
            catch(Exception ex)
            {
                //заменить на логгер
                Console.WriteLine(ex);
            }
        }

        public void Delete(DirectoryModel dir)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir.);
                dirInfo.Delete(true);
                Console.WriteLine("Каталог удален");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
