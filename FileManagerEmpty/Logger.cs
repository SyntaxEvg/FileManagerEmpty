using lesson3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileManagerEmpty
{
    public static class Logger
    {
        static object Lock=new object();
        public static Setting setting =new();
        static bool LoggerWrite=true;
        readonly static string PuthFolderErrorLog = setting.GetCurrentDirectory + "/errors/";
        static string PuthLogger =Path.Combine(PuthFolderErrorLog, setting.errorsLogFile);

        public static  void WriteLog(ref Exception ex)
        {
            if (!LoggerWrite)
            {
                return; //логер сломан и записи не ведет
            }
            if (File.Exists(PuthLogger))
            {
                lock (Lock)
                {
                    try
                    {
                        var jsonString = JsonSerializer.Serialize(ex.Message);
                        File.WriteAllText(Path.Combine(PuthLogger, PuthLogger), jsonString +Environment.NewLine);
                    }
                    catch
                    {                     
                        Console.Write($"Ошибка записи в файл {PuthLogger}");
                    }
                }
            }
        }
        public static void ErrorsLogFile()
        {
           
            try
            {
                if (!Directory.Exists(PuthFolderErrorLog))
                {
                    Directory.CreateDirectory(PuthFolderErrorLog);
                }
                if (!File.Exists(PuthLogger))
                {
                    File.Create(PuthLogger);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex} Логер не будет вести журнал");
                LoggerWrite = false;
            }
        }
    }
}
