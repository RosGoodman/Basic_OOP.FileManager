
using NLog;
using System;
using System.IO;

namespace FileManager.WPF.Services.WorkWithFiles
{
    /// <summary> Класс,описывающий запись пути. </summary>
    internal struct DirectoryWriterToTXT
    {
        private static readonly string _path = Directory.GetCurrentDirectory() + "\\lastDir.txt";

        /// <summary> Записать путь последней открытой директории. </summary>
        /// <param name="logger"> Логгер. </param>
        /// <param name="lastPath"> Полный путь директории. </param>
        public static void WriteLastPath(ILogger logger, string lastPath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_path, false, System.Text.Encoding.Default))
                {
                    sw.Write(lastPath);
                }
            }
            catch (Exception e) { logger.Error("Ошибка при попытке записи файла."); }
        }
    }
}
