using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PageDownloaderSharp
{
    /// <summary>
    /// Состояние ответа
    /// <list type="table">
    /// <item>
    /// <term>Checked</term>
    /// <description>Пункт выбран</description>
    /// </item>
    /// <item>
    /// <term>Unchecked</term>
    /// <description>Пункт не выбран</description>
    /// </item>
    /// <item>
    /// <term>None</term>
    /// <description>Не определено</description>
    /// </item>
    /// </list>
    /// </summary>
    enum AnswerCondition
    {
        Checked,
        Unchecked,
        None
    }
    /// <summary>
    /// Состояние задания
    /// <list type="table">
    /// <item>
    /// <term>Correct</term>
    /// <description>Задание решено верно</description>
    /// </item>
    /// <item>
    /// <term>Incorrect</term>
    /// <description>Задание решено не верно</description>
    /// </item>
    /// <item>
    /// <term>None</term>
    /// <description>Результат решения задания не известен</description>
    /// </item>
    /// </list>
    /// </summary>
    enum QuestionCondition
    {
        Correct,
        Incorrect,
        None
    }
    /// <summary>
    /// Тип задания
    /// <list type="table">
    /// <item>
    /// <term>Radio</term>
    /// <description>Задание с одним вариантом ответа</description>
    /// </item>
    /// <item>
    /// <term>Input</term>
    /// <description>Задание с полем для ввода</description>
    /// </item>
    /// <item>
    /// <term>Checkbox</term>
    /// <description>Задание с несколькими вариантами ответа</description>
    /// </item>
    /// <item>
    /// <term>Select</term>
    /// <description>Задание с несколькими вариантами ответа в каждом пункте</description>
    /// </item>
    /// <item>
    /// <term>Image</term>
    /// <description>Задание с изображением</description>
    /// </item>
    /// <item>
    /// <term>None</term>
    /// <description>Тип задания не определен</description>
    /// </item>
    /// </list>
    /// </summary>
    enum QuestionType
    { 
        Radio,
        Input,
        Checkbox,
        Select,
        Image,
        None
    }

    /// <summary>
    /// Абстрактный класс модели ответа на задание
    /// </summary>
    abstract class AnswerModel
    {
        public string Text { get; set; }
        /// <summary>
        /// Состояние ответа
        /// </summary>
        public AnswerCondition Condition { get; set; }

        abstract public void Print();
    }
    /// <summary>
    /// Модель ответа на задание с радиокнопками
    /// </summary>
    class RadioAnswerModel : AnswerModel
    {
        public override void Print()
        {
            switch (Condition)
            {
                case AnswerCondition.Checked:
                    Console.WriteLine($"(*){Text}");
                    break;
                case AnswerCondition.Unchecked:
                    Console.WriteLine($"( ){Text}");
                    break;
                case AnswerCondition.None:
                    Console.WriteLine($"(None){Text}");
                    break;
            }
        }
        public RadioAnswerModel(string text, AnswerCondition condition)
        {
            Text = text;
            Condition = condition;
        }
    }
    /// <summary>
    /// Модель ответа на задание с чекбоксами
    /// </summary>
    class CheckboxAnswerModel : AnswerModel
    {
        public override void Print()
        {
            switch (Condition)
            {
                case AnswerCondition.Checked:
                    Console.WriteLine($"[v]{Text}");
                    break;
                case AnswerCondition.Unchecked:
                    Console.WriteLine($"[ ]{Text}");
                    break;
                case AnswerCondition.None:
                    Console.WriteLine($"[None]{Text}");
                    break;
            }
        }
        public CheckboxAnswerModel(string text, AnswerCondition condition)
        {
            Text = text;
            Condition = condition;
        }
    }
    /// <summary>
    /// Модель ответа на задание с выпадающими меню
    /// </summary>
    class SelectAnswerModel : AnswerModel
    {
        public override void  Print()
        {
            switch (Condition)
            {
                case AnswerCondition.Checked:
                    Console.WriteLine($"{Text} [ v {TextOption}]");
                    break;
                case AnswerCondition.Unchecked:
                    Console.WriteLine($"{Text} [ v Не выбрано]");
                    break;
                case AnswerCondition.None:
                    Console.WriteLine($"{Text} [None]");
                    break;
            }
        }
        public string TextOption { get; set; }
        public SelectAnswerModel(string text, string textOption, AnswerCondition condition)
        {
            Text = text;
            TextOption = textOption;
            Condition = condition;
        }
    }
    /// <summary>
    /// Модель ответа в виде строки
    /// </summary>
    class InputAnswerModel : AnswerModel
    {
        public override void Print()
        {
            switch (Condition)
            {
                case AnswerCondition.Checked:
                    Console.WriteLine($"[{Text}]");
                    break;
                case AnswerCondition.Unchecked:
                    Console.WriteLine($"[Пусто]");
                    break;
                case AnswerCondition.None:
                    Console.WriteLine($"[None]");
                    break;
            }
        }
        public InputAnswerModel(string text, AnswerCondition condition)
        {
            Text = text;
            Condition = condition;
        }
    }
    /// <summary>
    /// Шаблонная модель задания с типом задания T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class QuestionModel<T> where T: AnswerModel
    {
        /// <summary>
        /// Текст задания
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// Тип задания
        /// </summary>
        public QuestionType Type { get; set; }
        /// <summary>
        /// Состояние задания
        /// </summary>
        public QuestionCondition Condition { get; set; }

        /// <summary>
        /// Список ответов на задание
        /// </summary>
        public List<T> Answers { get; set; } = new List<T>();

        public void Print()
        {
            Console.WriteLine(Text);
            foreach (var item in Answers)
            {
                item.Print();
            }
        }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="text">Текст задания</param>
        /// <param name="type">Тип задания</param>
        /// <param name="condition">Состояние задания</param>
        public QuestionModel(string text, QuestionType type, QuestionCondition condition)
        {
            Text = text;
            Type = type;
            Condition = condition;
        }
    }
    /// <summary>
    /// Модель страницы с тестом
    /// </summary>
    class PageModel
    {
        /// <summary>
        /// Список всех радио-заданий теста
        /// </summary>
        public List<QuestionModel<RadioAnswerModel>> radioQuestions = new List<QuestionModel<RadioAnswerModel>>();
        public List<QuestionModel<CheckboxAnswerModel>> checkboxQuestions = new List<QuestionModel<CheckboxAnswerModel>>();
        public List<QuestionModel<InputAnswerModel>> inputQuestions = new List<QuestionModel<InputAnswerModel>>();
        public List<QuestionModel<SelectAnswerModel>> selectQuestions = new List<QuestionModel<SelectAnswerModel>>();

        private bool isTestFinished()
        {
            return object.ReferenceEquals(document.QuerySelector("a.endtestlink"), null);
        }

        public void Print()
        {
            PageModelLogger logger = new PageModelLogger(this);
            logger.Print();
            int questionNumber = 0;
            foreach (var item in radioQuestions)
            {
                string header = String.Format("------------------------------------------Radio#{0:d3}----------------------------------------------", questionNumber);
                Console.WriteLine(header);
                item.Print();
                Console.WriteLine();
                questionNumber++;
            }
            questionNumber = 0;
            foreach (var item in checkboxQuestions)
            {
                string header = String.Format("---------------------------------------Checkbox#{0:d3}----------------------------------------------", questionNumber);
                Console.WriteLine(header);
                item.Print();
                Console.WriteLine();
                questionNumber++;
            }
            questionNumber = 0;
            foreach (var item in inputQuestions)
            {
                string header = String.Format("------------------------------------------Input#{0:d3}----------------------------------------------", questionNumber);
                Console.WriteLine(header);
                item.Print();
                Console.WriteLine();
                questionNumber++;
            }
            questionNumber = 0;
            foreach (var item in selectQuestions)
            {
                string header = String.Format("-----------------------------------------Select#{0:d3}----------------------------------------------", questionNumber);
                Console.WriteLine(header);
                item.Print();
                Console.WriteLine();
                questionNumber++;
            }
        }

        /// <summary>
        /// HTML-содержимое страницы с тестом
        /// </summary>
        private IHtmlDocument document;

        /// <summary>
        /// Возвращает тип задания
        /// </summary>
        /// <param name="element">Элемент внутри которого нужно определить наличие и тип задания</param>
        /// <returns></returns>
        private QuestionType getQuestionType(IElement element)
        {
            if (element.ParentElement.QuerySelector("input[type=radio]") != null) return QuestionType.Radio;
            if (element.ParentElement.QuerySelector("input[type=checkbox]") != null) return QuestionType.Checkbox;
            if (element.ParentElement.QuerySelector("input[type=text]") != null) return QuestionType.Input;
            if (element.ParentElement.QuerySelector("select") != null) return QuestionType.Select;
            if (element.ParentElement.QuerySelector("img") != null) return QuestionType.Image;
            return QuestionType.None;
        }

        /// <summary>
        /// Название предмета
        /// </summary>
        public string Discipline { get; private set; }
        /// <summary>
        /// Название теста
        /// </summary>
        public string TestName { get; private set; }
        /// <summary>
        /// Количество баллов за тест
        /// </summary>
        public int Mark { get; private set; }
        /// <summary>
        /// Количество заданий в тесте
        /// </summary>
        public int QuestionCount { get; private set; } = 0;
        /// <summary>
        /// Инициализация основных сведений о тесте
        /// </summary>
        private void initGeneralInfo()
        {
            if (!isTestFinished()) throw new Exception("Тест ещё не закончен");
            setDescipline();
            setTestName();
            setMark();
            setQuestionCount();
            setQuestions();
        }
        /// <summary>
        /// Устанавиливает количество заданий в тесте
        /// </summary>
        private void setQuestionCount()
        {
            QuestionCount = document.QuerySelectorAll("div[id^=question]").Length;
        }
        /// <summary>
        /// Устанавливает название теста
        /// </summary>
        private void setTestName()
        {
            TestName = document.QuerySelector("li.breadcrumb-item a[title=Тест]").TextContent;
        }
        /// <summary>
        /// Устанавливает название предмета
        /// </summary>
        private void setDescipline()
        {
            Discipline = document.QuerySelector("div.page-header-headings h1").TextContent;
        }
        /// <summary>
        /// Устанавливает количество баллов за тест
        /// </summary>
        private void setMark()
        {
            var element = document.QuerySelector("tr td.cell b");
            bool elementExsist = !object.ReferenceEquals(null, element);
            if (elementExsist)
            {
                Mark = int.Parse(element.TextContent);
            }
            else
            {
                Mark = -1;
            }
        }

        /// <summary>
        /// Заполняет список заданий
        /// </summary>
        private void setQuestions()
        {
            var answersDiv = document.QuerySelectorAll("div.qtext");//Блоки заданий
            //int questionNumber = 1;//Номер теста
            foreach (var answerDiv in answersDiv)
            {
                
                var questionText = answerDiv.TextContent.Replace("\n", " ");//Замена переноса строк на пробел
                questionText = Regex.Replace(questionText, @"([ ]){2,}", @" ");//Удаление повторяющихся пробелов
                //string header = String.Format("------------------------------------------------{0:d3}----------------------------------------------", questionNumber);
                //Console.WriteLine(header);
                switch (getQuestionType(answerDiv))
                {
                    case QuestionType.Radio:
                        var tempRadioQuestion = new QuestionModel<RadioAnswerModel>(questionText, QuestionType.Radio, QuestionCondition.None);
                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("label"))
                        {
                            var input = item.ParentElement.QuerySelector("input").GetAttribute("checked");
                            AnswerCondition answerCondition = AnswerCondition.None;
                            if (input == "checked") answerCondition = AnswerCondition.Checked;
                            else answerCondition = AnswerCondition.Unchecked;
                            item.RemoveElement(item.QuerySelector("span"));
                            tempRadioQuestion.Answers.Add(new RadioAnswerModel(item.TextContent, answerCondition));
                        }
                        radioQuestions.Add(tempRadioQuestion);
                        break;
                    case QuestionType.Input:
                        var tempInputQuestion = new QuestionModel<InputAnswerModel>(questionText, QuestionType.Input, QuestionCondition.None);
                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("input[id*=answer]"))
                        {
                            var input = item.ParentElement.QuerySelector("input").GetAttribute("value");
                            AnswerCondition answerCondition = AnswerCondition.None;
                            if (input.Length > 0 ) answerCondition = AnswerCondition.Checked;
                            else answerCondition = AnswerCondition.Unchecked;
                            tempInputQuestion.Answers.Add(new InputAnswerModel(item.GetAttribute("value"), answerCondition));
                        }
                        inputQuestions.Add(tempInputQuestion);
                        break;
                    case QuestionType.Checkbox:
                        var tempCheckboxQuestion = new QuestionModel<CheckboxAnswerModel>(questionText, QuestionType.Checkbox, QuestionCondition.None);
                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("label"))
                        {
                            var input = item.ParentElement.QuerySelector("input").GetAttribute("checked");
                            AnswerCondition answerCondition = AnswerCondition.None;
                            if (input == "checked") answerCondition = AnswerCondition.Checked;
                            else answerCondition = AnswerCondition.Unchecked;
                            item.RemoveElement(item.QuerySelector("span"));
                            tempCheckboxQuestion.Answers.Add(new CheckboxAnswerModel(item.TextContent, answerCondition));
                        }
                        checkboxQuestions.Add(tempCheckboxQuestion);
                        break;
                    case QuestionType.Select:
                        var tempSelectQuestion = new QuestionModel<SelectAnswerModel>(questionText, QuestionType.Select, QuestionCondition.None);

                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("tr[class*=r]"))
                        {
                            AnswerCondition answerCondition = AnswerCondition.None;
                            if (item.QuerySelector("option[selected=selected]").GetAttribute("value") == "0")
                            {
                                answerCondition = AnswerCondition.Unchecked;
                            }
                            else
                            {
                                answerCondition = AnswerCondition.Checked;
                            }
                            var text = item.QuerySelector("p").TextContent;
                            var optionText = item.QuerySelector("option[selected=selected]").TextContent;
                            tempSelectQuestion.Answers.Add(new SelectAnswerModel(text, optionText, answerCondition));
                        }
                        selectQuestions.Add(tempSelectQuestion);
                        break;
                    case QuestionType.None:
                        throw new Exception("Неизвестный тип задания");
                }
            }
        }
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="document">HTML-содержимое страницы теста</param>
        public PageModel(IHtmlDocument document)
        {
            this.document = document;
            initGeneralInfo();
        }
    }
    /// <summary>
    /// Логгер модели страницы
    /// </summary>
    class PageModelLogger
    {
        /// <summary>
        /// Модель страницы теста
        /// </summary>
        private readonly PageModel model;
        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="pageModel">Модель страницы теста</param>
        public PageModelLogger(PageModel pageModel)
        {
            model = pageModel;
        }
        /// <summary>
        /// Выводит информацию о тесте в консоль
        /// </summary>
        public void Print()
        {
            printDiscipline();
            printTestName();
            printMark();
            printQuestionCount();
        }

        /// <summary>
        /// Выводит количество баллов за тест
        /// </summary>
        private void printMark()
        {
            if (model.Mark > 0) Console.WriteLine($"Баллов: {model.Mark}/100");
            else Console.WriteLine($"Баллов: None");
        }
        /// <summary>
        /// Выводит количество заданий в тесте
        /// </summary>
        private void printQuestionCount()
        {
            if (model.QuestionCount > 0) Console.WriteLine($"Заданий: {model.QuestionCount}");
            else Console.WriteLine($"Заданий: 0");
        }
        /// <summary>
        /// Выводит название теста
        /// </summary>
        private void printTestName()
        {
            if (model.TestName.Length > 0 && model.TestName != null) Console.WriteLine($"Тест: {model.TestName}");
            else Console.WriteLine($"Тест: None");
        }
        /// <summary>
        /// Выводит название предмета
        /// </summary>
        private void printDiscipline()
        {
            if (model.Discipline.Length > 0 && model.Discipline != null) Console.WriteLine($"Предмет: {model.Discipline}");
            else Console.WriteLine($"Предмет: None");
        }
    }
}
