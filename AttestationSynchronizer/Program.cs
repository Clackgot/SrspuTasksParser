using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AttestationSynchronizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region sdoLoader
            //List<User> users = new List<User>();
            //users.Add(new User("clackgot@gmail.com", "uVJ3e3Uf"));
            //users.Add(new User("spir.alex2017@yandex.ru", "xaY9r9YJ"));
            //users.Add(new User("paaa01@bk.ru", "mV33XxrP"));
            //users.Add(new User("dmitry.kurochkin66@gmail.com", "A9revK9R"));
            //users.Add(new User("samarkin20022002@gmail.com", "4jPn4tAK"));

            //SdoLoader loader = new SdoLoader(users);
            #endregion



            #region vedLoader

            List<User> users2 = new List<User>();
            users2.Add(new User("clackgot@gmail.com", "31160xs1"));
            users2.Add(new User("paaa01@bk.ru", "jt1s83q1"));
            users2.Add(new User("ouskornikov@mail.ru", "5ddjyii4"));
            users2.Add(new User("dmitry.kurochkin66@gmail.com", "74a5y2e3"));
            users2.Add(new User("samarkin20022002@gmail.com", "q54541c8"));

            DecLoader decLoader = new DecLoader(users2);

            #endregion






        }



    }
}
