using System;

namespace PageDownloaderSharp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //История
            //https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873

            var testUrl = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=818378&cmid=54999";
            SdoControl sdo = new SdoControl();
            var testModel = new TestModel(sdo.GetPage(testUrl));
            //testModel.Print();

            Console.ReadKey();
        }
    }
}
