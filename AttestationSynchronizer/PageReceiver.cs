using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AttestationSynchronizer
{
    public class PageReceiver{
        public string Login { get; set; } = "clackgot@gmail.com";
        public string Password { get; set; } = "31160xs1";
        /// <summary>
        /// Осуществляет GET/POST/... запросы и сохраняет куки, 
        /// и настройки headers на протяжении своего существования
        /// </summary>
        private HttpClient Client;
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
        public string Url { get; private set; } = "https://dec.srspu.ru/Account/Login.aspx";
        /// <summary>
        /// Парсер HTML кода
        /// </summary>
        private HtmlParser Parser;
        /// <summary>
        /// Конструктор основного класса приложения
        /// </summary>
        public PageReceiver() : this("clackgot@gmail.com", "31160xs1") { }
        public PageReceiver(string login, string password)
        {
            Login = login;
            Password = password;
            Parser = new HtmlParser();
            //var handler = new HttpClientHandler { UseCookies = true };
            Client = new HttpClient(new HttpClientHandler {UseCookies = true });
            Console.WriteLine("Логин: " + Login);
            Console.WriteLine("Пароль: " + Password);
            ClientInit();
            //StartUpInit().GetAwaiter().GetResult();
            //LoginAsync().GetAwaiter().GetResult();
            FirstRequest().GetAwaiter().GetResult();//GET https://dec.srspu.ru/Account/Login.aspx
            FirstRequest().GetAwaiter().GetResult();//GET https://dec.srspu.ru/Account/Login.aspx
        }

        private async Task FirstRequest()
        {
            HttpResponseMessage responseMessage = await Client.GetAsync("https://dec.srspu.ru/Account/Login.aspx");//GET запрос, и его запись в переменную
            var html = await responseMessage.Content.ReadAsStringAsync();//Содержимое ответа(документ)
            var document = Parser.ParseDocument(html);//Создание документа для парсинга
            
            Console.WriteLine($"\n\t\tGET запрос https://dec.srspu.ru/Account/Login.aspx");
            foreach (var item in Client.DefaultRequestHeaders)
            {
                Console.Write($"{item.Key} : ");
                foreach (var it in item.Value)
                {
                    Console.Write($"{it} ");
                }
                Console.WriteLine();
            }

            Console.WriteLine($"\n" +
                $"\t\tGET ответ https://dec.srspu.ru/Account/Login.aspx");
            foreach (var item in responseMessage.Headers)
            {
                Console.Write($"{item.Key} : ");
                foreach (var it in item.Value)
                {
                    Console.Write($"{it} ");
                }
                if(item.Key == "Set-Cookie")
                {
                    Client.DefaultRequestHeaders.Add("Cookie", item.Value);
                }
                Console.WriteLine();
            }
        }

        private void ClientInit()
        {
            Client.BaseAddress = new Uri(Url);
            Client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            Client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            Client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            Client.DefaultRequestHeaders.Add("DNT", "1");
            Client.DefaultRequestHeaders.Add("Host", "dec.srspu.ru");
            Client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0");
        }
        /// <summary>
        /// Первоначальный GET запрос к платформе, 
        /// инициализирующий токен <see cref="SdoControl.LoginToken">LoginToken</see> и сессионную cookie <see cref="SdoControl.MoodleSession">MoodleSession</see>
        /// </summary>
        /// <returns></returns>
        private async Task StartUpInit()
        {
            HttpResponseMessage responseMessage = await Client.GetAsync(Url);//GET запрос, и его запись в переменную
            var html = await responseMessage.Content.ReadAsStringAsync();//Содержимое ответа(документ)
            var document = Parser.ParseDocument(html);//Создание документа для парсинга
            Console.WriteLine($"\t\tGET {Url}");
            foreach (var item in responseMessage.Headers)
            {
                Console.Write($"{item.Key} : ");
                foreach (var it in item.Value)
                {
                    Console.Write($"{it} ");
                }
                Console.WriteLine();
            }

            responseMessage = await Client.GetAsync(Url);//GET запрос, и его запись в переменную
            html = await responseMessage.Content.ReadAsStringAsync();//Содержимое ответа(документ)
            Console.WriteLine("----------GET /----------");
            foreach (var item in responseMessage.Headers)
            {
                Console.Write($"{item.Key} : ");
                foreach (var it in item.Value)
                {
                    Console.Write($"{it} ");
                }
                Console.WriteLine();
            }

        }
        /// <summary>
        /// POST запрос с проверкой на валидность логина пароля
        /// </summary>
        /// <returns></returns>
        private async Task LoginAsync()
        {   
            ///Собираем содержимое тела POST запроса
            var content = new FormUrlEncodedContent(new[]
            {
                    //new KeyValuePair<string, string>("__EVENTTARGET", ""),
                    //new KeyValuePair<string, string>("__EVENTARGUMENT", ""),
                    //new KeyValuePair<string, string>("__VIEWSTATE", "/wEPDwULLTE5Njc0MjQ0ODAPZBYCZg9kFgICBQ9kFgwCAg88KwAKAgAPFgIeDl8hVXNlVmlld1N0YXRlZ2QGD2QQFgFmFgE8KwAMAQAWBh4EVGV4dAUS0J/QvtGA0YLRhNC+0LvQuNC+HgtOYXZpZ2F0ZVVybAUYfi9Qb3J0Zm9saW8vRGVmYXVsdC5hc3B4Hg5SdW50aW1lQ3JlYXRlZGdkZAILDw8WAh8CBRIvd2ViYXBwLyMvbWFpbC9hbGxkZAINDxYCHglpbm5lcmh0bWwFKdCt0LvQtdC60YLRgNC+0L3QvdC+0LUg0L/QvtGA0YLRhNC+0LvQuNC+ZAIPDxYCHgdWaXNpYmxlZxYCAgEPDxYEHwIFHC9XZWJBcHAvIy9QZXJzb25hbEthYi8tMTEwMTAfAQU00JvQldCU0J3QldCSINCV0JLQk9CV0J3QmNCZINCS0JvQkNCU0JjQnNCY0KDQntCS0JjQp2RkAhEPPCsACQIADxYCHwBnZAYPZBAWAWYWATwrAAsCABYKHwFlHgROYW1lBQJnaB8CZR4GVGFyZ2V0ZR8DZwEPZBAWB2YCAQICAgMCBAIFAgYWBxQrAAIWCh8BBQzQntGG0LXQvdC60LgfBgUDZ2kwHwIFBC9WZWQfB2UfA2cUKwACFgIeA1VybAUSZmEtcGVuY2lsLXNxdWFyZS1vZBQrAAIWCh8BBRTQoNCw0YHQv9C40YHQsNC90LjQtR8GBQNnaTEfAgUZaHR0cDovL3NjaGVkdWxlLm5waS10dS5ydR8HZR8DZxQrAAIWAh8IBQ1mYS1saW5lLWNoYXJ0ZBQrAAIWCh8BBQrQn9C70LDQvdGLHwYFA2dpMh8CBQYvUGxhbnMfB2UfA2cUKwACFgIfCAUOZmEtbmV3c3BhcGVyLW9kFCsAAhYKHwEFFNCh0YLQsNGC0LjRgdGC0LjQutCwHwYFA2dpMx8CBQkvc3RhdHZlZC8fB2UfA2cUKwACFgIfCAUMZmEtYmFyLWNoYXJ0ZBQrAAIWCh8BBRrQl9Cw0LTQvtC70LbQtdC90L3QvtGB0YLQuB8GBQNnaTQfAgUSL1N0YXQvRGVidG9ycy5hc3B4HwdlHwNnFCsAAhYCHwgFDmZhLWhhY2tlci1uZXdzZBQrAAIWCh8BBRvQodCy0L7QtNC90YvQtSDQvtGG0LXQvdC60LgfBgUDZ2k1HwIFFC9Ub3RhbHMvRGVmYXVsdC5hc3B4HwdlHwNnFCsAAhYCHwgFCmZhLWxlYW5wdWJkFCsAAhYKHwEFMNCe0LHRhdC+0LTQvdC+0Lkg0JvQuNGB0YIgKNCh0L7RgtGA0YPQtNC90LjQutC4KR8GBQNnaTYfAgUeaHR0cHM6Ly9kZWMuc3JzcHUucnUvb2J4b2RsaXN0HwdlHwNnFCsAAhYCHwgFB2ZhLWJvb2tkZGRkAhkPPCsABAEADxYCHgVWYWx1ZQUdMjAyMSDCqSBDb3B5cmlnaHQgYnkgTU1JUyBMYWJkZBgCBR5fX0NvbnRyb2xzUmVxdWlyZVBvc3RCYWNrS2V5X18WBQUPY3RsMDAkQVNQeE1lbnUxBSZjdGwwMCRMb2dpblZpZXcxJEhlYWRMb2dpblN0YXR1cyRjdGwwMQUmY3RsMDAkTG9naW5WaWV3MSRIZWFkTG9naW5TdGF0dXMkY3RsMDMFEWN0bDAwJEFTUHhOYXZCYXIxBSpjdGwwMCRNYWluQ29udGVudCR1Y0xvZ2luRm9ybVBhZ2UkYnRuTG9naW4FEGN0bDAwJExvZ2luVmlldzEPD2QCAWSz4l5qMQV+CGZaCMbZIRdGNt8jBzQRvsUJK0baP07U8g=="),
                    new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", "CD85D8D2"),
                    //new KeyValuePair<string, string>("ctl00$ASPxMenu1", "{\"selectedItemIndexPath\":\"\",\"checkedState\":\"\"}"),
                    //new KeyValuePair<string, string>("ASPxNavBar1", "{\"selectedItemIndexPath\":\"\",\"groupsExpanding\":\"1\"}"),
                    //new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbUserName$State", "{\"rawValue\":\"" + Login + "\",\"validationState\":\"\"}"),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbUserName", Login),
                    //new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbPassword$State", "{\"rawValue\":\"" + Password + "\",\"validationState\":\"\"}"),
                    new KeyValuePair<string, string>("ctl00$MainContent$ucLoginFormPage$tbPassword", Password),

                    //new KeyValuePair<string, string>("DXScript", "1_16,1_17,1_28,1_66,1_18,1_19,1_20,1_22,1_29,1_36,1_38,1_225,1_226,1_224"),
                    //new KeyValuePair<string, string>("DXCss", "\"0_1884,1_69,1_70,1_71,0_1889,1_250,0_1780,1_251,0_2246,0_2253,0_1775,/css/Mail/plugins/font-awesome/css/font-awesome.min.css?t=1,https://fonts.googleapis.com/icon?family=Material+Icons,/css/Mail/dist/css/adminlte.min.css?t=2,/css/Main/animate.css?t=1,/css/Main/mainstyles.css?t=2\""),
                });
            //Client.DefaultRequestHeaders.Add("Cookie", "MoodleSession=" + MoodleSession);//Добавляем куки в заголовок будущих запросов

            var loginResponse = await Client.PostAsync(Url, content);//Получаем результат запроса

            var html = await loginResponse.Content.ReadAsStringAsync();//Содержимое ответа(документ)
            var document = Parser.ParseDocument(html);//Создание документа для парсинга

            Console.WriteLine("\t\tPOST ");
            foreach (var item in loginResponse.Headers)
            {
                Console.Write($"{item.Key} : ");
                foreach (var it in item.Value)
                {
                    Console.Write($"{it} ");
                }
                Console.WriteLine();
            }


            File.WriteAllText("index.html", html, System.Text.Encoding.UTF8);
        }

        public AngleSharp.Html.Dom.IHtmlDocument GetPage(string url)
        {
            HttpResponseMessage responseMessage = Client.GetAsync(url).GetAwaiter().GetResult();//GET запрос, и его запись в переменную
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var html = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();//Содержимое ответа(документ)
                var document = Parser.ParseDocument(html);//Создание документа для парсинга
                return document;
            }
            else if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Ошибка 404. Ресурс не существует");
            }
            return null;
        }

        public void SavePage(string url)
        {
            HttpResponseMessage responseMessage = Client.GetAsync(url).GetAwaiter().GetResult();//GET запрос, и его запись в переменную
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var html = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();//Содержимое ответа(документ)
                var path = @"index.html";

                File.WriteAllText(path, html);
            }
            else if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new Exception("Ошибка 404. Ресурс не существует");
            }
        }


    }
}
