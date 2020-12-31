using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageParser
{
    public class TestModel
    {
        private AngleSharp.Html.Dom.IHtmlDocument document;
        public string Author { get; private set; }



        private bool setAuthor()
        {
            var authorName = document.QuerySelector("div#user-picture div").TextContent;
            Author = authorName;
            return authorName != "";
            
        }

        private bool parserInit(string documentName)
        {
            var parser = new HtmlParser();
            var html = File.ReadAllText(documentName);
            document = parser.ParseDocument(html);
            return document.TextContent != "";
        }
        public TestModel(string documentName)
        {
            var complete = parserInit(documentName);
            if(complete)
            {
                Console.WriteLine("Парсер успешно инициализрован");
            }
            else
            {
                string errorMessage = "Парсер не удалось проинициализировать";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
            complete = setAuthor();
            if (complete)
            {
                Console.WriteLine("Имя автора успешно установлена");
            }
            else
            {
                string errorMessage = "Не удалось найти автора теста";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
        }
    }
}
