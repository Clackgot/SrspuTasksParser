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


    class Loader
    {
        public User User { get; set; }
        public Loader(User user)
        {
            User = user;
            var document = getDocument().GetAwaiter().GetResult();
            Console.WriteLine(document.Source.Text);
            File.WriteAllText("index.html", document.Source.Text);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        private async Task<IDocument> getDocument()
        {
            Configuration config = new Configuration();
            config.WithDefaultCookies();
            config.WithDefaultLoader();

            var context = BrowsingContext.New(config);

            var queryDocument = await context.OpenAsync("https://sdo.srspu.ru/login/index.php");
            var form = queryDocument.QuerySelector<IHtmlFormElement>("form#login");
            Dictionary<string, string> fields = new Dictionary<string, string>();
            var username = "clackgot@gmail.com";
            var password = "uVJ3e3Uf";
            var anchor = "";
            var logintoken = form.QuerySelector("input[name=logintoken]").GetAttribute("value");

            DocumentRequest request = new DocumentRequest(new Url("https://sdo.srspu.ru/login/index.php"));
            request.Method = AngleSharp.Io.HttpMethod.Post;
            
            string body = $"anchor=&logintoken={logintoken}&username={username}&password={password}";
            request.Body = GenerateStreamFromString(body);
            var postResult = await context.OpenAsync(request);

            //var resultDocument = await form.SubmitAsync(fields);
            //Console.WriteLine(postResult.Cookie);
            return queryDocument;
        }


    }


    class RestLoader
    {
        private HtmlParser parser = new HtmlParser();
        private readonly string url = "https://sdo.srspu.ru/login/index.php";
        List<User> users = new List<User>();
        public RestLoader()
        {
            users.Add(new User("clackgot@gmail.com", "uVJ3e3Uf"));
            users.Add(new User("spir.alex2017@yandex.ru", "xaY9r9YJ"));
            users.Add(new User("paaa01@bk.ru", "mV33XxrP"));
            users.Add(new User("dmitry.kurochkin66@gmail.com", "A9revK9R"));
            //users.Add(new User("clackgot@gmail.com", "uVJ3e3Uf"));
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
    public class Program
    {




        public static void Main(string[] args)
        {

            RestLoader loader = new RestLoader();
           
        }



    }
}
