using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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


    class SdoLoader
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

    
    class Discipline
    {
        private HtmlParser parser = new HtmlParser();
        private IHtmlDocument document;
        private string status;
        private string type;
        private string group;
        private string department;
        private string name;
        private string teacher;
        private string course;
        private string year;
        private string hours;
        private string semester;
        private string dateUpdate;
        private string computer;
        private string saveCount;

        public string Status { get => status; private set => status = value; }
        public string Type { get => type; private set => type = value; }
        public string Group { get => group; private set => group = value; }
        public string Department { get => department; private set => department = value; }
        public string Name { get => name; private set => name = value; }
        public string Teacher { get => teacher; private set => teacher = value; }
        public string Course { get => course; private set => course = value; }
        public string Year { get => year; private set => year = value; }
        public string Hours { get => hours; private set => hours = value; }
        public string Semester { get => semester; private set => semester = value; }
        public string DateUpdate { get => dateUpdate; private set => dateUpdate = value; }
        public string Computer { get => computer; private set => computer = value; }
        public string SaveCount { get => saveCount; private set => saveCount = value; }

        private void parse()
        {
            Status = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblStatus").TextContent;
            Type = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblTypeVed").TextContent;
            Group = document.QuerySelector("a#ctl00_MainContent_ucVedBox_lblGroup").TextContent;
            Department = document.QuerySelector("a#ctl00_MainContent_ucVedBox_lblKafName").TextContent;
            Name = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblDis").TextContent;
            Teacher = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblPrep").TextContent;
            Course = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblKurs").TextContent;
            Year = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblYear").TextContent;
            Hours = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblHours").TextContent;
            Semester = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblSem").TextContent;
            DateUpdate = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblDateUpdate").TextContent;
            Computer = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblLastUser").TextContent;
            SaveCount = document.QuerySelector("span#ctl00_MainContent_ucVedBox_lblSaveCount").TextContent;
        }

        public Discipline(IHtmlDocument document)
        {
            this.document = document;
            parse();
        }
    }


    class DecLoader
    {
        private List<User> users;
        private HtmlParser parser = new HtmlParser();
        private readonly string url = "https://dec.srspu.ru/Account/Login.aspx";
        private List<Discipline> disciplines = new List<Discipline>();

        public DecLoader(List<User> users)
        {
            this.users = users;


            //initDisciplines().Wait();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource(new TimeSpan(100000000));
            CancellationToken token = cancelTokenSource.Token;

            List<Task<IHtmlDocument>> tasks = new List<Task<IHtmlDocument>>();
            
            foreach (var user in users)
            {
                tasks.Add(getPage("https://dec.srspu.ru/Ved/Ved.aspx?id=57392", user, token));   
            }


            Task.WaitAll(tasks.ToArray());
            
        }

        private async Task initDisciplines()
        {
            //var document = getPage("https://dec.srspu.ru/Ved/?group=6254", users[0]) ;
            //var tags = document.QuerySelectorAll("a.dxeHyperlink_MaterialCompact");
            //foreach (var tag in tags)
            //{
            //    string url = @"https://dec.srspu.ru/Ved/" + tag.GetAttribute("href");
            //    disciplines.Add(new Discipline(await getPage(url, users[0])));
            //}
        }


        public async Task<IHtmlDocument> getPage(string targetUrl, User user, CancellationToken token)
        {
            try
            {
                var client = await getAuthorizedClient(user);
                var responseMessage = await client.GetAsync(targetUrl);
                var content = await responseMessage.Content.ReadAsStringAsync();
                var document = await parser.ParseDocumentAsync(content);
                Console.WriteLine($"У {document.QuerySelector("span#ctl00_LoginView1_HeadLoginName").TextContent} {document.QuerySelectorAll("tr#ctl00_MainContent_ucVedBox_TableVed_DXDataRow0 td")[16].TextContent} балл(ов) по дискретке");
                return document;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при загрузке пользователя");
                
                return null;
            }
            
            

            if (token.IsCancellationRequested)
            {
                Console.WriteLine($"Загрузка страницы пользователя {user.Login} отменена");
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
            var ctl00_MainContent_ucLoginFormPage_tbUserName_State = @"{&quot;rawValue&quot;:&quot;"+user.Login+"&quot;,&quot;validationState&quot;:&quot;&quot;}";
            var ctl00_MainContent_ucLoginFormPage_tbUserName = user.Login;
            var ctl00_MainContent_ucLoginFormPage_tbPassword_State = @"{&quot;rawValue&quot;:&quot;"+user.Password+"&quot;,&quot;validationState&quot;:&quot;&quot;}";
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
            return client;
        }


    }
    public class Program
    {




        public static void Main(string[] args)
        {

            List<User> users = new List<User>();
            users.Add(new User("clackgot@gmail.com", "uVJ3e3Uf"));
            users.Add(new User("spir.alex2017@yandex.ru", "xaY9r9YJ"));
            users.Add(new User("paaa01@bk.ru", "mV33XxrP"));
            users.Add(new User("dmitry.kurochkin66@gmail.com", "A9revK9R"));
            users.Add(new User("samarkin20022002@gmail.com", "4jPn4tAK"));

            SdoLoader loader = new SdoLoader(users);

            List<User> users2 = new List<User>();
            users2.Add(new User("clackgot@gmail.com", "31160xs1"));
            users2.Add(new User("ouskornikov@mail.ru", "5ddjyii4"));
            users2.Add(new User("dmitry.kurochkin66@gmail.com", "74a5y2e3"));
            //users.Add(new User("samarkin20022002@gmail.com", "4jPn4tAK"));

            DecLoader decLoader = new DecLoader(users2);

        }



    }
}
