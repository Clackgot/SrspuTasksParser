using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AttestationSynchronizer
{
    public class User
    {
        private string _login;
        private string _password;
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public User(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }

    public class SdoLoader
    {
        private HtmlParser parser = new HtmlParser();
        private readonly string url = "https://sdo.srspu.ru/login/index.php";
        List<User> users;

        public SdoLoader(List<User> users)
        {
            this.users = users;
            List<Task<IHtmlDocument>> tasks = new List<Task<IHtmlDocument>>();

            foreach (var user in users)
            {
                tasks.Add(getPage("https://sdo.srspu.ru/my/", user));

            }


            Task.WaitAll(tasks.ToArray());


        }

        public async Task<IHtmlDocument> getPage(string targetUrl, User user)
        {
            var client = await getAuthorizedClient(user);
            var responseMessage = await client.GetAsync(targetUrl);
            var content = await responseMessage.Content.ReadAsStringAsync();
            var document = await parser.ParseDocumentAsync(content);
            Console.WriteLine(document.QuerySelector("div.page-context-header h1").TextContent);
            return document;
        }


        private async Task<HttpClient> getAuthorizedClient(User user)
        {

            var handler = new HttpClientHandler() { UseCookies = true };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Host", "sdo.srspu.ru");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");

            var getRequest = await client.GetAsync(url);


            var document = parser.ParseDocument(await getRequest.Content.ReadAsStringAsync());
            var logintoken = document.QuerySelector("input[name=logintoken]").GetAttribute("value");

            //var moodlesession = handler.CookieContainer.GetCookies(new Url(url))[0].Value;



            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>() {
             new KeyValuePair<string, string>("anchor", ""),
             new KeyValuePair<string, string>("logintoken", logintoken),
             new KeyValuePair<string, string>("username", user.Login),
             new KeyValuePair<string, string>("password", user.Password),

            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(keyValues);
            //client.DefaultRequestHeaders.Add("Cookie", $"MoodleSession = {moodlesession}");
            var postRequest = await client.PostAsync(url, content);
            var document2 = parser.ParseDocument(postRequest.Content.ReadAsStringAsync().Result);
            return client;
        }

    }



    public struct Marks
    {
        public struct AttestationMarks
        {
            public string lection;
            public string practic;
            public string laboratory;
            public string other;
            public string total;
            public void Print()
            {
                Console.Write($"" +
                    $"[{(lection == "" ? " " : lection)}]" +
                    $"[{(practic == "" ? " " : practic)}]" +
                    $"[{(laboratory == "" ? " " : laboratory)}]" +
                    $"[{(other == "" ? " " : other)}]" +
                    $"[{(total == "" ? " " : total)}]");
            }
        }
        public AttestationMarks attestation1;
        public AttestationMarks attestation2;
        public AttestationMarks attestation3;
        public string bonus;
        public string finalyRating;
        public string markByRating;
        public string total;
        public void Print()
        {
            attestation1.Print();
            Console.Write(" ");
            attestation2.Print();
            Console.Write(" ");
            attestation3.Print();
            Console.Write(" ");
            Console.WriteLine($"Бонус: {bonus} Итоговый рейтинг по КТ: {finalyRating} Оценка по рейтингу: {markByRating} Итоги: {total}");
        }
    }

    public class Student
    {
        #region fields
        private HtmlParser parser = new HtmlParser();
        private IHtmlDocument document;
        string fullname;
        string gradebookNumber;
        Marks marks;

        public Student(string fullname, string gradebookNumber, Marks marks)
        {
            Fullname = fullname;
            GradebookNumber = gradebookNumber;
            Marks = marks;
        }

        private void parse()
        {

            fullname = document.GetElementById("ctl00_LoginView1_HeadLoginName").TextContent;
            gradebookNumber = document.QuerySelectorAll("tr#ctl00_MainContent_ucVedBox_TableVedFixed_DXDataRow0 td")[1].TextContent;
            var marksTags = document.QuerySelectorAll("tr#ctl00_MainContent_ucVedBox_TableVed_DXDataRow0 td");




            marks = new Marks()
            {
                attestation1 = new Marks.AttestationMarks { lection = marksTags[0].TextContent, practic = marksTags[1].TextContent, laboratory = marksTags[2].TextContent, other = marksTags[3].TextContent, total = marksTags[4].TextContent },
                attestation2 = new Marks.AttestationMarks { lection = marksTags[5].TextContent, practic = marksTags[6].TextContent, laboratory = marksTags[7].TextContent, other = marksTags[8].TextContent, total = marksTags[9].TextContent },
                attestation3 = new Marks.AttestationMarks { lection = marksTags[10].TextContent, practic = marksTags[11].TextContent, laboratory = marksTags[12].TextContent, other = marksTags[13].TextContent, total = marksTags[14].TextContent },
                bonus = marksTags[15].TextContent,
                finalyRating = marksTags[16].TextContent,
                markByRating = marksTags[17].TextContent,
                total = marksTags[18].TextContent,
            };

        }

        public Student(IHtmlDocument document)
        {
            this.document = document;
            parse();
        }
        public string Fullname { get => fullname; set => fullname = value; }
        public string GradebookNumber { get => gradebookNumber; set => gradebookNumber = value; }
        internal Marks Marks { get => marks; set => marks = value; }



        #endregion


        public void Print()
        {
            Console.Write($"{fullname} {GradebookNumber} ");
            marks.Print();
        }
    }
    public class Discipline
    {
        #region fields
        private HtmlParser parser = new HtmlParser();
        private IHtmlDocument document;
        private string status;
        private string examType;
        private string group;
        private string department;
        private string name;
        private string teacher;
        private string course;
        private string year;
        private string hours;
        private string semester;
        private string computer;
        private string saveCount;

        public string Status { get => status; private set => status = value; }
        public string ExamType { get => examType; private set => examType = value; }
        public string Group { get => group; private set => group = value; }
        public string Department { get => department; private set => department = value; }
        public string Name { get => name; private set => name = value; }
        public string Teacher { get => teacher; private set => teacher = value; }
        public string Course { get => course; private set => course = value; }
        public string Year { get => year; private set => year = value; }
        public string Hours { get => hours; private set => hours = value; }
        public string Semester { get => semester; private set => semester = value; }
        public string Computer { get => computer; private set => computer = value; }
        public string SaveCount { get => saveCount; private set => saveCount = value; }
        public List<Student> Students { get => students; set => students = value; }
        #endregion
        List<Student> students = new List<Student>();

        private void parse()
        {
            Status = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblStatus").TextContent;
            ExamType = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblTypeVed").TextContent;
            Group = document.QuerySelector("a#ctl00_MainContent_ucVedBox_lblGroup").TextContent;
            Department = document.QuerySelector("a#ctl00_MainContent_ucVedBox_lblKafName").TextContent;
            Name = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblDis").TextContent;
            Teacher = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblPrep").TextContent;
            Course = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblKurs").TextContent;
            Year = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblYear").TextContent;
            Hours = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblHours").TextContent;
            Semester = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblSem").TextContent;
            Computer = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblLastUser").TextContent;
            SaveCount = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblSaveCount").TextContent;
        }

        public List<List<string>> GetData()
        {
            List<List<string>> tempData = new List<List<string>>();
            foreach (var student in Students)
            {
                tempData.Add(new List<string>() {
                student.Fullname,
                student.GradebookNumber,
                student.Marks.attestation1.lection,
                student.Marks.attestation1.practic,
                student.Marks.attestation1.laboratory,
                student.Marks.attestation1.other,
                student.Marks.attestation1.total,
                student.Marks.attestation2.lection,
                student.Marks.attestation2.practic,
                student.Marks.attestation2.laboratory,
                student.Marks.attestation2.other,
                student.Marks.attestation2.total,
                student.Marks.attestation3.lection,
                student.Marks.attestation3.practic,
                student.Marks.attestation3.laboratory,
                student.Marks.attestation3.other,
                student.Marks.attestation3.total,
                student.Marks.bonus,
                student.Marks.finalyRating,
                student.Marks.markByRating,
                student.Marks.total,
                });
            }
            return tempData;
        }


        public void AddStudent(Student studentMarks)
        {
            Students.Add(studentMarks);
        }
        public Discipline(IHtmlDocument document)
        {
            this.document = document;
            parse();
        }

        public void Print()
        {
            Console.WriteLine(Name);
            foreach (var student in Students)
            {
                student.Print();
            }
        }


    }
    public class DecLoader
    {
        private List<User> users;
        private HtmlParser parser = new HtmlParser();
        private readonly string url = "https://dec.srspu.ru/Account/Login.aspx";
        public List<Discipline> disciplines = new List<Discipline>();
        private CancellationTokenSource cancelTokenSource;
        private CancellationToken token;

        private List<string> disciplinesLinks;

        public DecLoader(List<User> users)
        {
            this.users = users;
            cancelTokenSource = new CancellationTokenSource();

            token = cancelTokenSource.Token;


            initDisciplines().Wait();
            LoadStudents(disciplinesLinks).Wait();

        }

        private async Task initDisciplines()
        {
            try
            {
                disciplinesLinks = new List<string>();
                var client = getAuthorizedClient(users[0]).Result;
                var document = await getPage("https://dec.srspu.ru/Ved/", client, CancellationToken.None);
                var disciplinesTags = document.QuerySelectorAll("a.dxeHyperlink_MaterialCompact");
                List<Task<IHtmlDocument>> tasksDocument = new List<Task<IHtmlDocument>>();
                foreach (var disciplineTag in disciplinesTags)
                {
                    var link = @"https://dec.srspu.ru/Ved/" + disciplineTag.GetAttribute("href");
                    //AddDispline(new Discipline(await getPage(link, client, CancellationToken.None)));
                    tasksDocument.Add(getPage(link, client, CancellationToken.None));
                    disciplinesLinks.Add(link);
                }
                Task.WaitAll(tasksDocument.ToArray(), CancellationToken.None);
                foreach (var page in tasksDocument)
                {
                    AddDispline(new Discipline(page.Result));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при инициализации дисциплин");
            }
        }

        private async Task UpdateDisciplines()
        {
            foreach (var user in users)
            {
                foreach (var disciplinesLink in disciplinesLinks)
                {

                }

            }
        }

        private async Task LoadStudents(List<string> links)
        {

            List<Task> tasks = new List<Task>();
            foreach (var user in users)
            {
                try
                {
                    tasks.Add(AddStudent(links, user));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при загрузке пользователя {user.Login}");
                }
                Task.WaitAll(tasks.ToArray());
            }


        }

        private async Task AddStudent(List<string> links, User user)
        {
            var client = await getAuthorizedClient(user);
            List<Task<IHtmlDocument>> tasksDocument = new List<Task<IHtmlDocument>>();
            foreach (var link in links)
            {
                tasksDocument.Add(getPage(link, client, CancellationToken.None));
            }
            Task.WaitAll(tasksDocument.ToArray(), CancellationToken.None);
            foreach (var page in tasksDocument)
            {
                var currentDiscipline = page.Result.QuerySelector("span#ctl00_MainContent_ucVedBox_lblDis");
                var student = new Student(page.Result);

                disciplines.Find(d => d.Name == currentDiscipline.TextContent).AddStudent(student);
            }
            Console.WriteLine($"Добавлен {user.Login}");
        }
        public void AddDispline(Discipline discipline)
        {
            disciplines.Add(discipline);
        }

        public async Task<IHtmlDocument> getPage(string targetUrl, HttpClient client, CancellationToken token)
        {
            try
            {
                var responseMessage = await client.GetAsync(targetUrl);
                var content = await responseMessage.Content.ReadAsStringAsync();
                var document = await parser.ParseDocumentAsync(content);
                return document;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при загрузке страницы {targetUrl}");
                cancelTokenSource.Cancel();
                return null;
            }
        }

        public async Task<IHtmlDocument> getPage(string targetUrl, User user, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine($"Загрузка пользователя {user.Login} отменена");
                return null;
            }
            try
            {
                var client = await getAuthorizedClient(user);
                var responseMessage = await client.GetAsync(targetUrl);
                var content = await responseMessage.Content.ReadAsStringAsync();
                var document = await parser.ParseDocumentAsync(content);
                return document;
            }
            catch
            {
                Console.WriteLine($"Ошибка при загрузке пользователя {user.Login}");
                cancelTokenSource.Cancel();
                return null;
            }
        }
        public void Print()
        {
            foreach (var discipline in disciplines)
            {
                discipline.Print();
            }
        }
        private async Task<HttpClient> getAuthorizedClient(User user)
        {

            var handler = new HttpClientHandler() { UseCookies = true };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Host", "dec.srspu.ru");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");


            var getRequest = await client.GetAsync(url);




            var document = parser.ParseDocument(await getRequest.Content.ReadAsStringAsync());

            var eventtarget = document.QuerySelector("input[id=__EVENTTARGET]").GetAttribute("value");
            var eventargument = document.QuerySelector("input[id=__EVENTARGUMENT]").GetAttribute("value");
            var viewstate = document.QuerySelector("input[id=__VIEWSTATE]").GetAttribute("value");
            var viewstategenerator = document.QuerySelector("input[id=__VIEWSTATEGENERATOR]").GetAttribute("value");
            var ctl00_ASPxMenu1 = @"{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;checkedState&quot;:&quot;&quot;}";
            var ctl00_ASPxNavBar1 = @"{&quot;selectedItemIndexPath&quot;:&quot;&quot;,&quot;groupsExpanding&quot;:&quot;1&quot;}";
            var ctl00_MainContent_ucLoginFormPage_tbUserName_State = @"{&quot;rawValue&quot;:&quot;" + user.Login + "&quot;,&quot;validationState&quot;:&quot;&quot;}";
            var ctl00_MainContent_ucLoginFormPage_tbUserName = user.Login;
            var ctl00_MainContent_ucLoginFormPage_tbPassword_State = @"{&quot;rawValue&quot;:&quot;" + user.Password + "&quot;,&quot;validationState&quot;:&quot;&quot;}";
            var ctl00_MainContent_ucLoginFormPage_tbPassword = user.Password;
            var ctl00_MainContent_ucLoginFormPage_btnLogin = "Вход";
            var DXScript = @"1_16,1_17,1_28,1_66,1_18,1_19,1_20,1_22,1_29,1_36,1_38,1_225,1_226,1_224";
            var DXCss = @"0_1884,1_69,1_70,1_71,0_1889,1_250,0_1780,1_251,0_2246,0_2253,0_1775,/css/Mail/plugins/font-awesome/css/font-awesome.min.css?t=1,https://fonts.googleapis.com/icon?family=Material+Icons,/css/Mail/dist/css/adminlte.min.css?t=2,/css/Main/animate.css?t=1,/css/Main/mainstyles.css?t=2";


            List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>() {
             new KeyValuePair<string, string>("__EVENTTARGET", eventtarget),
             new KeyValuePair<string, string>("__EVENTARGUMENT", eventargument),
             new KeyValuePair<string, string>("__VIEWSTATE", viewstate),
             new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", viewstategenerator),
             new KeyValuePair<string, string>("ctl00$ASPxMenu1", ctl00_ASPxMenu1),
             new KeyValuePair<string, string>("ctl00$ASPxNavBar1", ctl00_ASPxNavBar1),
             new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbUserName$State", ctl00_MainContent_ucLoginFormPage_tbUserName_State),
             new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbUserName", ctl00_MainContent_ucLoginFormPage_tbUserName),
             new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbPassword$State", ctl00_MainContent_ucLoginFormPage_tbPassword_State),
             new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbPassword", ctl00_MainContent_ucLoginFormPage_tbPassword),
             new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$btnLogin", ctl00_MainContent_ucLoginFormPage_btnLogin),
             new KeyValuePair<string, string>("DXScript", DXScript),
             new KeyValuePair<string, string>("DXCss", DXCss),
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(keyValues);
            var postRequest = await client.PostAsync(url, content);
            var doc = await parser.ParseDocumentAsync(await postRequest.Content.ReadAsStringAsync(), CancellationToken.None);
            if (doc.QuerySelector("span#ctl00_MainContent_ucLoginFormPage_lblError") != null) throw new Exception($"Неправильный логин или пароль ({user.Login})");
            return client;
        }


    }
}
