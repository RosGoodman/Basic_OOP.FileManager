
using FileManager.WPF.Model;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace FileManager.WPF.ViewModel
{
    /// <summary> Класс, описывающий контроллер директорий. </summary>
    internal class DirectoryControl : AbstrctFileControl
    {
        private static ILogger _logger;
        private DirectoryInfo _dirInfo;
        private ObservableCollection<BaseFile> _directories;
        private ObservableCollection<FileModel> _files;

        /// <summary> Коллекция всех файлов и директорий, содержащихся в текущей. </summary>
        public ObservableCollection<BaseFile> AllFilesInDirectoiy
        {
            get => _directories;
            set => _directories = value;
        }
        //public ObservableCollection<FileModel> Files { get => _files; }

        /// <summary> Создание экземпляра класса.</summary>
        /// <param name="logger"> Логгер. </param>
        public DirectoryControl(ILogger logger)
        {
            _logger = logger;
            _logger.Debug("Создание экземпляра класса DirectoryControl.");

            AllFilesInDirectoiy = new ObservableCollection<BaseFile>();
        }

        #region methods

        ///// <summary> Получить массив с наименованиями директорий, содержащихся в текущей. </summary>
        ///// <param name="dirName"> Полный путь к директории. </param>
        ///// <returns> Массив  </returns>
        //public static string[] GetDirectoryes(string dirName)
        //{
        //    _logger.Debug("Получение списка директорий.");
        //    return Directory.GetDirectories(dirName);
        //}

        /// <summary> Создать директорию по указанному пути. </summary>
        /// <param name="path"> Полный путь родительской директории. </param>
        public override void Create(string path)
        {
            try
            {
                _dirInfo = new DirectoryInfo(path);
                if (_dirInfo.Exists)
                    _dirInfo.Create();
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания директории.");
            }
        }

        /// <summary> Удалить директорию. </summary>
        /// <param name="dirPath"> Полный путь удаляемой директории. </param>
        public override void Delete(string dirPath)
        {
            try
            {
                Directory.Delete(dirPath, true);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке удаления директории.");
            }
        }

        /// <summary> Переименовать / переместить директорию. </summary>
        /// <param name="dirPath"> Текущий полный путь к директории. </param>
        /// <param name="newPath"> Новый полный путь к директории. </param>
        public override void MoveTo(string dirPath, string newPath)
        {
            try
            {
                _dirInfo = new DirectoryInfo(dirPath);
                if (_dirInfo.Exists && !Directory.Exists(newPath))
                    _dirInfo.MoveTo(newPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке перемещения директории.");
            }
        }

        /// <summary> Копировать директорию. </summary>
        /// <param name="fileName"> Имя копируемой директории. </param>
        /// <param name="copyDir"> Полный путь копируемой директории. </param>
        /// <param name="newPath"> Полный путь новой директории. </param>
        public override void Copy(string fileName, string copyDir, string newPath)
        {
            //Проверяем - если директории не существует, то создаём;
            if (Directory.Exists(newPath + "\\" + fileName) != true)
            {
                Directory.CreateDirectory(newPath + "\\" + fileName);
            }

            RecursiveCopy(fileName, copyDir, newPath);
        }

        /// <summary> Рекурсивное копирование файлов из указанной директории. </summary>
        /// <param name="fileName"> Имя копируемой директории. </param>
        /// <param name="copyDir"> Полный путь копируемой директории. </param>
        /// <param name="newPath"> Полный путь новой директории. </param>
        private void RecursiveCopy(string fileName, string copyDir, string newPath)
        {
            //Берём нашу исходную папку
            DirectoryInfo dir_inf = new DirectoryInfo(copyDir);
            //Перебираем все внутренние папки
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                //Проверяем - если директории не существует, то создаём;
                if (Directory.Exists(newPath + "\\" + fileName + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(newPath + "\\" + fileName + "\\" + dir.Name);
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                RecursiveCopy(dir.Name, dir.FullName, newPath + "\\" + fileName);
            }

            //Перебираем файлики в папке источнике.
            foreach (string file in Directory.GetFiles(copyDir))
            {
                //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
                string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
                //Копируем файлик с перезаписью из источника в приёмник.
                File.Copy(file, newPath + "\\" + fileName + "\\" + filik, true);
            }
        }


        public void CreateSubDirectory(string subPath)
        {
            try
            {
                _dirInfo = new DirectoryInfo(subPath);
                if (_dirInfo.Exists)
                    _dirInfo.CreateSubdirectory(subPath);
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке создания дочерней директории.");
            }
        }

        //void perebor_updates(string begin_dir, string end_dir)
        //{
        //    void perebor_updates(string begin_dir, string end_dir)
        //    {
        //        //Берём нашу исходную папку
        //        DirectoryInfo dir_inf = new DirectoryInfo(begin_dir);
        //        //Перебираем все внутренние папки
        //        foreach (DirectoryInfo dir in dir_inf.GetDirectories())
        //        {
        //            //Проверяем - если директории не существует, то создаём;
        //            if (Directory.Exists(end_dir + "\\" + dir.Name) != true)
        //            {
        //                Directory.CreateDirectory(end_dir + "\\" + dir.Name);
        //            }

        //            //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
        //            perebor_updates(dir.FullName, end_dir + "\\" + dir.Name);
        //        }

        //        //Перебираем файлики в папке источнике.
        //        foreach (string file in Directory.GetFiles(begin_dir))
        //        {
        //            //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
        //            string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
        //            //Копируем файлик с перезаписью из источника в приёмник.
        //            File.Copy(file, end_dir + "\\" + filik, true);
        //        }
        //    }
        //}

        #endregion
    }
}
