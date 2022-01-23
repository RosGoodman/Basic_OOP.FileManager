
using NLog;
using System;
using System.IO;

namespace FileManager.WPF.Services.WorkWithFiles
{
    /// <summary> Класс, описывающий получение записанной директории из JSON. </summary>
    internal struct DirectoryReaderFromTXT
    {
        private static readonly string _path = Directory.GetCurrentDirectory() + "\\lastDir.txt";

        /// <summary> Прочитать из файла путь к последней открытой директории. </summary>
        /// <param name="logger"> Логгер. </param>
        /// <returns> Путь к директории. </returns>
        public static string ReadDirectoryPath(ILogger logger)
        {
            string path;
            try
            {
                using (StreamReader sr = new StreamReader(_path))
                {
                    path = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                logger.Error($"{e} - ошибка при попытке чтения файла. {_path}");
                return "C:\\";
            }
            return path;
        }
    }
}
