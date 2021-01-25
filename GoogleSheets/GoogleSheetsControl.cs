using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecLoader;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;

namespace GoogleSheets
{
    public class GoogleSheetsControl
    {
        private readonly string applicationName; //Имя приложения
        private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets }; //Права пользователя
        private SheetsService sheetsService; //Сервис Google Sheets
        public bool needClearWhenWriteed = true;

        /// <summary>
        /// Конструктор класса Google Sheets Control
        /// </summary>
        /// <param name="applicationName">Имя приложения, которое будет работать с таблицами</param>
        public GoogleSheetsControl(string applicationName)
        {
            this.applicationName = applicationName;
            initSheetsService();
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

        public async Task SendDisciplineInfo(string spreadSheetId, Discipline discipline, string sheetName)
        {
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;  // Значение не будет парситься перед записью
            ValueRange valueRange = new ValueRange();// Данные для отправки в таблицу
            valueRange.MajorDimension = "COLUMNS";
            var tempData = new List<IList<object>>(); //Подготавлемые данные для отправки в таблицу
            tempData.Add(new List<object>() {
                discipline.Status,
                discipline.ExamType,
                discipline.Group,
                discipline.Department,
                discipline.Name,
                discipline.Teacher,
                discipline.Course,
                discipline.SaveCount,
            });
            valueRange.Values = tempData;//Добавление подготовленных данных для Update запроса
            SpreadsheetsResource.ValuesResource.UpdateRequest request = sheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"{sheetName}!W3:W11");//Доступная область редактирования
            request.ValueInputOption = valueInputOption;//Опция ввода (на самом деле хз что это)
            UpdateValuesResponse response = await request.ExecuteAsync(); //Запрос
        }

        public async Task SendData(string spreadSheetId, Discipline discipline, string sheetName)
        {
            await clearSheetSubject(spreadSheetId, sheetName, 30);
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;  // Значение не будет парситься перед записью
            ValueRange valueRange = new ValueRange();// Данные для отправки в таблицу
            valueRange.MajorDimension = "ROWS";//ROWS / COLUMNS
            var tempData = new List<IList<object>>(); //Подготавлемые данные для отправки в таблицу

            foreach (var row in discipline.GetData())
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
            UpdateValuesResponse response = await request.ExecuteAsync(); //Запрос
            await SendDisciplineInfo(spreadSheetId, discipline, discipline.Name);

            Console.WriteLine($"Обновлён лист {sheetName}");
        }



        private async Task clearSheetSubject(string sheetId, string sheetName, int lines)
        {
            SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum valueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;  // TODO: Update placeholder value.


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
            SpreadsheetsResource.ValuesResource.UpdateRequest request = sheetsService.Spreadsheets.Values.Update(valueRange, sheetId, $"{sheetName}!A3:U");//Доступная область редактирования
            request.ValueInputOption = valueInputOption;//Опция ввода (на самом деле хз что это)
            UpdateValuesResponse response = await request.ExecuteAsync(); //Запрос
        }

        /// <summary>
        /// Конструктор класса Google Sheets Control с названием приложения по умолчанию
        /// </summary>
        public GoogleSheetsControl() : this("SrspuTaskParser") { }

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
}
