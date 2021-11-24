
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

        /// <summary> Создать директорию по указанному пути. </summary>
        /// <param name="path"> Полный путь родительской директории. </param>
        public override void Create(string path)
        {
            try
            {
                _dirInfo = new DirectoryInfo(path);
                if (!_dirInfo.Exists)
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
            if (Directory.Exists($"{newPath}\\{fileName}") != true)
            {
                Directory.CreateDirectory($"{newPath}\\{fileName}");
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
                if (Directory.Exists($"{newPath}\\{fileName}\\{dir.Name}") != true)
                {
                    Directory.CreateDirectory($"{newPath}\\{fileName}\\{dir.Name}");
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                RecursiveCopy(dir.Name, dir.FullName, $"{newPath}\\{fileName}");
            }

            //Перебираем файлики в папке источнике.
            foreach (string file in Directory.GetFiles(copyDir))
            {
                //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
                string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));

                try
                {
                    //Копируем файлик с перезаписью из источника в приёмник.
                    File.Copy(file, $"{newPath}\\{fileName}\\{filik}", true);
                }
                catch(Exception ex) { _logger.Error($"{ex} - ошибка при попытке доступа к файлу."); }
            }
        }

        /// <summary> Создать директорию по указанному пути. </summary>
        /// <param name="subPath"> Полный путь новой директории. </param>
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

        #region search methods

        /// <summary> Найти файлы и папки по маске в указанной директории. </summary>
        /// <param name="directory"> Директория поиска. </param>
        /// <param name="mask"> Маска файла. </param>
        /// <returns> Условная директория с найденными файлами. </returns>
        public DirectoryModel FileSearchByMask(string directory, string mask)
        {
            DirectoryModel newDir = new DirectoryModel(_logger, directory);

            LoadFilesInCurrentDir_ForSearch(newDir, mask);
            RecursiveSearch(newDir, mask);

            return newDir;
        }

        /// <summary> Загрузить найденные по маске файлы из текущей директории. </summary>
        /// <param name="newDir"> Текущая директория поиска. </param>
        /// <param name="mask"> Маска файла. </param>
        private void LoadFilesInCurrentDir_ForSearch(DirectoryModel newDir, string mask)
        {
            DirectoryInfo dir_inf = new DirectoryInfo(newDir.FullPath);

            try
            {
                var thisDirs = Directory.GetDirectories(dir_inf.FullName, mask);
                newDir.LoadDirectoryesToSubFiles(thisDirs);
            }
            catch (UnauthorizedAccessException ex) { _logger.Error($"{ex.Message} - ошибка при попытке доступа к файлу {dir_inf.FullName}"); }
            try
            {
                newDir.LoadFilesToSubFiles(Directory.GetFiles(dir_inf.FullName, mask));
            }
            catch (UnauthorizedAccessException ex) { _logger.Error($"{ex.Message} - ошибка при попытке доступа к файлу {dir_inf.FullName}"); }
        }

        /// <summary> Рекурсивный поиск по вложенным директориям в указанной. </summary>
        /// <param name="newDir"> Директория поиска. </param>
        /// <param name="mask"> Маска файла. </param>
        private void RecursiveSearch(DirectoryModel newDir, string mask)
        {
            DirectoryInfo dir_inf = new DirectoryInfo(newDir.FullPath);
            DirectoryInfo[] dirs = null;

            try { dirs = dir_inf.GetDirectories(); }
            catch (UnauthorizedAccessException ex) 
            {
                _logger.Error($"{ex.Message} - ошибка при попытке доступа к файлу"); 
                return;
            }

            foreach (DirectoryInfo dir in dirs)
            {
                try
                {
                    var thisDirs = Directory.GetDirectories(dir.FullName, mask);
                    newDir.LoadDirectoryesToSubFiles(thisDirs);
                }
                catch(UnauthorizedAccessException ex) { _logger.Error($"{ex.Message} - ошибка при попытке доступа к файлу {dir.FullName}"); }
                try
                {
                    newDir.LoadFilesToSubFiles(Directory.GetFiles(dir.FullName, mask));
                }
                catch (UnauthorizedAccessException ex) { _logger.Error($"{ex.Message} - ошибка при попытке доступа к файлу {dir.FullName}"); }

                RecursiveSearch(new DirectoryModel(_logger, dir.FullName), mask);
            }
        }

        #endregion

        #endregion
    }
}
