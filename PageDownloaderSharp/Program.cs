using AngleSharp.Html.Parser;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Xml.Serialization;

namespace PageDownloaderSharp
{
    //public class Config
    //{
    //    public string username;
    //    public string password;
    //    public string testLink;


    //    private static string fileName = "config.xml";

    //    public void Save()
    //    {
    //        XmlSerializer xs = new XmlSerializer(typeof(Config));
    //        using (StreamWriter sw = new StreamWriter(fileName))
    //            xs.Serialize(sw, this);
    //    }

    //    public static Config Load()
    //    {
    //        try
    //        {
    //            XmlSerializer xs = new XmlSerializer(typeof(Config));
    //            //using (StreamReader sr = new StreamReader(fileName))
    //            //return (Config)xs.Deserialize(sr);
    //            StreamReader sr = new StreamReader(fileName)
    //            sr.ReadToEnd();

    //        }
    //        catch
    //        {
    //            Config config = new Config
    //            {
    //                username = "clackgot@gmail.com",
    //                password = "uVJ3e3Uf",
    //                testLink = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873"
    //            };
    //            config.Save();
    //            return config;
    //        }
    //    }
    //}






    public class Config
    {
        public string username = "qwe";
        public string password = "qwe";
        public string testLink = "qwe";


        private static string fileName = "settings.json";

        private void Save()
        {
            File.WriteAllText(fileName, JsonConvert.SerializeObject(this));
        }

        public static Config Load()
        {
            try
            {
                var json = File.ReadAllText(fileName);
                dynamic jsonDe = JsonConvert.DeserializeObject(json);
                return (Config)jsonDe;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Config config = new Config
                {
                    username = "clackgot@gmail.com",
                    password = "uVJ3e3Uf",
                    testLink = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873"
                };
                config.Save();
                return config;
            }
        }
    }
    partial class Program
    {
        static void Main(string[] args)
        {

            //История
            //https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873

            //var testUrl = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=818378&cmid=54999";

            //Console.WriteLine(sdo.GetPage("https://sdo.srspu.ru/mod/quiz/review.php?attempt=515662&cmid=19917").QuerySelector("body").TextContent);


            Config config = new Config();
            config = Config.Load();
            Console.WriteLine(config.password);
            while (true)
            {
                Console.WriteLine("Обновить документ?(y/n)");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    var testUrl = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873";
                    SdoControl sdo = new SdoControl();
                    sdo.SavePage(testUrl);
                    Console.Clear();
                    break;
                }
                else if (key.Key == ConsoleKey.N)
                {
                    Console.Clear();
                    break;
                }
                Console.Clear();
            }

            //TasksRepository tasksRepository = new TasksRepository("data.xls");
            HtmlParser parser = new HtmlParser();

            try
            {
                PageModel pageModel = new PageModel(parser.ParseDocument(File.ReadAllText("index.html")));
                pageModel.Print();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}");
            }

        }
    }
}
