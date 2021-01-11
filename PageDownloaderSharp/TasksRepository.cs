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
            Console.WriteLine($"{Text.Item1} [{Text.Item2}] ({Math.Floor(Propability * 100)}%)");
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
            if (selectQuestion1.Answers.Count != selectQuestion2.Answers.Count) return true;
            if (selectQuestion1.Text.ToLower() != selectQuestion2.Text.ToLower()) return true;

            bool x = false;
            bool y = false;
            foreach (var item in selectQuestion1.Answers)
            {

                x = selectQuestion2.Answers.Exists(m => m.Text.Item1.ToLower() == item.Text.Item1.ToLower());
                y = selectQuestion2.Answers.Exists(m => m.Text.Item2.ToLower() == item.Text.Item2.ToLower());
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
            Console.WriteLine($"[v]{Text} ({Math.Floor(Propability * 100)}%)");
        }
    }
    public class CheckboxQuestion
    {
        public int CorrectAnswers { get; set; } = 0;
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



        public void Update(CheckboxQuestion inCheckboxQuestion)
        {
            if (this != inCheckboxQuestion) return;//Если входящее задание не равно текущему не обновлять и выйти из метода
            double inQuestion = 0.0;
            double storageQuestion = 0.0;
            foreach (var item in Answers)
            {
                storageQuestion += Math.Abs((item.Propability - 0.5));
            }
            foreach (var item in inCheckboxQuestion.Answers)
            {
                inQuestion += Math.Abs((item.Propability - 0.5));
            }
            if (inQuestion > storageQuestion)
            {
                Answers.Clear();
                Answers = inCheckboxQuestion.Answers;
            }
            else
            {
                Console.WriteLine("Апдейт не нужен");
            }
        }

        public static bool operator !=(CheckboxQuestion selectQuestion1, CheckboxQuestion selectQuestion2)
        {
            if (selectQuestion1.Answers.Count != selectQuestion2.Answers.Count) return true;
            if (selectQuestion1.Text.ToLower() != selectQuestion2.Text.ToLower()) return true;


            foreach (var item in selectQuestion1.Answers)
            {
                if (!(selectQuestion2.Answers.Exists(m2 => m2.Text.ToLower() == item.Text.ToLower())))
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
    public class RadioAnswer
    {
        public string Text { get; set; }
        private double propability;
        public double Propability { get { return propability; } set { if ((value >= 0) && (value <= 1)) propability = value; } }
        public void Print()
        {
            Console.WriteLine($"(o) {Text} ({Math.Floor(Propability * 100)}%)");
        }
        public RadioAnswer(string text, double propability)
        {
            Text = text;
            Propability = propability;
        }
    }
    public class RadioQuestion
    {
        public List<RadioAnswer> Answers { get; set; } = new List<RadioAnswer>();
        /// <summary>
        /// Текст задания
        /// </summary>
        public string Text { get; set; }

        public RadioQuestion(string text)
        {
            Text = text;
        }

        public void Update(RadioQuestion inRadioQuestion)
        {
            if (this != inRadioQuestion) return;//Если входящее задание не равно текущему не обновлять и выйти из метода
            double maxStoragePropability = 0;
            double maxInPropability = 0;
            foreach (var storageAnswer in Answers)
            {
                if (storageAnswer.Propability > maxStoragePropability) maxStoragePropability = storageAnswer.Propability;
            }
            foreach (var inAnswer in inRadioQuestion.Answers)
            {
                if (inAnswer.Propability > maxInPropability) maxInPropability = inAnswer.Propability;
            }
            if (maxStoragePropability < maxInPropability)
            {
                Answers.Clear();
                Answers = inRadioQuestion.Answers;
            }
        }

        public static bool operator !=(RadioQuestion radioQuestion1, RadioQuestion radioQuestion2)
        {
            if (radioQuestion1.Answers.Count != radioQuestion2.Answers.Count) return true;
            if (radioQuestion1.Text.ToLower() != radioQuestion2.Text.ToLower()) return true;

            foreach (var item in radioQuestion1.Answers)
            {
                if (!(radioQuestion2.Answers.Exists(m2 => m2.Text.ToLower() == item.Text.ToLower())))
                {
                    return true;
                }
            }
            return false;

        }

        public static bool operator ==(RadioQuestion radioQuestion1, RadioQuestion radioQuestion2)
        {
            return !(radioQuestion1 != radioQuestion2);
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
    public class InputAnswer
    {
        public string Text { get; set; }
        private double propability;
        public double Propability { get { return propability; } set { if ((value >= 0) && (value <= 1)) propability = value; } }
        public void Print()
        {
            Console.Write($"[{Text} ({Math.Floor(Propability * 100)}%) ]");
        }
        public InputAnswer(string text, double propability)
        {
            Text = text;
            Propability = propability;
        }
    }
    public class InputQuestion
    {
        public List<InputAnswer> Answers { get; set; } = new List<InputAnswer>();
        /// <summary>
        /// Текст задания
        /// </summary>
        public string Text { get; set; }

        public InputQuestion(string text)
        {
            Text = text;
        }

        public void Update(InputQuestion inInputQuestion)
        {
            if (this != inInputQuestion) return;//Если входящее задание не равно текущему не обновлять и выйти из метода
            foreach (var remoteAnswer in inInputQuestion.Answers)
            {
                var foundAnswer = Answers.Find(answer => answer.Text.ToLower() == remoteAnswer.Text.ToLower());
                if(!(object.ReferenceEquals(foundAnswer, null)))
                {
                    if(foundAnswer.Propability < remoteAnswer.Propability)
                    {
                        foundAnswer.Propability = remoteAnswer.Propability;
                    }
                    break;
                }
                else
                {
                    Answers.Add(remoteAnswer);
                }
            }
        }

        public static bool operator !=(InputQuestion inputQuestion1, InputQuestion inputQuestion2)
        {
            return inputQuestion1.Text.ToLower() != inputQuestion2.Text.ToLower();
        }

        public static bool operator ==(InputQuestion inputQuestion1, InputQuestion inputQuestion2)
        {
            return !(inputQuestion1 != inputQuestion2);
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
        private List<SelectQuestion> SelectQuestions = new List<SelectQuestion>();
        private List<CheckboxQuestion> CheckboxQuestions = new List<CheckboxQuestion>();
        private List<RadioQuestion> RadioQuestions  = new List<RadioQuestion>();
        private List<InputQuestion> InputQuestions  = new List<InputQuestion>();
        public bool Save()
        {
            return true;
        }
        public void AddSelectQuestion(SelectQuestion question)
        {
            var found = SelectQuestions.Find(item => item == question);

            bool inNull = object.ReferenceEquals(found, null);
            if (!inNull)
            {
                found.Update(question);
            }
            else
            {
                SelectQuestions.Add(question);
            }

        }
        public void AddCheckboxQuestion(CheckboxQuestion question)
        {
            var found = CheckboxQuestions.Find(item => item == question);

            bool inNull = object.ReferenceEquals(found, null);
            if (!inNull)
            {
                found.Update(question);
            }
            else
            {
                CheckboxQuestions.Add(question);
            }

        }
        public void AddRadioQuestion(RadioQuestion question)
        {
            var found = RadioQuestions.Find(item => item == question);

            bool inNull = object.ReferenceEquals(found, null);
            if (!inNull)
            {
                found.Update(question);
            }
            else
            {
                RadioQuestions.Add(question);
            }

        }

        public void AddInputQuestion(InputQuestion question)
        {
            var found = InputQuestions.Find(item => item == question);

            bool inNull = object.ReferenceEquals(found, null);
            if (!inNull)
            {
                found.Update(question);
            }
            else
            {
                InputQuestions.Add(question);
            }


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

            AddSelectQuestion(selectQuestion1);
            AddSelectQuestion(selectQuestion2);
        }
        private void LoadCheckboxQuestions()
        {
            CheckboxQuestion checkboxQuestion1 = new CheckboxQuestion("Какие последовательные контейнерами поддерживают произвольный доступ?");
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("массив", 0.65));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("однонаправленный список", 0.5));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("двунаправленный список", 0.5));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("вектор", 0.5));
            checkboxQuestion1.Answers.Add(new CheckboxAnswer("дек", 0.5));


            CheckboxQuestion checkboxQuestion2 = new CheckboxQuestion("Какие последовательные контейнерами поддерживают произвольный доступ?");
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("массив", 0.15));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("однонаправленный список", 0.55));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("двунаправленный список", 0.55));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("дек", 0.55));
            checkboxQuestion2.Answers.Add(new CheckboxAnswer("вектор", 0.55));



            AddCheckboxQuestion(checkboxQuestion1);
            AddCheckboxQuestion(checkboxQuestion2);
        }
        private void LoadRadioQuestions()
        {
            RadioQuestion radioQuestion1 = new RadioQuestion("Что Не относится к критериям социальной стратификации современного общества?");
            radioQuestion1.Answers.Add(new RadioAnswer("возраст", 0.65));
            radioQuestion1.Answers.Add(new RadioAnswer("престиж", 0.15));
            radioQuestion1.Answers.Add(new RadioAnswer("доход", 0.44));
            radioQuestion1.Answers.Add(new RadioAnswer("образование", 0.33));

            RadioQuestion radioQuestion2 = new RadioQuestion("Что НЕ относится к критериям социальной стратификации современного общества?");
            radioQuestion2.Answers.Add(new RadioAnswer("возраст", 0.95));
            radioQuestion2.Answers.Add(new RadioAnswer("престиж", 0.15));
            radioQuestion2.Answers.Add(new RadioAnswer("образование", 0.69));
            radioQuestion2.Answers.Add(new RadioAnswer("доход", 0.45));

            AddRadioQuestion(radioQuestion1);
            AddRadioQuestion(radioQuestion2);
        }
        private void LoadInputQuestions()
        {
            InputQuestion inputQuestion1 = new InputQuestion("Гениальный полководец, «маршал Победы», который принимал парад Победы 24 июня 1945 г. это ");
            inputQuestion1.Answers.Add(new InputAnswer("Жуков", 0.65));

            InputQuestion inputQuestion2 = new InputQuestion("Гениальный полководец, «маршал Победы», который принимал парад Победы 24 июня 1945 г. это ");
            inputQuestion2.Answers.Add(new InputAnswer("Наполеон", 0.85));

            AddInputQuestion(inputQuestion1);
            AddInputQuestion(inputQuestion2);
        }


        public void Load()
        {
            LoadSelectQuesions();
            LoadCheckboxQuestions();
            LoadRadioQuestions();
            LoadInputQuestions();
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

        private void PrintRadioQuestions()
        {
            foreach (var question in RadioQuestions)
            {
                question.Print();
            }
        }

        private void PrintInputQuestions()
        {
            foreach (var question in InputQuestions)
            {
                question.Print();
            }
        }
        public void Print()
        {
            PrintSelectQuestions();
            PrintCheckboxQuestions();
            PrintRadioQuestions();
            PrintInputQuestions();
        }
        public TasksRepository(string pathExcelFile)
        {
            Load();
        }
    }
}
