using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PageDownloaderSharp
{

    public class SdoControl
    {
        public string Login { get; set; } = "clackgot@gmail.com";
        public string Password { get; set; } = "uVJ3e3Uf";
        /// <summary>
        /// Осуществляет GET/POST/... запросы и сохраняет куки, 
        /// и настройки headers на протяжении своего существования
        /// </summary>
        public HttpClient Client;
        /// <summary>
        /// Куки получаемый при первом подключении
        /// </summary>
        public string MoodleSession { get; private set; }
        /// <summary>
        /// Токен, который достаётся из скрытого input'а  
        /// </summary>
        public string LoginToken { get; private set; }
        /// <summary>
        /// Базовая страница входа в платформу
        /// </summary>
        public string Url { get; private set; } = "https://sdo.srspu.ru/login/index.php";
        /// <summary>
        /// Парсер HTML кода
        /// </summary>
        public HtmlParser Parser;
        /// <summary>
        /// Конструктор основного класса приложения
        /// </summary>
        public SdoControl() : this("clackgot@gmail.com", "uVJ3e3Uf") { }
        public SdoControl(string login, string password)
        {
            Login = login;
            Password = password;
            Parser = new HtmlParser();
            var handler = new HttpClientHandler { UseCookies = true };
            Client = new HttpClient(handler);
            Console.WriteLine("Логин: " + Login);
            Console.WriteLine("Пароль: " + Password);
            ClientInit();
            StartUpInit().GetAwaiter().GetResult();
            LoginAsync().GetAwaiter().GetResult();
        }
        public void ClientInit()
        {
            Client.BaseAddress = new Uri(Url);
            Client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "sdo.srspu.ru");
            Client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            Client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            Client.DefaultRequestHeaders.Add("DNT", "1");
            Client.DefaultRequestHeaders.Add("Host", "sdo.srspu.ru");
            Client.DefaultRequestHeaders.Add("TE", "Trailers");
            Client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0");
        }
        /// <summary>
        /// Первоначальный GET запрос к платформе, 
        /// инициализирующий токен <see cref="SdoControl.LoginToken">LoginToken</see> и сессионную cookie <see cref="SdoControl.MoodleSession">MoodleSession</see>
        /// </summary>
        /// <returns></returns>
        public async Task StartUpInit()
        {
            HttpResponseMessage responseMessage = await Client.GetAsync(Url);//GET запрос, и его запись в переменную
            var html = await responseMessage.Content.ReadAsStringAsync();//Содержимое ответа(документ)
            var document = Parser.ParseDocument(html);//Создание документа для парсинга
            LoginToken = document.QuerySelector("input[name=logintoken]").GetAttribute("value");//Находим токен
            MoodleSession = responseMessage.Headers.GetValues("set-cookie").ToList()[0];//Из заголовка ответа вытаскиваем куки
            MoodleSession = MoodleSession.Substring(14);
            MoodleSession = MoodleSession.Substring(0, MoodleSession.Length - 26);//Обрезаем лишнее
            Console.WriteLine("MoodleSession: " + MoodleSession);
            Console.WriteLine("LoginToken: " + LoginToken);
        }
        /// <summary>
        /// POST запрос с проверкой на валидность логина пароля
        /// </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            ///Собираем содержимое тела POST запроса
            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("anchor", ""),
                    new KeyValuePair<string, string>("logintoken", LoginToken),
                    new KeyValuePair<string, string>("username", Login),
                    new KeyValuePair<string, string>("password", Password),
                });
            Client.DefaultRequestHeaders.Add("Cookie", "MoodleSession=" + MoodleSession);//Добавляем куки в заголовок будущих запросов

            var loginResponse = await Client.PostAsync(Url, content);//Получаем результат запроса

            var html = await loginResponse.Content.ReadAsStringAsync();//Содержимое ответа(документ)
            var document = Parser.ParseDocument(html);//Создание документа для парсинга

            var loginMsg = document.QuerySelector("a#loginerrormessage");

            if (loginMsg == null)
            {
                Console.WriteLine("Успешный вход");
            }
            else
            {
                throw new Exception("Неправильный логин или пароль");
            }
        }

        public AngleSharp.Html.Dom.IHtmlDocument GetPage(string url)
        {
            HttpResponseMessage responseMessage = Client.GetAsync(url).GetAwaiter().GetResult();//GET запрос, и его запись в переменную
            var html = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();//Содержимое ответа(документ)
            var document = Parser.ParseDocument(html);//Создание документа для парсинга
            return document;
        }

    }
}

