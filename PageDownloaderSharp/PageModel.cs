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
    }

    /// <summary>
    /// Модель ответа на задание с радиокнопками
    /// </summary>
    class RadioAnswerModel : AnswerModel
    {
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
    class QuestionModel<T>
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

        public void printQuestionTypes()
        {
            Console.WriteLine($"Radios: {radioQuestions.Count}");
            Console.WriteLine($"Checkboxes: {checkboxQuestions.Count}");
            Console.WriteLine($"Inputs: {inputQuestions.Count}");
            Console.WriteLine($"Selects: {selectQuestions.Count}");
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
            int questionNumber = 1;//Номер теста
            foreach (var answerDiv in answersDiv)
            {
                
                var questionText = answerDiv.TextContent.Replace("\n", " ");//Замена переноса строк на пробел
                questionText = Regex.Replace(questionText, @"([ ]){2,}", @" ");//Удаление повторяющихся пробелов

                

                string header = String.Format("------------------------------------------------{0:d3}----------------------------------------------", questionNumber);
                Console.WriteLine(header);
                Console.WriteLine($"{questionText}");
                switch (getQuestionType(answerDiv))
                {
                    case QuestionType.Radio:
                        var tempRadioQuestion = new QuestionModel<RadioAnswerModel>(questionText, QuestionType.Radio, QuestionCondition.None);
                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("label"))
                        {
                            item.RemoveElement(item.QuerySelector("span"));
                            Console.WriteLine($"  ( ) {item.TextContent}");
                            tempRadioQuestion.Answers.Add(new RadioAnswerModel(item.TextContent, AnswerCondition.None));
                        }
                        radioQuestions.Add(tempRadioQuestion);
                        break;
                    case QuestionType.Input:
                        var tempInputQuestion = new QuestionModel<InputAnswerModel>(questionText, QuestionType.Input, QuestionCondition.None);
                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("input[id*=answer]"))
                        {
                            Console.WriteLine($"  [{item.GetAttribute("value")}]");
                            tempInputQuestion.Answers.Add(new InputAnswerModel(item.GetAttribute("value"), AnswerCondition.None));
                        }
                        inputQuestions.Add(tempInputQuestion);
                        break;
                    case QuestionType.Checkbox:
                        var tempCheckboxQuestion = new QuestionModel<CheckboxAnswerModel>(questionText, QuestionType.Checkbox, QuestionCondition.None);
                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("label"))
                        {
                            item.RemoveElement(item.QuerySelector("span"));
                            Console.WriteLine($"  [ ] {item.TextContent}");
                            tempCheckboxQuestion.Answers.Add(new CheckboxAnswerModel(item.TextContent, AnswerCondition.None));
                        }
                        checkboxQuestions.Add(tempCheckboxQuestion);
                        break;
                    case QuestionType.Select:
                        var tempSelectQuestion = new QuestionModel<SelectAnswerModel>(questionText, QuestionType.Select, QuestionCondition.None);

                        foreach (var item in answerDiv.ParentElement.QuerySelectorAll("tr[class*=r]"))
                        {
                            var text = item.QuerySelector("p").TextContent;
                            var optionText = item.QuerySelector("option[selected=selected]").TextContent;
                            Console.Write($"  {text} ");
                            Console.WriteLine($"[ v {optionText}]");
                            tempSelectQuestion.Answers.Add(new SelectAnswerModel(text, optionText, AnswerCondition.None));
                        }
                        selectQuestions.Add(tempSelectQuestion);
                        break;
                    case QuestionType.None:
                        Console.WriteLine("Неизвестный тип задания");
                        throw new Exception("Неизвестный тип задания");
                }
                Console.WriteLine();
                questionNumber++;
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

            PageModelLogger logger = new PageModelLogger(this);
            logger.Print();
            WriteAllTextQuestionToFile(document);

        }
        /// <summary>
        /// Записывает текст заданий в файл
        /// </summary>
        /// <param name="htmlDocument">HTML-содержимое страницы теста</param>
        private static void WriteAllTextQuestionToFile(IHtmlDocument htmlDocument)
        {
            List<string> lines = new List<string>();
            foreach (var item in htmlDocument.QuerySelectorAll("div.qtext"))
            {
                var line = item.TextContent.Replace("\n", " ");
                line = Regex.Replace(line, @"([ ]){2,}", @" ");
                lines.Add(line);
            }
            File.WriteAllLines("data.txt", lines.ToArray());
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
            if (model.Mark > 0) Console.WriteLine($"Mark: {model.Mark}/100");
            else Console.WriteLine($"Mark: None");
        }
        /// <summary>
        /// Выводит количество заданий в тесте
        /// </summary>
        private void printQuestionCount()
        {
            if (model.QuestionCount > 0) Console.WriteLine($"AnswersCount: {model.QuestionCount}");
            else Console.WriteLine($"AnswersCount: 0");
        }
        /// <summary>
        /// Выводит название теста
        /// </summary>
        private void printTestName()
        {
            if (model.TestName.Length > 0 && model.TestName != null) Console.WriteLine($"TestName: {model.TestName}");
            else Console.WriteLine($"TestName: None");
        }
        /// <summary>
        /// Выводит название предмета
        /// </summary>
        private void printDiscipline()
        {
            if (model.Discipline.Length > 0 && model.Discipline != null) Console.WriteLine($"Discipline: {model.Discipline}");
            else Console.WriteLine($"Discipline: None");
        }
    }
}
