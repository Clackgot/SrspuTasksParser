using System;
using System.Collections.Generic;
using System.Linq;

namespace PageDownloaderSharp
{




    class TestModel
    {
        private AngleSharp.Html.Dom.IHtmlDocument document;
        public string Author { get; set; }
        public string Mark { get; set; }

        private List<Question> questions;
        public TestModel(AngleSharp.Html.Dom.IHtmlDocument htmlDocument)
        {
            document = htmlDocument;
            questions = new List<Question>();

            setAuthor();
            setMark();
            setQuestions();
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

        private void setQuestions()
        {
            int index = 1;
            foreach (var question in document.QuerySelectorAll("div[id^=question-]"))
            {
                
                var grage = question.QuerySelector("div.grade").TextContent;
                bool isCorrectAnswer = (grage == "Баллов: 1 из 1");
                var questionText = question.QuerySelector("div.qtext").TextContent;
                var checkboxes = question.QuerySelectorAll("input[type=checkbox][name*=choice]");
                var radios = question.QuerySelectorAll("input[type=radio]");
                var selects = question.QuerySelectorAll("select.custom-select");
                var inputs = question.QuerySelectorAll("input[id*=answer][type=text]");
                
                if(checkboxes.Length != 0)
                {
                    //Console.WriteLine($"{index}. {grage} чекбоксов:{checkboxes.Length}");
                }
                else if (radios.Length != 0)
                {
                    var questionLocal = new Question(QuestionType.Radio, questionText);
                    var spans = question.QuerySelectorAll("span.answernumber");
                    if (isCorrectAnswer)
                    {
                        foreach (var item in spans)
                        {
                            bool isChecked = item.ParentElement.ParentElement.QuerySelector("input[checked=checked]") != null;
                            if(isChecked)questionLocal.Answers.Add(new Answer(AnswerСondition.Correct,
                                item.ParentElement.TextContent));
                            if (!isChecked) questionLocal.Answers.Add(new Answer(AnswerСondition.Incorrect,
                                item.ParentElement.TextContent));
                        }
                        questions.Add(questionLocal);
                    }
                    else
                    {
                        foreach (var item in spans)
                        {
                            bool isChecked = item.ParentElement.ParentElement.QuerySelector("input[checked=checked]") != null;
                            if(isChecked) questionLocal.Answers.Add(new Answer(AnswerСondition.Incorrect,
                                item.ParentElement.TextContent));
                            if (!isChecked) questionLocal.Answers.Add(new Answer(AnswerСondition.None,
                                item.ParentElement.TextContent));
                        }
                        questions.Add(questionLocal);
                    }
                    
                }
                else if (selects.Length != 0)
                {
                    //Console.WriteLine($"{index}. {grage} селектеров:{selects.Length}");
                }
                else if (inputs.Length != 0)
                {
                    //Console.WriteLine($"{index}. {grage} инпутов:{inputs.Length}");
                }


                index++;
            }
        }


        private void printQuestions()
        {
            foreach (var item in questions)
            {
                item.Print();
                Console.WriteLine();
            }
        }
        public void Print()
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine(document.Title);
            Console.WriteLine("Автор: " + Author);
            Console.WriteLine("Оценка: " + Mark + "/100");
            Console.WriteLine("-----------------------");
            printQuestions();
        }
    }
}
