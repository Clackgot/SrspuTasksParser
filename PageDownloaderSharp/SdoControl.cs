using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace PageDownloaderSharp
{
    partial class Program
    {
        public class SdoControl
        {
            Encoding encoding = Encoding.UTF8;
            private IHtmlDocument document;//Текущий документ
            private HtmlParser parser;//AngleSharp парсер
            public string LoginToken { get; private set; }//Значение скрытого элемента logintoken на странице входа sdo
            public string Login { get; private set; }
            public string Password { get; private set; }
            public string MoodleSession { get; private set; }

            public string Url { get; set; }

            /// <summary>
            /// Заполняет запрос данными по умолчанию
            /// </summary>
            /// <param name="request">Запрос</param>
            /// <returns></returns>
            private WebRequest setRequsetData(WebRequest request)
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)request;
                httpWebRequest.Host = "sdo.srspu.ru";
                httpWebRequest.Method = "GET";
                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0";
                httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                httpWebRequest.Headers.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                return (WebRequest)httpWebRequest;
            }

            /// <summary>
            /// GET запрос к этому URL
            /// </summary>
            /// <param name="url">URL страницы, к которой нужно произвести GET запрос</param>
            /// <returns>Возвращает ответ сервера WebRequest</returns>
            private WebResponse GetRequest(string url)
            {
                WebRequest request = WebRequest.Create(url);
                request = setRequsetData(request);
                return request.GetResponse();

            }
            private WebResponse PostRequest(string url)
            {
                WebRequest request = WebRequest.Create(url);
                request = setRequsetData(request);
                request.Method = "POST";
                return request.GetResponse();

            }

            /// <summary>
            /// Get запросом получает HTML-код документа по заданному URL
            /// </summary>
            /// <param name="url">URL-страницы</param>
            /// <returns>HTML-код в виде строки string</returns>
            private string getHtmlCode(string url)
            {
                WebResponse response = GetRequest(url);
                var moodleSession = response.Headers.Get("set-cookie").Substring(14);
                MoodleSession = moodleSession.Substring(0, moodleSession.Length - 26);
                Stream stream = response.GetResponseStream();
                byte[] byteArray = new byte[0];
                int b;
                do
                {
                    b = stream.ReadByte();
                    if (b != -1)
                    {
                        Array.Resize(ref byteArray, byteArray.Length + 1);
                        byteArray[byteArray.Length - 1] = (byte)b;
                    }
                }
                while (b != -1);
                response.Close();
                stream.Close();
                return encoding.GetString(byteArray);
            }

            /// <summary>
            /// Инициализирует парсер AngleSharp 
            /// </summary>
            /// <returns></returns>
            private HtmlParser initParser()
            {
                return new HtmlParser();
            }

            /// <summary>
            /// Конструктор класса, удалённой работы с sdo.srspu.ru
            /// </summary>
            /// <param name="url"></param>
            public SdoControl(string login, string password)
            {
                Login = login;
                Password = password;
                Url = "https://sdo.srspu.ru/login/index.php";
                parser = initParser();//Инициализация парсера AngleSharp
                document = parser.ParseDocument(getHtmlCode(Url));//Инициализация IHtmlDocumnt HTML-кодом
                setLoginToken();
            }
            public SdoControl() : this("clackgot@gmail.com", "uVJ3e3Uf") { }

            /// <summary>
            /// Находит и устанавливает значение скрытого элемента страницы logintoken
            /// </summary>
            /// <returns></returns>
            private bool setLoginToken()
            {
                LoginToken = document.QuerySelector("input[name=logintoken]").GetAttribute("value");
                return LoginToken != "";
            }

            private bool connect()
            {
                WebRequest request = WebRequest.Create(Url);
                request = setRequsetData(request);
                CookieContainer cookieContainer = new CookieContainer();
                cookieContainer.Add(new Cookie("MoodleSession", MoodleSession));
                ((HttpWebRequest)request).CookieContainer = cookieContainer;

                string data = "anchor=&logintoken="+LoginToken+"&username=clackgot%40gmail.com&password=uVJ3e3Uf";
                byte[] byteArray = Encoding.UTF8.GetBytes(data);

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                return true;
                 
            }

        }
    }
}
