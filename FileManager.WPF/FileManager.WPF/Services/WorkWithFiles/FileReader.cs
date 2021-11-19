
using FileManager.WPF.Model;
using NLog;
using System;
using System.IO;
using System.Text.Json;

namespace FileManager.WPF.Services.WorkWithFiles
{
    internal class JSONFileReader
    {
        private static readonly string Path = Directory.GetCurrentDirectory() + "\\lastDir.json";
        private readonly ILogger _logger;

        public JSONFileReader(ILogger logger)
        {
            _logger = logger;
            _logger.Info("Создание экземпляра класса для чтения JSON.");
            
        }

        public DirectoryModel GetLastDirectory()
        {
            string json = ReadFile();
            if (json != string.Empty)
                return Deserialize(json);

            return new DirectoryModel("C:\\");
        }

        private DirectoryModel Deserialize(string json)
        {
            return JsonSerializer.Deserialize<DirectoryModel>(json);
        }

        private string ReadFile()
        {
            if (!File.Exists(Path))
            {
                _logger.Warn($"{File.Exists(Path)} - файл с JSON не найден.");
                return String.Empty;
            }
            try
            {
                using (StreamReader sr = new StreamReader(Path))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"{ex} - ошибка при попытке чтения файла.");
            }

            return string.Empty;
        }
    }
}
