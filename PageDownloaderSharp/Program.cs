using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;

namespace PageDownloaderSharp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //История
            //https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873

            //var testUrl = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=818378&cmid=54999";

            //Console.WriteLine(sdo.GetPage("https://sdo.srspu.ru/mod/quiz/review.php?attempt=515662&cmid=19917").QuerySelector("body").TextContent);
            if(!File.Exists("index.html"))
            {
                var testUrl = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=818378&cmid=54999";
                SdoControl sdo = new SdoControl();
                sdo.SavePage(testUrl);
            }

            //TasksRepository tasksRepository = new TasksRepository("data.xls");
            HtmlParser parser = new HtmlParser(); 


            PageModel pageModel = new PageModel(parser.ParseDocument(File.ReadAllText("index.html")));
            pageModel.printAllModelsList();
            //tasksRepository.Print();
        }
    }
}
