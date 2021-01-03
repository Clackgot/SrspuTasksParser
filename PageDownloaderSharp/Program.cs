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
            TasksRepository tasksRepository = new TasksRepository("data.xls");
            var question2 = new Question(QuestionType.Radio, "Совокупность людей, выделенных на основе поведенческих признаков - это ...");
            question2.Answers.Add(new Answer(AnswerСondition.Incorrect, "маргиналы"));
            question2.Answers.Add(new Answer(AnswerСondition.Incorrect, "массовая общность (агрегат)"));
            question2.Answers.Add(new Answer(AnswerСondition.None, "номинальная социальная группа"));
            question2.Answers.Add(new Answer(AnswerСondition.None, "реальная социальная группа"));

            var question3 = new Question(QuestionType.Checkbox, "Какие последовательные контейнерами поддерживают произвольный доступ?");
            question3.Answers.Add(new Answer(AnswerСondition.Correct, "массив"));
            question3.Answers.Add(new Answer(AnswerСondition.None, "однонаправленный список"));
            question3.Answers.Add(new Answer(AnswerСondition.Incorrect, "двунаправленный список"));
            question3.Answers.Add(new Answer(AnswerСondition.Correct, "вектор"));
            question3.Answers.Add(new Answer(AnswerСondition.None, "дек"));

            tasksRepository.AddQuestion(question2);
            tasksRepository.AddQuestion(question3);
            tasksRepository.Print();
        }
    }
}
