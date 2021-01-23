using AttestationSynchronizer;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GoogleSheetsControl
{
    class GSControl
    {
        private readonly string applicationName; //Имя приложения
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets }; //Права пользователя
        private SheetsService sheetsService; //Сервис Google Sheets
        public bool needClearWhenWriteed = true;

        DecLoader loader;
        /// <summary>
        /// Конструктор класса Google Sheets Control
        /// </summary>
        /// <param name="applicationName">Имя приложения, которое будет работать с таблицами</param>
        public GSControl(string applicationName)
        {
            this.applicationName = applicationName;
            initSheetsService();
            //clearSheetSubject("1bJFruzClSZcpuoUWXxOfJGJQ-l3Dt0DWXA4GHJDrgDw", 30);
            //PostSheetSubjectUsers("1bJFruzClSZcpuoUWXxOfJGJQ-l3Dt0DWXA4GHJDrgDw", new List<List<string>>() { 
            //new List<string>(){"qwe", "привет", "sfsdf12" },
            //new List<string>(){"231", "43", "434" },
            //new List<string>(){"8678", "zxc", "43xx4" },
            //new List<string>(){"11111", "321", "123" },

            //}, "Математика");

        }
        /// <summary>
        /// Возвращает таблицу по заданному ID
        /// </summary>
        /// <param name="sheetId">ID таблицы</param>
        /// <returns></returns>
        public Spreadsheet GetSheet(string sheetId)
        {
            bool includeGridData = true;  // TODO: Update placeholder value.
            SpreadsheetsResource.GetRequest request = sheetsService.Spreadsheets.Get(sheetId);
            request.IncludeGridData = includeGridData;
            var spreadsheet = request.Execute();
            return spreadsheet;
        }


        public void SendData(string spreadSheetId, List<List<string>> data, string sheetName)
        {
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;  // Значение не будет парситься перед записью
            ValueRange valueRange = new ValueRange();// Данные для отправки в таблицу
            valueRange.MajorDimension = "ROWS";//ROWS / COLUMNS
            var tempData = new List<IList<object>>(); //Подготавлемые данные для отправки в таблицу
            Random rnd = new Random();

            foreach (var row in data)
            {
                List<object> tempRow = new List<object>();
                foreach (var cell in row)
                {
                    tempRow.Add(cell);
                }
                tempData.Add(tempRow);
            }

            valueRange.Values = tempData;//Добавление подготовленных данных для Update запроса

            SpreadsheetsResource.ValuesResource.UpdateRequest request = sheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"{sheetName}!A3:U");//Доступная область редактирования
            request.ValueInputOption = valueInputOption;//Опция ввода (на самом деле хз что это)
            UpdateValuesResponse response = request.Execute(); //Запрос
            Console.WriteLine($"Обновлён лист {sheetName}");
        }



        private void clearSheetSubject(string sheetId, int lines)
        {
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;  // TODO: Update placeholder value.


            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";//"ROWS";//COLUMNS
            var tempData = new List<IList<object>>(lines);

            var tempList = new List<object>();
            for (int i = 0; i < 21; i++)
            {
                tempList.Add("");
            }
            for (int i = 0; i < lines; i++)
            {
                tempData.Add(tempList);
            }
            valueRange.Values = tempData;
            SpreadsheetsResource.ValuesResource.UpdateRequest request = sheetsService.Spreadsheets.Values.Update(valueRange, sheetId, $"Шаблон!A3:U");//Доступная область редактирования
            request.ValueInputOption = valueInputOption;//Опция ввода (на самом деле хз что это)
            UpdateValuesResponse response = request.Execute(); //Запрос
        }

        /// <summary>
        /// Конструктор класса Google Sheets Control с названием приложения по умолчанию
        /// </summary>
        public GSControl() : this("Quickstart") { }

        /// <summary>
        /// Инициализация сервиса Google Sheets
        /// </summary>
        private void initSheetsService()
        {
            sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = getCredential(),
                ApplicationName = applicationName,
            });
        }
        /// <summary>
        /// Возвращает учётные данные, полученные из JSON-файла credentials.json
        /// </summary>
        /// <returns></returns>
        private UserCredential getCredential()
        {
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }



    }


    class Storage
    {
        public string spreadsheatId = "1bJFruzClSZcpuoUWXxOfJGJQ-l3Dt0DWXA4GHJDrgDw";//ID таблицы
        GSControl control; //Получение и сохранение таблиц Google Sheets
        DecLoader loader;

        private void initDecLoader()
        {
            List<User> users = new List<User>()
        {
            new User("clackgot@gmail.com", "31160xs1"),
            new User("samarkin20022002@gmail.com", "q54541c8"),
            new User("dmitry.kurochkin66@gmail.com", "74a5y2e3"),
            new User("ouskornikov@mail.ru", "5ddjyii4"),
            new User("paaa01@bk.ru", "jt1s83q1"),
            new User("nivadan@inbox.ru", "72s87eli"),
        };
            loader = new DecLoader(users);
        }
        public Storage()
        {
            control = new GSControl();
            initDecLoader();
            foreach (var discipline in loader.disciplines)
            {
                control.SendData(spreadsheatId, discipline.GetData(), discipline.Name);
            }

        }
    }





    class Program
    {
        static void Main(string[] args)
        {
            //GSControl control = new GSControl();
            Storage storage = new Storage();
        }
    }
}
