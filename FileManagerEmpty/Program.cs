using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace FileManagerEmpty
{

    //Вывод дерева файловой системы с условием “пейджинга”
    //ls C:\Source -p 2
    //Копирование каталога
    //cp C:\Source D:\Target
    //Копирование файла
    //cp C:\source.txt D:\target.txt
    //Удаление каталога рекурсивно
    //rm C:\Source
    //Удаление файла
    //rm C:\source.txt
    //Вывод информации
    //file C:\source.txt
    //Функции и требования
    //Просмотр файловой структуры
    //Поддержка копирование файлов, каталогов
    //Поддержка удаление файлов, каталогов
    //Получение информации о размерах, системных атрибутов файла, каталога
    //Вывод файловой структуры должен быть постраничным
    //В конфигурационном файле должна быть настройка вывода количества элементов на страницу
    //При выходе должно сохраняться, последнее состояние
    //Должны быть комментарии в коде
    // Должна быть документация к проекту в формате md
    //Приложение должно обрабатывать непредвиденные ситуации (не падать)
    //При успешном выполнение предыдущих пунктов – реализовать сохранение ошибки в текстовом файле в каталоге errors/random_name_exception.txt
    //При успешном выполнение предыдущих пунктов – реализовать движение по истории команд (стрелочки вверх, вниз)
    internal class Program
    {
        static Dictionary<string, string> collectionHelp = new Dictionary<string, string>();
        static string Folder = @"C:\";
        static string stringa = new String('=', 30);
        public delegate void OnKey(ConsoleKeyInfo key);
        public event OnKey KeyPr;

        public static class Help
        {
            public static void ConsoleHelp()
            {
                //
                Console.WriteLine();
                if (!collectionHelp.ContainsKey("ls"))
                {
                    collectionHelp.Add("ls", "Вывод дерева файловой системы с условием “пейджинга”\nПараметр -p, пример ls C:\\Source -p 2");
                    collectionHelp.Add("cp", "Копирование каталога,пример C:\\Source D:\\Target или C:\\source.txt D:\\target.txt");
                    collectionHelp.Add("rm", "Удаление каталога рекурсивно/Файла, пример rm C:\\Source или rm C:\\source.txt");
                    collectionHelp.Add("file", "Вывод информации");
                    collectionHelp.Add("Help", "Все доступные команды,пример file C:\\source.txt");
                }
                foreach (var item in collectionHelp)
                {
                    Console.WriteLine(item);
                }

            }
        }
        class CommandConsole
        {
            //public string ConsoleRead()
            //{
            //    string Commanda = null;
            //    List<string> list = new List<string>();
            //    StringBuilder stroka = new();
            //    while (true)
            //    {
            //        Commanda = null;
            //        ConsoleKeyInfo key = Console.ReadKey(false);
            //        stroka.Append(key.Key.ToString());

            //        if (stroka.Length >= 2)
            //        {
            //            if (stroka.ToString().StartsWith("OK"))
            //            {

            //                Console.WriteLine("OKey");
            //                Console.WriteLine("ИИ определил что вы написали oK");
            //                Console.WriteLine("Если это та команда что  вы хотели жмите Enter");
            //                ConsoleKeyInfo key1 = Console.ReadKey(false);
            //                if (key1.Key == ConsoleKey.Enter)
            //                {
            //                    Commanda = "OKey";
            //                    break;
            //                }
            //            }


            //            break;
            //        }
            //    }
            //    Console.WriteLine($"выбран команда {Commanda}");
            //    Console.ReadLine();
            //}



        }

        /// <summary>
        /// Требуется создать консольный файловый менеджер начального уровня, который покрывает минимальный набор функционала по работе с файлами.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Cursor.
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Title = "FILE Manager " + Environment.Version.ToString();
            ErrorsLogFile();
            DrawInterface();

            Console.ReadKey();

        }

        private static void ErrorsLogFile()
        {
            var errorsLogFile = "erro.json";
            var DirError = Environment.CurrentDirectory + "/errors";
            var pathErr = DirError + "/" + errorsLogFile;
            try
            {
                if (!Directory.Exists(DirError))
                {
                    Directory.CreateDirectory(DirError);
                }
                if (!File.Exists(pathErr))
                {
                    File.Create(pathErr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        private static void DrawInterface()
        {
            while (true)
            {

                Console.Clear();

                //рисовка и заполнение
                Console_CursorPos(0, 0);
                Console.WriteLine(stringa);
                //тут  метод для показа файлов и папок
                DicAndFolders();
                Console.WriteLine("Информация о выбранном");
                Console_CursorPos(0, 17);
                Console.WriteLine(stringa);
                Informations();//по дефолту
                Console.WriteLine(stringa);
                Console.WriteLine("Ввведите нужную команду");
                Console_CursorPos(0, 22);//коор команды ввода

                InputCommand();
                Console.WriteLine();
                Console_CursorPos(0, 22);//коор команды ввода 4 строки достаточно
                Console_CursorPos(0, 26);
                Console.WriteLine(stringa);
                Console.WriteLine("Поддерживаемые команды");
                Console.WriteLine(stringa);
                Help.ConsoleHelp();
                Console.WriteLine(stringa);
                //сюда сдвиг курсора  для ввода 
                Console_CursorPos(3, 22);

                while (true)//ожидаем вводы от юзера
                {
                    if (!CommandsUser())
                    {
                        break;
                    }

                }
            }

        }

        private static bool CommandsUser()
        {
            var comm = Console.ReadLine();
            var Splitt = comm?.Split(' ');
            if (comm?.Length > 0 && Splitt?.Length > 0)
            {
                string res = ParseCommandLine(Splitt[0]);//опред только команду

                if (res == null)
                {
                    Console.WriteLine("Команда не найдена");
                    Console_CursorPos(0, 22);
                    Console.Write(">> ");
                    Console_CursorPos(3, 22);
                }
                else
                {
                    Console.WriteLine($"Выбрана работа с командой {res}");
                    if (Splitt.Length > 0)
                    {
                        bool IsExit = ParseArgumentLine(ref res, ref comm);
                        if (!IsExit)//выход из цикла если выбрана помощь на весь экран.. кто знает  может  там 1000 настроек  будет 
                        {
                            return false;
                        }
                        else
                        {
                            Console_CursorPos(0, 22);
                            Console.Write(">> ");
                            Console_CursorPos(3, 22);
                            return true; //перерисовываем
                        }
                    }
                    return true;
                }

            }
            return true;
        }

        private static bool ParseArgumentLine(ref string res, ref string comm)
        {

            (string PathInput, string PathOutput, int pag) = CheckDirandFile(ref res, ref comm);


            //collectionHelp.Add("cp", "Копирование каталога,пример C:\\Source D:\\Target или C:\\source.txt D:\\target.txt");
            //collectionHelp.Add("rm", "Удаление каталога рекурсивно/Файла, пример rm C:\\Source или rm C:\\source.txt");
            //collectionHelp.Add("file", "Вывод информации");
            //collectionHelp.Add("Help", "Все доступные команды,пример file C:\\source.txt");



            switch (res)
            {
                case "ls": return Directory_Output(ref PathInput, pag);
                case "cp": return CopyDirectoryOrFiles(ref PathInput, ref PathOutput); break;
                case "rm": DeleteDirectoryOrFile(ref PathInput); break;
                case "file": return OutFile(ref PathInput, ref comm); break;
                case "Help": HelpInfo(true); return false;
                default: Console.WriteLine("BagCommand"); break;
            }
            return true;


            //if (!collectionHelp.ContainsKey("ls"))
            //{
            //    collectionHelp.Add("ls", "Вывод дерева файловой системы с условием “пейджинга”\nПараметр -p, пример ls C:\\Source -p 2");
            //    collectionHelp.Add("cp", "Копирование каталога,пример C:\\Source D:\\Target или C:\\source.txt D:\\target.txt");
            //    collectionHelp.Add("rm", "Удаление каталога рекурсивно/Файла, пример rm C:\\Source или rm C:\\source.txt");
            //    collectionHelp.Add("file", "Вывод информации,пример file C:\\source.txt"");
            //    collectionHelp.Add("Help", "Все доступные команды);
            //}
        }

        private static void HelpInfo(bool flag = false)
        {
            Console.Clear();
            Help.ConsoleHelp();
            if (flag)
            {
                Console.WriteLine("Для закрытие помощи,нажмите любую клавищу");
                Console.ReadKey(true);
            }
        }

        private static bool OutFile(ref string pathInput, ref string comm)
        {
            if (pathInput == null)
            {
                var pathN = comm.Replace("file ", "", StringComparison.OrdinalIgnoreCase);
                if (!Path.HasExtension(pathN))// папка 
                {
                    Informations(ref pathN, true);//true -РАБ  С ПАПКОЙ
                }
                else
                {
                    Informations(ref pathN, false);
                }

            }
            var result = Path.HasExtension(pathInput);
            if (!result && pathInput != null) // папка 
            {
                Informations(ref pathInput, true);//true -РАБ  С ПАПКОЙ
            }
            else if (pathInput != null)//файл 
            {
                Informations(ref pathInput, false);
            }
            return true;//означ.  что не пер. интрерфейс
        }

        private static void Informations(ref string pathInput, bool v)
        {
            if (v)//work folder
            {
                GetDirectoryInfo(ref pathInput);
                return;
            }
            GetFileInfo(ref pathInput);

        }

        private static void DeleteDirectoryOrFile(ref string pathInput)
        {
            var result = Path.HasExtension(pathInput);
            if (!result && pathInput != null) // папка 
            {
                if (Directory.Exists(pathInput))
                {
                    DeleteDirectory(pathInput);
                }
                else
                {
                    Console.Write($"{pathInput} не существует Нажмите любую клавишу");
                    Console.ReadKey();
                }
            }
            else if (pathInput != null)//файл
            {
                if (File.Exists(pathInput))
                {
                    DeleteFile(pathInput);
                }

            }
            Console.Write($"Null");





        }




        private static bool CopyDirectoryOrFiles(ref string pathInput, ref string pathOutput)
        {
            var result = Path.HasExtension(pathInput);
            if (!result) // папка 
            {
                if (!Directory.Exists(pathOutput))
                {
                    Directory.CreateDirectory(pathOutput);
                }
                DirectoryInfo dir = new DirectoryInfo(pathInput);
                DirectoryInfo[] dirs = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string tempPath = Path.Combine(pathInput, file.Name);
                    try
                    {
                        file.CopyTo(tempPath, false);
                    }
                    catch (Exception e)
                    {
                        Console.Write($"Ошибка при копировании файла {file.Name} (Нажмите любую клавишу)");
                        string rootPath = Directory.GetCurrentDirectory();
                        //if (File.Exists(Path.Combine(rootPath, errorsLogFile)))
                        //{
                        //    var jsonString = JsonSerializer.Serialize(e.Message);
                        //    try
                        //    {
                        //        File.WriteAllText(Path.Combine(rootPath, errorsLogFile), jsonString);
                        //    }
                        //    catch
                        //    {
                        //        Console.Write($"Ошибка записи в файл {errorsLogFile}");
                        //    }
                        //}
                        Console.ReadKey();
                    }
                }
            }
            else//file
            {
                File_Copy(pathInput, pathOutput);


            }



            result = Path.HasExtension(pathInput);
            return false;
            if (pathInput == null || Directory.Exists(pathInput))//еще одна провера папки, вдруг юзер удалил ее =)
            {
                return false;
            }
            //if (!Directory.Exists(pathOutput))
            //{
            //    Directory.CreateDirectory(pathOutput);

            //    pathTo = userCommands[2];
            //    if (!Directory.Exists(pathTo))
            //    {
            //        CopyDirectory(pathFrom, pathTo);
            //    }
            //    else
            //    {
            //        StandAtCommandLine();
            //        Console.Write($"{pathTo} уже существует (Нажмите любую клавишу)");
            //        Console.ReadKey();
            //        break;
            //    }
            //}
            //else if (File.Exists(pathFrom) && userCommands.Count == 3)
            //{
            //    pathTo = userCommands[2];
            //    if (!File.Exists(pathTo))
            //    {
            //        CopyFile(pathFrom, pathTo);
            //    }
            //    else
            //    {
            //        StandAtCommandLine();
            //        Console.Write($"{pathTo} уже существует (Нажмите любую клавишу)");
            //        Console.ReadKey();
            //        break;
            //    }
            //}
            //else
            //{
            //    StandAtCommandLine();
            //    Console.Write($"{pathFrom} не существует (Нажмите любую клавишу)");
            //    Console.ReadKey();
            //}
        }

        private static void File_Copy(string pathInput, string pathOutput)
        {
            try
            {
                if (File.Exists(pathInput))
                {
                    File.Copy(pathInput, pathOutput);
                    Console.Write("Копирование успешно");
                }
            }
            catch (Exception e)
            {
                Console.Write($"При копировании произошла ошибка");
                string rootPath = Directory.GetCurrentDirectory();
                //if (File.Exists(Path.Combine(rootPath, errorsLogFile)))
                //{
                //    var jsonString = JsonSerializer.Serialize(e.Message);
                //    try
                //    {
                //        File.WriteAllText(Path.Combine(rootPath, errorsLogFile), jsonString);
                //    }
                //    catch
                //    {
                //        Console.Write($"Ошибка записи в файл {errorsLogFile}");
                //    }
                //}
                Console.ReadKey();
            }
        }

        static string SelectFolder = @"C:/";
        static int Paging = 0;
        private static bool Directory_Output(ref string selectFolder, int pagg)
        {
            if (selectFolder != null && Directory.Exists(selectFolder))//еще одна провера папки, вдруг юзер удалил ее =)
            {
                SelectFolder = selectFolder;
                Paging = pagg > -1 && pagg < int.MaxValue ? pagg : 0;
                //выводим инфу постранично
                return true;
            }
            return false;
        }

        private static (string, string, int) CheckDirandFile(ref string res, ref string comm)
        {
            //универ.метод определения есть ли папка или файл простая проверка перед работой позволит  понять , были ли пробелы в названиях папки
            string puthInput = null;

            string puthOup = null;
            int pag = 0;
            string newSting = null;
            bool ListorAll = res == "ls";
            //два вида регулярки в продакшен их можно поместить в статический класс и скопилировать сразу,
            //тут это делать не буду , чтобы была видна логика 
            string pattern = null;
            if (ListorAll)
            {
                pattern = @".:\\+[#'.А-Яа-яA-Za-z0-9\\ ]* -p[ 0-9]{0,9999}";
            }
            else
            {
                pattern = @".:\\+['.А-Яа-яA-Za-z0-9\\ ]* .:\\+['.А-Яа-яA-Za-z0-9\\ ]*";
            }
            var regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var mathess = regex.Matches(comm);
            if (ListorAll && mathess != null && mathess.Count == 0)//user has not entered pagination 
            {
                pattern = @".:\\+['.А-Яа-яA-Za-z0-9\\ ]*";
                regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                mathess = regex.Matches(comm);
                foreach (Match item in mathess)
                {
                    var type = item.Value;
                    if (Directory.Exists(type))// пример ls C:\\Source -p 2"
                    {
                        puthInput = type;//она же дир  изспользуется не по назначению 
                        return (puthInput, puthOup, pag);
                    }
                }


            }
            foreach (Match mathes in mathess)
            {
                var type = mathes.Value;
                if (ListorAll)
                {
                    int num;
                    var SplitDirandPagin = type.Split(" -p");
                    int.TryParse(SplitDirandPagin[1], out num);
                    if (num != 0)
                    {
                        pag = num;
                    }
                    if (Directory.Exists(SplitDirandPagin[0]))// пример ls C:\\Source -p 2"
                    {
                        puthInput = SplitDirandPagin[0];//она же дир  изспользуется не по назначению 
                        break;
                    }
                }
                else
                {
                    var SplitDirandPagin = type.TrimEnd().Split(" ");
                    if (SplitDirandPagin.Count() == 2)//хороший вариант деления 2исходный файла не содержат  пробелы и запрещающ. символы
                    {
                        puthInput = SplitDirandPagin[0];
                        puthOup = SplitDirandPagin[1];
                    }
                    else
                    {//плохой вариант  имя файла содержит  пробелы,проходим по символьно ищем делитель 
                        StringBuilder stringBuilder = new StringBuilder();
                        var t = ""; bool Dobor = false; string tempSymbol = null;
                        for (int i = 0; i < type.Length; i++)
                        {
                            if (type[i] != ' ' && !Dobor)//
                            {
                                if (tempSymbol != null)
                                {
                                    stringBuilder.Append(tempSymbol);
                                    tempSymbol = null;
                                }
                                stringBuilder.Append(type[i]);
                            }
                            else
                            {//"D:\Users\\De
                                if (Dobor && type[i] != ':')
                                {
                                    stringBuilder.Append(type[i]);
                                }
                                else
                                {
                                    if (type[i] == ':')
                                    {
                                        var pt = stringBuilder.ToString();
                                        if (puthInput == null)
                                        {
                                            tempSymbol = pt.Substring(pt.Length - 1);
                                            puthInput = pt.Remove(pt.Length - 1);
                                            stringBuilder.Clear();
                                            Dobor = false;
                                            continue;
                                        }
                                        else if (puthOup == null)
                                        {
                                            tempSymbol = pt.Substring(pt.Length - 1);
                                            puthOup = pt.Remove(pt.Length - 1);
                                            stringBuilder.Clear();
                                            Dobor = false;
                                            continue;
                                        }
                                    }
                                    stringBuilder.Append(' ');//добиваем пробел 
                                    Dobor = true;
                                }
                                //запоминаем позицию  последнего пробела 
                            }
                        }
                        if (stringBuilder.Length > 0 && puthOup == null)//после таких манипяляций остаются остатки 
                        {
                            var pt = stringBuilder.ToString();
                            tempSymbol = pt.Substring(pt.Length - 1);
                            puthOup = pt.Remove(pt.Length - 1);
                            stringBuilder.Clear();
                            Dobor = false;
                            continue;

                        }
                    }

                }


            }
            return (puthInput, puthOup, pag);





        }

        /// <summary>
        /// Определяем тип команды поданной на вход
        /// </summary>
        /// <param name="comm"></param>
        /// <returns></returns>
        private static string ParseCommandLine(string comm)
        {
            foreach (var com in collectionHelp)
            {
                var minimalC = comm.ToLower();
                if (minimalC.StartsWith(com.Key.ToLower()))//ищем команду 
                {
                    return com.Key;
                }
            }
            return null;
        }

        private static void InputCommand()
        {
            Console.Write(">> ");
            //потом найти эту позицию  и поставить сюда курсор  для ввода мышки           
        }

        static bool GetInfo = false;
        private static void Informations()
        {

            var currentDirectory = Folder;
            GetDirectoryInfo(ref currentDirectory);
        }

        private static void Console_CursorPos(int v1, int v2)
        {
            Console.CursorLeft = v1;//по x строке
            Console.CursorTop = v2;//по y столбцу
        }

        private static void DicAndFolders()
        {
            var path = SelectFolder;//выбранная папка
            var PageMaxSetting = Paging; //выбранная страница
            var pageLines = 8;// кол-во выводимых файлов на страницу
            Console.Write("Select [");
            Console.WriteLine(path + "]");
            Console.WriteLine(stringa);
            //создаем словарь с индексом элемента, для того чтобы потом по нему перемещаться 
            Dictionary<int, string> directoriesOrFiles = new();
            int indexFile = 0;
            foreach (var item in Directory.GetDirectories(path))
            {
                directoriesOrFiles.Add(indexFile, item);
                indexFile++;
            }
            //тоже самое делаем с файлами
            foreach (var item in Directory.GetFiles(path))
            {
                directoriesOrFiles.Add(indexFile, item);
                indexFile++;
            }
            //провер. допустимое заданное число  выводимых файлов на экран
            if (directoriesOrFiles.Count() > 0)
            {
                Dictionary<int, List<KeyValuePair<int, string>>> Pagination = new();
                //распределения файлов по разным страницам 
                var temp = pageLines;//кол-во выводимы 
                int indexPathPagin = 0;
                int index = 0;
                foreach (var pathId in directoriesOrFiles)
                {
                    if (indexPathPagin < pageLines) //0<20
                    {
                        var t = KeyValuePair.Create(pathId.Key, pathId.Value);//Определяет пару "ключ-значение",                        
                        if (Pagination.ContainsKey(index))
                        {
                            Pagination[index].Add(t);
                        }
                        else
                        {
                            var t1 = KeyValuePair.Create(pathId.Key, pathId.Value);//Определяет пару "ключ-значение",
                            Pagination.Add(index, new List<KeyValuePair<int, string>>() { t1 });
                        }
                    }
                    else
                    {
                        index++;
                        var t = KeyValuePair.Create(pathId.Key, pathId.Value);//Определяет пару "ключ-значение",
                        Pagination.Add(index, new List<KeyValuePair<int, string>>() { t });
                        indexPathPagin = 0;
                    }
                    indexPathPagin++;
                }
                //выводим на экран нужную  заданную пагинацию человеком - p 5...
                if (Pagination.ContainsKey(Paging))
                {
                    var ViewModelFile = Pagination[Paging];

                    foreach (var item in ViewModelFile)
                    {
                        Console.WriteLine(item.Value);//выводим название файлов или папок 
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console_CursorPos(0, 14);
                    Console.WriteLine($"Page {Paging} of {Pagination.Count - 1}");
                    Console.WriteLine(stringa);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Пагинация не доступна");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }


            }


        }


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


        //вывод информации о каталоге
        static void GetDirectoryInfo(ref string path)
        {
            if (Directory.Exists(path))//еще раз  проверим вдруг удалена.. 
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                var directories = Directory.GetDirectories(path);
                var files = Directory.GetFiles(path);//делаем еще раз провер,  если вдруг  файл был удален в другом месте 
                Console_CursorPos(0, 17);
                Console.WriteLine($"Последний доступ к текущему : {directoryInfo.LastAccessTime} / ");
                Console.WriteLine($"Время последней операции записи: {directoryInfo.LastWriteTime}");
                Console.WriteLine($"Кол-во: {directories.Length} Folder and {files.Length} Files");
                GetInfo = true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Folder not Found");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;//возращаем цвет

        }
        static void GetFileInfo(ref string path)
        {
            if (File.Exists(path))//еще раз  проверим вдруг удалена.. 
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                FileInfo fileInfo = new FileInfo(path);
                Console.Write($"File: {path}");
                Console.Write($"Last Access: {fileInfo.LastAccessTime}");
                Console.Write($"Last Write: {fileInfo.LastWriteTime}");
                Console.Write($"Length: {fileInfo.Length} / ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("File not Found");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;//возращаем цвет

        }


        /// <summary>
        /// получение размера файла или каталога
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static long GetSize(string path)
        {
            if (Directory.Exists(path))
            {
                string[] subDirectories;
                subDirectories = Directory.GetDirectories(path);
                try
                {

                }
                catch (Exception e)
                {
                    //string rootPath = Directory.GetCurrentDirectory();
                    //if (File.Exists(Path.Combine(rootPath, set.errorsLogFile)))
                    //{
                    //    var jsonString = JsonSerializer.Serialize(e.Message);
                    //    try
                    //    {
                    //        File.WriteAllText(Path.Combine(rootPath, set.errorsLogFile), jsonString);
                    //    }
                    //    catch
                    //    {
                    //        Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
                    //    }
                    //}
                    //return 0;
                }

                var subFiles = Directory.GetFiles(path);
                long size = 0;

                if (subDirectories.Length != 0)
                {
                    foreach (var file in subFiles)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                    foreach (var directory in subDirectories)
                    {
                        size += GetSize(directory);
                    }
                }
                else
                {
                    foreach (var file in subFiles)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                }
                return size;
            }
            else if (File.Exists(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Length;
            }
            else
            {
                return 0;
            }

        }


        //вывод файлов



        //вывод каталогов

        //удаление каталога
        static void DeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);

                Console.Write("Удаление успешно");
            }
            catch (Exception e)
            {
                Console.Write($"Ошибка при удалении каталога: {path}");
                string rootPath = Directory.GetCurrentDirectory();
                //if (File.Exists(Path.Combine(rootPath, set.errorsLogFile)))
                //{
                //    var jsonString = JsonSerializer.Serialize(e.Message);
                //    try
                //    {
                //        File.WriteAllText(Path.Combine(rootPath, set.errorsLogFile), jsonString);
                //    }
                //    catch
                //    {
                //        Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
                //    }
                //}
                Console.ReadKey();
            }
        }


        //удаление файла
        static void DeleteFile(string path)
        {
            var tempName = new FileInfo(path).Name;
            try
            {

                File.Delete(path);
                Console.Write($"[{tempName}] Удален");
            }
            catch (Exception e)
            {
                Console.Write($"Ошибка при удалении файла: {tempName}");
                //if (File.Exists(Path.Combine(rootPath, set.errorsLogFile)))
                //{
                //    var jsonString = JsonSerializer.Serialize(e.Message);
                //    try
                //    {
                //        File.WriteAllText(Path.Combine(rootPath, set.errorsLogFile), jsonString);
                //    }
                //    catch
                //    {
                //        Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
                //    }
                //}
                Console.ReadKey();
            }
        }


        //копирование директории
        static void CopyDirectory(string pathFrom, string pathTo)
        {
            DirectoryInfo dir = new DirectoryInfo(pathFrom);
            DirectoryInfo[] dirs = dir.GetDirectories();
            FileInfo[] files = dir.GetFiles();
            Directory.CreateDirectory(pathTo);
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(pathTo, file.Name);
                try
                {
                    file.CopyTo(tempPath, false);
                }
                catch (Exception e)
                {
                    //StandAtCommandLine();
                    Console.Write($"Ошибка при копировании файла {file.Name} (Нажмите любую клавишу)");
                    string rootPath = Directory.GetCurrentDirectory();
                    //if (File.Exists(Path.Combine(rootPath, set.errorsLogFile)))
                    //{
                    //    var jsonString = JsonSerializer.Serialize(e.Message);
                    //    try
                    //    {
                    //        File.WriteAllText(Path.Combine(rootPath, set.errorsLogFile), jsonString);
                    //    }
                    //    catch
                    //    {
                    //        Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
                    //    }
                    //}
                    Console.ReadKey();
                }
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(pathTo, subdir.Name);
                try
                {
                    CopyDirectory(subdir.FullName, tempPath);
                }
                catch (Exception e)
                {
                    //StandAtCommandLine();
                    Console.Write($"Ошибка при копировании директории {subdir.FullName} (Нажмите любую клавишу)");
                    string rootPath = Directory.GetCurrentDirectory();
                    //if (File.Exists(Path.Combine(rootPath, set.errorsLogFile)))
                    //{
                    //    var jsonString = JsonSerializer.Serialize(e.Message);
                    //    try
                    //    {
                    //        File.WriteAllText(Path.Combine(rootPath, set.errorsLogFile), jsonString);
                    //    }
                    //    catch
                    //    {
                    //        Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
                    //    }
                    //}
                    Console.ReadKey();
                }
            }
        }


        //копирование файла
        static void CopyFile(string pathFrom, string pathTo)
        {
            try
            {
                File.Copy(pathFrom, pathTo);
                //StandAtCommandLine();
                Console.Write("Копирование успешно");
            }
            catch (Exception e)
            {
                // StandAtCommandLine();
                Console.Write($"При копировании произошла ошибка");
                string rootPath = Directory.GetCurrentDirectory();
                //if (File.Exists(Path.Combine(rootPath, set.errorsLogFile)))
                //{
                //    var jsonString = JsonSerializer.Serialize(e.Message);
                //    try
                //    {
                //        File.WriteAllText(Path.Combine(rootPath, set.errorsLogFile), jsonString);
                //    }
                //    catch
                //    {
                //        Console.Write($"Ошибка записи в файл {set.errorsLogFile}");
                //    }
                //}
                Console.ReadKey();
            }
        }


        //вывод помощи

        //разделение строки на подстроки-команды


    }
}
