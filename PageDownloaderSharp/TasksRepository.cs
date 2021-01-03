using System;
using System.Collections.Generic;

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
    enum AnswerСondition
    {
        Correct,
        Incorrect,
        None
    }

    /// <summary>
    /// Ответ на вопрос
    /// </summary>
    class Answer
    {
        public AnswerСondition AnswerСondition { get; set; }
        public string Text { get; set; }
        public Answer(AnswerСondition answerСondition, string text)
        {
            AnswerСondition = answerСondition;
            Text = text;
        }
    }

    class Question
    {
        public QuestionType QuestionType { get; set; }
        public string Text { get; private set; }

        public List<Answer> Answers { get; set; }

        public Question(QuestionType questionType, string text)
        {
            Answers = new List<Answer>();
            QuestionType = questionType;
            Text = text;
        }

        private void CalculateRadio()
        {
            int incorrectCounter = 0;// Счётчик неправильный ответов
            foreach (var item in Answers)// Подсчитывает неправильные ответы
            {
                if (item.AnswerСondition == AnswerСondition.Incorrect)
                {
                    incorrectCounter++;
                }
            }
            if (incorrectCounter == (Answers.Count - 1))// Если неверные ответы все, кроме одного
            {
                foreach (var item in Answers)
                {
                    if (item.AnswerСondition == AnswerСondition.None)// Находим последний ответ
                    {
                        item.AnswerСondition = AnswerСondition.Correct;// И устанавливаем его верных
                    }
                }
            }
            foreach (var item in Answers)
            {
                if (item.AnswerСondition == AnswerСondition.Correct)// Если есть верный ответ
                {
                    foreach (var it in Answers)
                    {
                        if (it.AnswerСondition != AnswerСondition.Correct)// То выставляем остальные ответы верными
                        {
                            it.AnswerСondition = AnswerСondition.Incorrect;
                        }
                    }
                    break;
                }
            }
        }

        private void CalculateCheckbox()
        {
            bool correctExsist = false;

            foreach (var answer in Answers)
            {
                if(answer.AnswerСondition == AnswerСondition.Correct)
                {
                    correctExsist = true;
                    break;
                }
            }
            if(correctExsist) foreach (var answer in Answers)
            {
                if (answer.AnswerСondition == AnswerСondition.None)
                {
                    answer.AnswerСondition = AnswerСondition.Incorrect;
                }
            }
        }


        public void CalculateCorrectAnswer()
        {
            switch (QuestionType)
            {
                case QuestionType.Radio:
                    CalculateRadio();
                    break;
                case QuestionType.Checkbox:
                    CalculateCheckbox();
                    break;
            }
        }
        public void Print()
        {
            Console.WriteLine("Тип вопроса: " + QuestionType.ToString());
            Console.WriteLine("Текст вопроса: " + Text);
            if (Answers.Count == 0)
            {
                Console.WriteLine("Ответов пока нет");
            }
            else
            {
                foreach (var item in Answers)
                {
                    if (item.AnswerСondition == AnswerСondition.Correct)
                    {
                        Console.WriteLine(item.Text + " + ");
                    }
                    else if (item.AnswerСondition == AnswerСondition.Incorrect)
                    {
                        Console.WriteLine(item.Text + " - ");
                    }
                    else
                    {
                        Console.WriteLine(item.Text);
                    }
                }
            }
        }
    }

    class TasksRepository
    {
        public List<Question> questions = new List<Question>();
        public bool Save()
        {
            return true;
        }
        public bool Load()
        {
            var question1 = new Question(QuestionType.Radio, "Совокупность людей, выделенных на основе поведенческих признаков - это ...");
            question1.Answers.Add(new Answer(AnswerСondition.None, "маргиналы"));
            question1.Answers.Add(new Answer(AnswerСondition.None, "массовая общность (агрегат)"));
            question1.Answers.Add(new Answer(AnswerСondition.Incorrect, "номинальная социальная группа"));
            question1.Answers.Add(new Answer(AnswerСondition.None, "реальная социальная группа"));


            var question3 = new Question(QuestionType.Checkbox, "Какие последовательные контейнерами поддерживают произвольный доступ?");
            question3.Answers.Add(new Answer(AnswerСondition.None, "массив"));
            question3.Answers.Add(new Answer(AnswerСondition.Correct, "однонаправленный список"));
            question3.Answers.Add(new Answer(AnswerСondition.Correct, "двунаправленный список"));
            question3.Answers.Add(new Answer(AnswerСondition.None, "вектор"));
            question3.Answers.Add(new Answer(AnswerСondition.None, "дек"));

            questions.Add(question1);
            questions.Add(question3);
            return true;
        }
        private bool isAnswersEqual(List<Answer> answers1, List<Answer> answers2)
        {
            if (answers1.Count != answers2.Count)
            {
                return false;
            }
            foreach (var answer in answers1) // Перебираем ответы из задания в базе
            {
                //Если удалённый ответ не существует среди ответов в базе - значит задания не равны
                if (!answers2.Exists(item => item.Text == answer.Text))
                {
                    return false;
                }
            }
            return true;
        }

        private void questionRadioSet(Question remoteQuestion, Question storageQuestion)
        {
            if (storageQuestion.QuestionType == QuestionType.Radio)
                foreach (var item in storageQuestion.Answers)//Перебираем ответы найденого задания
                {
                    if (item.AnswerСondition == AnswerСondition.None)//Если состояния ответа не задано
                    {
                        //Обновляем его значение из входного параметра
                        item.AnswerСondition = remoteQuestion.Answers.Find(it => it.Text == item.Text).AnswerСondition;
                    }
                    else if (item.AnswerСondition == AnswerСondition.Incorrect)//Если вариант ответа неверен
                    {
                        var storageAnswerCondition = item.AnswerСondition;
                        var remoteAnswerCondition = remoteQuestion.Answers.Find(it => it.Text == item.Text).AnswerСondition;//Но по новым данным он верно
                                                                                                                            //Заменяем его значение на верное
                        if (remoteAnswerCondition == AnswerСondition.Correct) item.AnswerСondition = AnswerСondition.Correct;
                    }
                }
        }
        private void questionCheckboxSet(Question remoteQuestion, Question storageQuestion)
        {
            if (storageQuestion.QuestionType == QuestionType.Checkbox)
            {
                foreach (var item in storageQuestion.Answers)//Перебираем ответы найденого задания
                {
                    item.AnswerСondition = remoteQuestion.Answers.Find(it => it.Text == item.Text).AnswerСondition;
                }
            }
        }

        public void AddQuestion(Question question)
        {
            // Если текст задания, кол-во ответов и тип задания совпадают находим в базе это задание
            var questionFinded = questions.Find(item => item.Text == question.Text
            && item.QuestionType == question.QuestionType
            && isAnswersEqual(item.Answers, question.Answers)
            );
            if (questionFinded != null)// Если задание существует
            {
                if (questionFinded.QuestionType == QuestionType.Radio)
                {
                    questionRadioSet(question, questionFinded);
                }//Если это радиокнопка
                else if (questionFinded.QuestionType == QuestionType.Checkbox)
                {
                    questionCheckboxSet(question, questionFinded);
                }//Если это чекбокс
                

                questionFinded.CalculateCorrectAnswer();//Нормализуем ответы

            }
            else
            {
                questions.Add(question);//Иначе добавляем новое задание в базу
            }
        }
        public void Print()
        {
            foreach (var question in questions)
            {
                question.Print();
                Console.WriteLine();
            }
        }
        public TasksRepository(string pathExcelFile)
        {
            Load();
        }
    }
}
