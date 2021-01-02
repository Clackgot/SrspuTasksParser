using System;

namespace PageDownloaderSharp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string login, password, testUrl;
            Console.Write("Логин:");
            login = Console.ReadLine();
            Console.Write("Пароль:");
            password = Console.ReadLine();
            Console.Write("Ссылка на тест:");
            testUrl = Console.ReadLine();
            SdoControl sdo = new SdoControl(login, password);
            var testModel = new TestModel(sdo.GetPage(testUrl));
            
            Console.ReadKey();
        }
    }
}
