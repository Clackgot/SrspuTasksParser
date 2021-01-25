using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleSheets;
using DecLoader;


namespace General
{

    public class Storage
    {
        public string spreadsheatId = "1bJFruzClSZcpuoUWXxOfJGJQ-l3Dt0DWXA4GHJDrgDw";//ID таблицы
        GoogleSheetsControl control; //Получение и сохранение таблиц Google Sheets
        private Loader loader;
        List<(string, string)> loginsAndParsswords;

        private void initDecLoader(List<(string, string)> loginsAndParsswords)
        {

            List<User> users = new List<User>();
            foreach (var item in loginsAndParsswords)
            {
                users.Add(new User(item.Item1, item.Item2));
            }
            loader = new Loader(users);
        }

        private async Task sendDiscipline(Discipline discipline)
        {
            await control.SendData(spreadsheatId, discipline, discipline.Name);
        }

        public Storage(List<(string, string)> loginsAndParsswords)
        {

            control = new GoogleSheetsControl("SrspuDecParser");

            initDecLoader(loginsAndParsswords);
            List<Task> tasks = new List<Task>();
            foreach (var discipline in loader.disciplines)
            {
                tasks.Add(sendDiscipline(discipline));
            }
            Task.WaitAll(tasks.ToArray());
        }
    }


    class Program
    {
        public static void Run()
        {
            Console.WriteLine("В папке с программой необходимо иметь файл passwords.txt с логинами и паролями от ved.srspu.ru");
            Console.WriteLine("В таком формате:");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("obama@yandex.ru parolObamy228");
            Console.WriteLine("putin@gmail.com pass2123123");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("Для продолжения нажмите любую клавишу");
            Console.ReadKey();
            string[] lines;
            try
            {
                lines = File.ReadAllLines("passwords.txt");
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка при попытке загрузки файла с паролями.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadKey();
                return;
            }
            List<(string, string)> passwords = new List<(string, string)>();
            try
            {
                foreach (var line in lines)
                {
                    passwords.Add((line.Split(' ')[0], line.Split(' ')[1]));
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Ошибка при чтении файла с паролями passwords.txt");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("В файле должно быть пустых строк");
            }


            Storage storage = new Storage(passwords);
        }
        static void Main(string[] args)
        {
            Run();
        }
    }
}
