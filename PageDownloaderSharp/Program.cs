using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PageDownloaderSharp
{
    partial class Program
    {

        public class Sdo
        {
            public HttpClient Client { get; private set; }
            public string MoodleSession { get; private set; }
            public string LoginToken { get; private set; }
            public string Url { get; set; }

            public HtmlParser Parser { get; private set; }
            public Sdo()
            {
                Url = "https://sdo.srspu.ru/login/index.php";
                Parser = new HtmlParser();
                var handler = new HttpClientHandler { UseCookies = true };
                Client = new HttpClient(handler);

                Client.BaseAddress = new Uri(Url);
                Client.DefaultRequestHeaders.Add("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:84.0) Gecko/20100101 Firefox/84.0");
                Client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            }
            public async Task RunGetAsync()
            {
                HttpResponseMessage responseMessage = await Client.GetAsync("https://sdo.srspu.ru/login/index.php");
                var html = await responseMessage.Content.ReadAsStringAsync();
                var document = Parser.ParseDocument(html);
                LoginToken = document.QuerySelector("input[name=logintoken]").GetAttribute("value");
                MoodleSession = responseMessage.Headers.GetValues("set-cookie").ToList()[0];
                MoodleSession = MoodleSession.Substring(14);
                MoodleSession = MoodleSession.Substring(0, MoodleSession.Length - 26);
                Console.WriteLine(MoodleSession);
                Console.WriteLine(LoginToken);
            }
            public async Task RunPostAsync()
            {
                //var stringContent = new StringContent("anchor=&logintoken=+" + LoginToken + "&username=clackgot%40gmail.com&password=uVJ3e3Uf");
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("anchor", ""),
                    new KeyValuePair<string, string>("logintoken", LoginToken),
                    new KeyValuePair<string, string>("username", "clackgot@gmail.com"),
                    new KeyValuePair<string, string>("password", "uVJ3e3Uf"),
                });
                Client.DefaultRequestHeaders.Add("Cookie", "MoodleSession=" + MoodleSession);


                //var baseAddress = new Uri(Url);
                //var cookieContainer = new CookieContainer();
                //using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
                //{
                //    var content = new FormUrlEncodedContent(new[]
                //    {
                //    new KeyValuePair<string, string>("anchor", ""),
                //    new KeyValuePair<string, string>("logintoken", LoginToken),
                //    new KeyValuePair<string, string>("username", "clackgot@gmail.com"),
                //    new KeyValuePair<string, string>("password", "uVJ3e3Uf"),
                //    });
                //    //cookieContainer.Add(baseAddress, new Cookie("MoodleSession", MoodleSession));
                //    var result = await client.PostAsync(baseAddress, content);
                //    result.EnsureSuccessStatusCode();
                //    //Console.WriteLine(result.Content.ReadAsStringAsync().GetAwaiter().GetResult());

                //}
                var result = await Client.PostAsync(Url, content);
                File.WriteAllText("index.html", result.Content.ReadAsStringAsync().GetAwaiter().GetResult());


                //HttpResponseMessage responseMessage = await Client.PostAsync("https://sdo.srspu.ru/login/index.php", stringContent);
                //var html = responseMessage.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                //var document = Parser.ParseDocument(html);
                //Console.WriteLine(document.TextContent);
                //Console.WriteLine("Сервер вернул код: " + responseMessage.StatusCode);
            }

        }


        static void Main(string[] args)
        {
            Sdo sdo = new Sdo();
            sdo.RunGetAsync().GetAwaiter().GetResult();
            sdo.RunPostAsync().GetAwaiter().GetResult();
        }

    }
}
