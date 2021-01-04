using System;
using System.Collections.Generic;

namespace PageDownloaderSharp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            //История
            //https://sdo.srspu.ru/mod/quiz/review.php?attempt=173690&cmid=48873

            //var testUrl = "https://sdo.srspu.ru/mod/quiz/review.php?attempt=818378&cmid=54999";
            //SdoControl sdo = new SdoControl();
            //var testModel = new TestModel(sdo.GetPage(testUrl));

            //var question2 = new Question(QuestionType.Radio, "Совокупность людей, выделенных на основе поведенческих признаков - это ...", 1.0);
            //question2.Answers.Add(new Question(AnswerСondition.None, "маргиналы"));
            //question2.Answers.Add(new Question(AnswerСondition.None, "массовая общность (агрегат)"));
            //question2.Answers.Add(new Question(AnswerСondition.None, "номинальная социальная группа"));
            //question2.Answers.Add(new Question(AnswerСondition.None, "реальная социальная группа"));

            //var question3 = new Question(QuestionType.Checkbox, "Какие последовательные контейнерами поддерживают произвольный доступ?", 0.9);
            //question3.Answers.Add(new Question(AnswerСondition.None, "массив"));
            //question3.Answers.Add(new Question(AnswerСondition.None, "однонаправленный список"));
            //question3.Answers.Add(new Question(AnswerСondition.None, "двунаправленный список"));
            //question3.Answers.Add(new Question(AnswerСondition.None, "вектор"));
            //question3.Answers.Add(new Question(AnswerСondition.None, "дек"));



            TasksRepository tasksRepository = new TasksRepository("data.xls");



            tasksRepository.Print();
        }
    }
}
