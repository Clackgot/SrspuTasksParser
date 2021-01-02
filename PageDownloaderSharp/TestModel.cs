using System;
using System.Linq;

namespace PageDownloaderSharp
{
    class TestModel
    {
        private AngleSharp.Html.Dom.IHtmlDocument document;
        public string Author { get; set; }
        public string Mark { get; set; }
        public TestModel(AngleSharp.Html.Dom.IHtmlDocument htmlDocument)
        {
            document = htmlDocument;

            setAuthor();
            setMark();
            Print();
        }

        private void setAuthor()
        {
            Author = document.QuerySelector("div#user-picture div").TextContent;
        }
        private void setMark()
        {
            var element = document.All.First(o => o.TextContent == "Оценка" && o.TagName == "TH"); // If text, assign id.
            if(element != null)
            {
                Mark = element.ParentElement.QuerySelector("td b").TextContent;
            }
            else
            {
                Console.WriteLine("Оценка не найдена");
            }
            
        }

        public void Print()
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine(document.Title);
            Console.WriteLine("Автор: " + Author);
            Console.WriteLine("Оценка: " + Mark + "/100");
        }
    }
}
