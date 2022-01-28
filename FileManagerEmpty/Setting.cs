using FileManagerEmpty;
using System.Text.Json;

namespace lesson3
{
    public  class JsonSerWrite//можно любые параметры добавить ,в задании кол-во управлен.страницей требуется
    {
        public  int PageLines { get; set; } = 8;//  макс 8 из за обсобенности работы кривого интрефейса без поддержки контейнера
        public string Folder { get; set; }
    }

    public  class Setting
    {
        public  readonly string GetCurrentDirectory = Directory.GetCurrentDirectory();
        public  readonly string ErrorsLogFile = "Errors.json";
        public  readonly string Config = "Config.json";
        public Setting()
        {
           
        }
        public JsonSerWrite GetSettingConfig()
        {
            var path =Path.Combine(GetCurrentDirectory, Config);
            if (File.Exists(path))
            {
                try
                {
                    string jsonSettings = File.ReadAllText(path);
                    var set = JsonSerializer.Deserialize<JsonSerWrite>(jsonSettings);
                    if (set != null)
                    {
                        return set ;
                    }
                    return set;

                }
                catch (Exception e)
                {
                    Console.Write($"Ошибка при чтении настроек!");
                    Logger.WriteLog(ref e);
                }
            }
            return null;
        }
        public void SaveSettingsFile(JsonSerWrite js)
        {
            var path = Path.Combine(GetCurrentDirectory, Config);
            string jsonSettings = JsonSerializer.Serialize(path);
            try
            {
                File.WriteAllText(path, jsonSettings);
            }
            catch (Exception e)
            {
                Console.Write("Ошибка при записи файла настроек, смотрите лог!");
                Logger.WriteLog(ref e);               
            }
        }
    }
}
