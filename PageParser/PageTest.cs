using AngleSharp.Html.Parser;
using System;
using System.IO;

namespace PageParser
{
    public class PageTest
    {
        private AngleSharp.Html.Dom.IHtmlDocument document;
        public string Author { get; private set; }
        public string Score { get; private set; }
        private bool setAuthor()
        {
            var authorName = document.QuerySelector("div#user-picture div").TextContent;
            Author = authorName;
            return authorName != "";

        }
        private bool setScore()
        {
            var score = document.QuerySelector("tr td.cell b").TextContent;
            Score = score;
            return score != "";
        }
        private bool parserInit(string documentName)
        {
            var parser = new HtmlParser();
            var html = File.ReadAllText(documentName);
            document = parser.ParseDocument(html);
            return document.TextContent != "";
        }
        public PageTest(string documentName)
        {
            var complete = parserInit(documentName);
            if (complete)
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
                Console.WriteLine("Имя автора теста:" + Author);
            }
            else
            {
                string errorMessage = "Автор теста не найден";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
            complete = setScore();
            if (complete)
            {
                Console.WriteLine("Оценка за тест: " + Score);
            }
            else
            {
                string errorMessage = "Оценка за тест не найдена";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
        }
    }
}
