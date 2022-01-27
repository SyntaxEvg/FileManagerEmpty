//namespace lesson3
//{
//   public static class Setting
//    {
//      //  public int windowHeight { get; }
//      //  public int windowWidth { get; }
//        //public int bufferHeight { get; }
//        //public int bufferWidth { get; }
//        public int infoWindowHeight { get; }
//        public int commandLineHeight { get; }
//        public int pageLines { get; }
//        public string settingsFile { get; }
//        public int pageNumber { get; set; }
//        public string lastPath { get; set; }
//         public string errorsLogFile = "Errors.json";
//        public Setting()
//        {

//            infoWindowHeight = 5;
//            commandLineHeight = 2;
//            //lastPath = Environment.CurrentDirectory;
//            lastPath = @"C:\";


//            settingsFile = "set.json";
//            pageNumber = 1;
//            pageLines = 45;
//            //windowHeight = pageLines + infoWindowHeight + commandLineHeight + 1;
//           // windowWidth = 200;
//            //bufferHeight = windowHeight;
//            //bufferWidth = 200;
//        }
//static void CheckSettingsFile(ref Setting set)
//{
//    string path = Directory.GetCurrentDirectory();
//    if (File.Exists(Path.Combine(path, set.settingsFile)))
//    {
//        try
//        {
//            string jsonSettings = File.ReadAllText(Path.Combine(path, set.settingsFile));
//            var serr = JsonSerializer.Deserialize<Setting>(jsonSettings);
//            if (serr != null)
//            {
//                string[] directories = Directory.GetDirectories(serr.lastPath);
//                //Console.SetWindowSize(serr.windowWidth, serr.windowHeight);
//                //Console.SetBufferSize(serr.bufferWidth, serr.bufferHeight);
//                return;
//            }

//        }
//        catch (Exception e)
//        {
//            StandAtCommandLine();
//            Console.Write($"Ошибка при чтении настроек! Подробно в файле {set.errorsLogFile}. Настройки сброшены");
//            if (File.Exists(Path.Combine(path, set.errorsLogFile)))
//            {
//                var jsonString = JsonSerializer.Serialize(e.Message);
//                try
//                {
//                    File.WriteAllText(Path.Combine(path, set.errorsLogFile), jsonString);
//                }
//                catch
//                {
//                    Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
//                }
//            }
//            Console.ReadKey();
//        }
//    }
//}
//static void SaveSettingsFile(Setting set)
//{
//    string path = Directory.GetCurrentDirectory();
//    string jsonSettings = JsonSerializer.Serialize(set);
//    try
//    {
//        File.WriteAllText(Path.Combine(path, set.settingsFile), jsonSettings);
//    }
//    catch (Exception e)
//    {
//        StandAtCommandLine();
//        Console.Write("Ошибка при записи файла настроек!");
//        if (File.Exists(Path.Combine(path, set.errorsLogFile)))
//        {
//            var jsonString = JsonSerializer.Serialize(e.Message);
//            try
//            {
//                File.WriteAllText(Path.Combine(path, set.errorsLogFile), jsonString);
//            }
//            catch
//            {
//                Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
//            }
//        }
//        Console.ReadKey();
//    }
//}
//    }
//}
