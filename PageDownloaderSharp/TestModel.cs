using System;
using System.Collections.Generic;
using System.Linq;

namespace PageDownloaderSharp
{
    enum QuestionType
    {
        Input,
        Radio,//Радиокнопки
        Checkbox,//
        Select,//Менюшки <select>
        None
    }

    /// <summary>
    /// Ответ на вопрос
    /// </summary>
    class Answer
    {
        public bool IsCorrect { get; set; }
        public string Text { get; set; }
        public Answer(bool isCorrect, string text)
        {
            IsCorrect = isCorrect;
            Text = text;
        }
    }

    class Question
    {
        QuestionType questionType;
        public string Text { get; private set; }

        private List<Answer> answers;//Ответы на вопрос

        public Question(QuestionType questionType, string text)
        {
            answers = new List<Answer>();
            this.questionType = questionType;
            Text = text;
        }
        public void AddAnswer(Answer answer)
        {
            answers.Add(answer);
        }
        public void Print()
        {
            Console.WriteLine("Тип вопроса: " + questionType.ToString());
            Console.WriteLine("Текст вопроса: " + Text);
            if(answers.Count == 0)
            {
                Console.WriteLine("Ответов пока нет");
            }
            else
            {
                foreach (var item in answers)
                {
                    if (item.IsCorrect)
                    {
                        Console.WriteLine(item.Text + " + ");
                    }
                    else
                    {
                        Console.WriteLine(item.Text + " - ");
                    }
                }
            }
        }
        
    }

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
                    //Console.WriteLine($"{index}. {grage} радиокнопок:{radios.Length}");
                    var questionLocal = new Question(QuestionType.Radio, questionText);
                    var spans = question.QuerySelectorAll("span.answernumber");
                    foreach (var item in spans)
                    {
                        bool correct = item.ParentElement.ParentElement.QuerySelector("input[checked=checked]") != null;
                        questionLocal.AddAnswer(new Answer(correct, 
                            item.ParentElement.TextContent));
                        
                    }
                    

                    
                    questions.Add(questionLocal);
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
