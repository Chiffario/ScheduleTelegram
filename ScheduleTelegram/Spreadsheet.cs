using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ScheduleTelegram;

namespace ScheduleTelegram
{
    public class Spreadsheet
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        public static void Schedule()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // Токен в JSON, нужен для авторизации и чтения
                // НЕ ТРОГАТЬ!
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Подключение АПИ, что-то чисто Гугловское
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            Dates ScheduleDate = new Dates();

            // Define request parameters.
            String spreadsheetId = "1aUfQruu8lYsUz-CBoy1X7S0Hpke5bGCDBjSBpG7RcQQ";
            String range = Dates.GetNeededDate() + "!A3:P10";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;


            if (values != null && values.Count > 0)
            {
                Console.WriteLine("Номер, Урок");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0} {1}", row[0], row[13]);
                    using (StreamWriter middleData = new("E:/Sirius.Severstal/db/data.txt", true))
                    {
                        middleData.Write("{0} {1}" + "\r\n", row[0], row[13]);
                    }

                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }


        }
    }
}
