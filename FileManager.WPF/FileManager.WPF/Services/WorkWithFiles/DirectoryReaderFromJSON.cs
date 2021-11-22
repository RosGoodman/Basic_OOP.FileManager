
using FileManager.WPF.Model;
using NLog;
using System;
using System.IO;
using System.Text.Json;

namespace FileManager.WPF.Services.WorkWithFiles
{
    /// <summary> Класс, описывающий получение записанной директории из JSON. </summary>
    internal class DirectoryReaderFromJSON
    {
        private static readonly string _path = Directory.GetCurrentDirectory() + "\\lastDir.json";
        private readonly ILogger _logger;

        /// <summary> Создание экземпляра класса. </summary>
        /// <param name="logger"> Логгер. </param>
        public DirectoryReaderFromJSON(ILogger logger)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра класса для чтения JSON.");
        }

        /// <summary> Получить последнюю директорию. </summary>
        /// <returns> Стратовая директория. </returns>
        public DirectoryModel GetLastDirectory()
        {
            string json = ReadFile(_path);
            DirectoryModel directoryModel = null;

            if (json != string.Empty) directoryModel = JsonSerializer.Deserialize<DirectoryModel>(json);
            if (directoryModel != null) return directoryModel;

            return new DirectoryModel(_logger, "C:\\");
        }

        /// <summary> Прочитать JSON файл. </summary>
        /// <returns></returns>
        private string ReadFile(string path)
        {
            if (!File.Exists(path))
            {
                _logger.Warn($"{File.Exists(path)} - файл с JSON не найден.");
                return String.Empty;
            }
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex) { _logger.Error($"{ex} - ошибка при попытке чтения файла."); }

            return string.Empty;
        }
    }
}
