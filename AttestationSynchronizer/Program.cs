using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AttestationSynchronizer
{


    class UserConnect
    {
        private class FormModel
        {
            public string eventtarget = "";
            public string eventargument = "";
            public string viewstate = "";
            public string viewstategenerator = "";
            public string menu1 = "{\"selectedItemIndexPath\":\"\",\"checkedState\":\"\"}";
            public string navBar1 = "{\"selectedItemIndexPath\":\"\",\"groupsExpanding\":\"1\"}";
            public string tbUserNameState = "";
            public string tbUserName = "";
            public string tbPasswordState = "";
            public string tbPassword = "";
            public string btnLogin0 = "Вход";
            public string btnLogin1 = "";
            public string dxScript = "1_16,1_17,1_28,1_66,1_18,1_19,1_20,1_22,1_29,1_36,1_38,1_225,1_226,1_224";
            public string dxCss = "0_1884,1_69,1_70,1_71,0_1889,1_250,0_1780,1_251,0_2246,0_2253,0_1775,/css/Mail/plugins/font-awesome/css/font-awesome.min.css?t=1,https://fonts.googleapis.com/icon?family=Material+Icons,/css/Mail/dist/css/adminlte.min.css?t=2,/css/Main/animate.css?t=1,/css/Main/mainstyles.css?t=2";

            public FormModel(IHtmlDocument document, string username, string password)
            {
                eventtarget = document
                    .QuerySelector("input#__EVENTTARGET")
                    .GetAttribute("value");
                eventargument = document
                    .QuerySelector("input#__EVENTARGUMENT")
                    .GetAttribute("value");
                viewstate = document
                    .QuerySelector("input#__VIEWSTATE")
                    .GetAttribute("value");
                viewstategenerator = document
                    .QuerySelector("input#__VIEWSTATEGENERATOR")
                    .GetAttribute("value");
                viewstategenerator = document
                    .QuerySelector("input#__VIEWSTATEGENERATOR")
                    .GetAttribute("value");
                tbUserNameState = "{\"rawValue\":\"" + username + "\",\"validationState\":\"\"}";
                tbUserName = username;
                tbPasswordState = "{\"rawValue\":\"" + password + "\",\"validationState\":\"\"}";
                tbPassword = password;
            }
        }

        public Uri loginPageUrl = new Uri("https://dec.srspu.ru/Account/Login.aspx");//Страницы сайта
        FormModel formModel; // Данные форм страницы
        HttpClient client; //Клиент с помощью которого отправляются запросы
        HtmlParser parser; //Парсер

        private string username = "clackgot@gmail.com";
        string password = "31160xs1";

        public string Password
        {
            get { return password; }
            private set { password = value; }
        }
        public string Username
        {
            get { return username; }
            private set { username = value; }
        }


        private Cookie sessionId;
        private Cookie antiXsrf;

        private void initFormData()
        {
            var response = GetLoginPage().GetAwaiter().GetResult();
            var html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var document = parser.ParseDocument(html);//Создание документа для парсинга
            formModel = new FormModel(document, username, password);


            foreach (var item in response.Headers)
            {
                List<string> cookies = new List<string>();
                if (item.Key == "Set-Cookie")
                {

                    foreach (var it in item.Value)
                    {
                        client.DefaultRequestHeaders.Add(item.Key, it);

                        var cookieString = it.Split(new char[] { '=', ';' });
                        cookies.Add(cookieString[1]);
                    }
                    HttpClientHandler handler = new HttpClientHandler();
                    sessionId = new Cookie("ASP.NET_SessionId", cookies[0], "/", "dec.srspu.ru");
                    antiXsrf = new Cookie("__AntiXsrfToken", cookies[1], "/", "dec.srspu.ru");
                }
            }
        }
        public void initClient()
        {
            HttpClientHandler messageHandler = new HttpClientHandler();//Хэндлер клиента
            messageHandler.AllowAutoRedirect = true;//Разрешить редирект
            messageHandler.UseCookies = true;//Использовать куки
            client = new HttpClient(messageHandler);
            //Заголовки запросов по умолчанию
            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("Host", "dec.srspu.ru");
            client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0");
        }
        /// <summary>
        /// Конструктор класса подключения к dec.srspu.ru
        /// </summary>
        public UserConnect(string username, string password)
        {
            Username = username;
            Password = password;
            parser = new HtmlParser();
            initClient();//Инициализация клиента
            initFormData();//Инициализация данных форм страницы входа в учётную запись
            PostLoginPage().GetAwaiter().GetResult();//Подключение к учётной записи
        }
        public async Task<HttpResponseMessage> PostLoginPage()
        {
            //Content POST-запроса который включает в себя данные форм страницы
            FormUrlEncodedContent content = new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>("__EVENTTARGET", formModel.eventtarget),
                    new KeyValuePair<string, string>("__EVENTARGUMENT", formModel.eventargument),
                    new KeyValuePair<string, string>("__VIEWSTATE", formModel.viewstate),
                    new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", formModel.viewstategenerator),
                    new KeyValuePair<string, string>("ctl00$ASPxMenu1", formModel.menu1),
                    new KeyValuePair<string, string>("ASPxNavBar1", formModel.navBar1),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$btnLogin", formModel.btnLogin0),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbUserName$State", formModel.tbUserNameState),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbUserName", formModel.tbUserName),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbPassword$State", formModel.tbPasswordState),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbPassword", formModel.tbPassword),
                    new KeyValuePair<string, string>("DXScript", formModel.dxScript),
                    new KeyValuePair<string, string>("DXCss", formModel.dxCss),
            });
            var response = await client.PostAsync(loginPageUrl, content);
            //File.WriteAllText("index.html", response.Content.ReadAsStringAsync().GetAwaiter().GetResult(), Encoding.UTF8);
            //Console.WriteLine(response);
            var vedResponse = client.GetAsync("https://dec.srspu.ru/Ved/").GetAwaiter().GetResult();
            //File.WriteAllText("ved.html", vedResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult(), Encoding.UTF8);
            return response;
        }

        private async Task<HttpResponseMessage> GetLoginPage()
        {
            var response = await client.GetAsync(loginPageUrl);
            return response;
        }

        public IHtmlDocument GetPage(string url)
        {
            var response = client.GetAsync(url).GetAwaiter().GetResult();
            var html = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var document = parser.ParseDocument(html);//Создание документа для парсинга
            if (document.QuerySelector("#ctl00_MainContent_ucLoginFormPage_lblError") != null)
            {
                throw new Exception("Неправильный логин или пароль");
            }
            return document;
        }
        public async Task<IHtmlDocument> GetPageAsync(string url)
        {
            var response = await client.GetAsync(url);
            var html = await response.Content.ReadAsStringAsync();
            var document = parser.ParseDocument(html);//Создание документа для парсинга
            if (document.QuerySelector("#ctl00_MainContent_ucLoginFormPage_lblError") != null)
            {
                throw new Exception("Неправильный логин или пароль");
            }
            return document;
        }

    }

    class Program
    {
        private static void AsyncRequestions(UserConnect userConnect,int count)
        {
            List<Task<IHtmlDocument>> tasks = new List<Task<IHtmlDocument>>();

            for (int i = 0; i < count; i++)
                tasks.Add(userConnect.GetPageAsync("https://dec.srspu.ru/Ved/Ved.aspx?id=57400"));

            Task.WaitAll(tasks.ToArray());
        }

        private static void SyncRequestions(UserConnect userConnect, int count)
        {
            for (int i = 0; i < count; i++)
                userConnect.GetPage("https://dec.srspu.ru/Ved/Ved.aspx?id=57400");
        }

        public static void TestMethods(UserConnect userConnect, int countTest, int methodCount = 10)
        {
            Console.WriteLine($"Запущено {countTest} тестов для синхронного и асинхронного методов, которые выполнят по {methodCount} GET-запросов");
            System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
            double syncTimeSum = 0;
            double asyncTimeSum = 0;
            for (int i = 0; i < countTest; i++)
            {
                double syncTime;
                double asyncTime;
                myStopwatch.Start();
                SyncRequestions(userConnect, methodCount);
                myStopwatch.Stop();
                syncTime = myStopwatch.Elapsed.TotalSeconds;
                myStopwatch.Restart();
                AsyncRequestions(userConnect, methodCount);
                myStopwatch.Stop();
                asyncTime = myStopwatch.Elapsed.TotalSeconds;
                Console.WriteLine($"({i+1}/{countTest}). Синхронно: {syncTime} сек. Асинхронно: {asyncTime} сек.");
                syncTimeSum += syncTime;
                asyncTimeSum += asyncTime;
            }
            double avSync = syncTimeSum / countTest;
            double avAsync = asyncTimeSum / countTest;
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Среднее время синхронного метода: {avSync} сек.");
            Console.WriteLine($"Среднее время асинхронного метода: {avAsync} сек.");
            double delta = avSync - avAsync;
            Console.WriteLine($"Асинронный метод в среднем быстрее на {delta} сек.");


        }

        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<Test>();
            UserConnect userConnect = new UserConnect("clackgot@gmail.com", "31160xs1");
            Console.Write("Тестов:");
            var testCount = int.Parse(Console.ReadLine());
            Console.Write("Запросов:");
            var reqCount = int.Parse(Console.ReadLine());
            TestMethods(userConnect, testCount, reqCount);
            //Test test = new Test(3);
        }
    }
}
