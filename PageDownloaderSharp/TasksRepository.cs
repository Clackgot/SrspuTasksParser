using System;
using System.Collections.Generic;

namespace PageDownloaderSharp
{
    public class SelectAnswer
    {
        /// <summary>
        /// Ответ в виде кортежа из двух строк
        /// </summary>
        public Tuple<string, string> Text { get; set; }
        private double propability;
        /// <summary>
        /// Точность ответа
        /// </summary>
        public double Propability { get { return propability; } set { if ((value >= 0) && (value <= 1)) propability = value; } }

        /// <summary>
        /// Конструктор класса задания с селекторами
        /// </summary>
        /// <param name="answer">Вариант ответа задания</param>
        /// <param name="propability">Точность ответа на задание</param>
        public SelectAnswer(Tuple<string, string> answer, double propability)
        {
            Text = answer;
            Propability = propability;
        }
        /// <summary>
        /// Вывод на в консоль варианта ответа
        /// </summary>
        public void Print()
        {
            Console.WriteLine($"{Text.Item1} - {Text.Item2} ({Math.Floor(Propability*100)}%)");
        }
    }
    public class SelectQuestion
    {
        /// <summary>
        /// Список вариантов ответа
        /// </summary>
        public List<SelectAnswer> Answers { get; set; } = new List<SelectAnswer>();
        /// <summary>
        /// Текст задания
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="text">Текст задания</param>
        public SelectQuestion(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Обновляет ответы задания на более точные, при условии того, 
        /// что сумма вероятностей правильного ответа входящего задания больше суммы вероятностей текущего
        /// </summary>
        /// <param name="inSelectQuestion">Задание для обновления</param>
        public void Update(SelectQuestion inSelectQuestion)
        {
            if (this != inSelectQuestion) return;//Если входящее задание не равно текущему не обновлять и выйти из метода
            double sumInPropability = 0.0;//Сумма вероятностей ответов входящего задания
            double sumMaxStoragePropability = 0.0;//Сумма вероятностей ответов тукущего задания
            foreach (var inSelectAnswer in inSelectQuestion.Answers)
            {
                sumInPropability += inSelectAnswer.Propability;
            }
            foreach (var answer in Answers)
            {
                sumMaxStoragePropability += answer.Propability;
            }
            //Если сумма вероятностей правильного ответа выше 
            //во входящем задании - обновить ответы текущего задания
            if (sumInPropability > sumMaxStoragePropability)
            {
                Answers.Clear();
                Answers = inSelectQuestion.Answers;
            }
        }


        /// <summary>
        /// Перегруженный оператор сравнения двух заданий
        /// </summary>
        /// <param name="selectQuestion1">Первое задание для сравнение</param>
        /// <param name="selectQuestion2">Второе задание для сравнение</param>
        /// <returns></returns>
        public static bool operator ==(SelectQuestion selectQuestion1, SelectQuestion selectQuestion2)
        {
            return !(selectQuestion1 != selectQuestion2);
        }
        public static bool operator !=(SelectQuestion selectQuestion1, SelectQuestion selectQuestion2)
        {
            if(selectQuestion1.Answers.Count != selectQuestion2.Answers.Count) return true;
            if (selectQuestion1.Text != selectQuestion2.Text) return true;

            bool x = false;
            bool y = false;
            foreach (var item in selectQuestion1.Answers)
            {

                x = selectQuestion2.Answers.Exists(m => m.Text.Item1 == item.Text.Item1);
                y = selectQuestion2.Answers.Exists(m => m.Text.Item2 == item.Text.Item2);
                if (!(x || y)) return true;
            }
            return false;
        }
        /// <summary>
        /// Вывод задания в консоль
        /// </summary>
        public void Print()
        {
            Console.WriteLine(Text);
            foreach (var item in Answers)
            {
                item.Print();
            }
            Console.WriteLine();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

    public class CheckboxAnswer
    {
        public string Text { get; set; }
        private double propability;
        public double Propability { get { return propability; } set { if ((value >= 0) && (value <= 1)) propability = value; } }
        public CheckboxAnswer(string text, double propability)
        {
            Text = text;
            Propability = propability;
        }
        /// <summary>
        /// Вывод на в консоль варианта ответа
        /// </summary>
        public void Print()
        {
            Console.WriteLine($"{Text} ({Math.Floor(Propability * 100)}%)");
        }
    }

    public class CheckboxQuestion
    {
        /// <summary>
        /// Список вариантов ответа
        /// </summary>
        public List<CheckboxAnswer> Answers { get; set; } = new List<CheckboxAnswer>();

        /// <summary>
        /// Текст задания
        /// </summary>
        public string Text { get; set; }

        public CheckboxQuestion(string text)
        {
            Text = text;
        }



        public static bool operator !=(CheckboxQuestion selectQuestion1, CheckboxQuestion selectQuestion2)
        {
            if (selectQuestion1.Answers.Count != selectQuestion2.Answers.Count) return true;
            if (selectQuestion1.Text != selectQuestion2.Text) return true;

            foreach (var item in selectQuestion1.Answers)
            {
                if (!(selectQuestion2.Answers.Exists(m2 => m2.Text == item.Text)))
                {
                    return true;
                }
            }
            return false;

        }

        public static bool operator ==(CheckboxQuestion selectQuestion1, CheckboxQuestion selectQuestion2)
        {
            return !(selectQuestion1 != selectQuestion2);
        }


        public void Print()
        {
            Console.WriteLine(Text);
            foreach (var item in Answers)
            {
                item.Print();
            }
            Console.WriteLine();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }



    class TasksRepository
    {
        public List<SelectQuestion> SelectQuestions { get; set; } = new List<SelectQuestion>();
        public List<CheckboxQuestion> CheckboxQuestions { get; set; } = new List<CheckboxQuestion>();
        public bool Save()
        {
            return true;
        }


        private void LoadSelectQuesions()
        {
            SelectQuestion selectQuestion1 = new SelectQuestion("Установите соответствие между примерами и формами культуры");
            selectQuestion1.Answers.Add(new SelectAnswer(new Tuple<string, string>("Сказка \"Теремок\"", "народная культура"), 0.23));
            selectQuestion1.Answers.Add(new SelectAnswer(new Tuple<string, string>("Музыка Баха", "элитарная культура"), 10.23));
            selectQuestion1.Answers.Add(new SelectAnswer(new Tuple<string, string>("Песня \"Рюмка водки на столе\", исполняемая Григорием Лепсом", "массовая культура"), 0.23));

            SelectQuestion selectQuestion2 = new SelectQuestion("Установите соответствие между примерами и формами культуры");
            selectQuestion2.Answers.Add(new SelectAnswer(new Tuple<string, string>("наблюдение, самонаблюдение", "психодиагностические методы"), 0.13));
            selectQuestion2.Answers.Add(new SelectAnswer(new Tuple<string, string>("лабораторный, естественный, формирующий эксперименты", "обсервационные методы"), 0.41));
            selectQuestion2.Answers.Add(new SelectAnswer(new Tuple<string, string>("тесты, анкеты, социометрия, интервью, беседа", "экспериментальные методы"), 0.87));

            SelectQuestion selectQuestion3 = new SelectQuestion("Установите соответствие между примерами и формами культуры");
            selectQuestion3.Answers.Add(new SelectAnswer(new Tuple<string, string>("наблюдение, самонаблюдение", "экспериментальные методы"), 0.13));
            selectQuestion3.Answers.Add(new SelectAnswer(new Tuple<string, string>("лабораторный, естественный, формирующий эксперименты", "обсервационные методы"), 0.11));
            selectQuestion3.Answers.Add(new SelectAnswer(new Tuple<string, string>("тесты, анкеты, социометрия, интервью, беседа", "психодиагностические методы"), 0.87));

            SelectQuestions.Add(selectQuestion1);
            SelectQuestions.Add(selectQuestion2);
            SelectQuestions.Add(selectQuestion3);
        }
        private void LoadCheckboxQuestions()
        {
            CheckboxQuestion checkboxQuestion1 = new CheckboxQuestion("Какие последовательные контейнерами поддерживают произвольный доступ?");
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("массив", 0.32));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("однонаправленный список", 0.94));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("двунаправленный список", 0.77));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("вектор", 0.3));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("дек", 0.4));


            CheckboxQuestion checkboxQuestion2 = new CheckboxQuestion("Какие последовательные контейнерами поддерживают произвольный доступ?");
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("массив", 0));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("однонаправленный список", 1));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("двунаправленный список", 1));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("дек", 0));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("вектор", 1));


            CheckboxQuestions.Add(checkboxQuestion1);
            CheckboxQuestions.Add(checkboxQuestion2);
        }

        public bool Load()
        {
            LoadSelectQuesions();

            LoadCheckboxQuestions();


            return true;
        }



        private void PrintSelectQuestions()
        {
            foreach (var question in SelectQuestions)
            {
                question.Print();
            }
        }
        private void PrintCheckboxQuestions()
        {
            foreach (var question in CheckboxQuestions)
            {
                question.Print();
            }
        }
        public void Print()
        {
            //PrintSelectQuestions();
            PrintCheckboxQuestions();
            Console.WriteLine(CheckboxQuestions[0] == CheckboxQuestions[1]);
        }
        public TasksRepository(string pathExcelFile)
        {
            Load();
        }
    }
}
