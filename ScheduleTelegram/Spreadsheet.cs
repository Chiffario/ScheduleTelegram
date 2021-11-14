using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        public static void GetScheduleData()
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
            String spreadsheetId = "1qn5WoQTrMtmCtkhz9Lrjy88ZnLFHM19dCha2WuBBZ4k";
            String range = Dates.GetNeededDate() + "!A3:P10";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            
            foreach (var row in values)
            {
                // Print columns A and E, which correspond to indices 0 and 4.
                Console.WriteLine("{0}", row[14]);
                using (StreamWriter messageData = new("data.txt", true))
                {
                    messageData.Write("{0}" + "\r\n", row[14]);
                }
            }
            



        }
        public static void RenameSubjects()
        {
            StreamReader reader = new StreamReader("data.txt");
            string content = reader.ReadToEnd();
            reader.Close();

            string[] initialSubjectArray = new string[] 
            { 
                "(Английски)", 
                "(Литер.чт)\b", 
                "(Матем)\b", 
                "(Окр мир)\b", 
                "(Техн..)\b", 
                "(Русский)\b",
                "(Литер.)\b",
                "(литер)",
                "(эл.англ.яз)\b",
                "консультация [()]химия[()]",
                "(физика)",
                "(химия).....",
                "(история)",
                @"^/n",
                "акт зал"
            };

            string[] replacementSubjectArray = new string[] 
            { 
                "Английский язык",
                "Литературное чтение",
                "Математика",
                "Окружающий мир",
                "Технология",
                "Русский язык",
                "Литература",
                "Литература (элективы)",
                "Английский язык (электив)",
                "Консультация (химия)",
                "Физика,",
                "Химия,",
                "История,",
                "Окно",
                ""
            };

            for (int i = 0; i < initialSubjectArray.Length; i++)
            {
                content = Regex.Replace(content, initialSubjectArray[i], replacementSubjectArray[i]);
            }

            StreamWriter writer = new StreamWriter("data.txt");
            writer.Write(content);
            writer.Close();


        }

        public static void ParseAndFix()
        {
            //string regexPattern = @"\s[Кк].*";
            string regexPattern = @"([Кк]аб.)\s?\w+\W?";
            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            StreamReader reader = new StreamReader("data.txt");
            string content = reader.ReadToEnd();
            reader.Close();
            
            content = Regex.Replace(content, regexPattern, " ");
            content = Regex.Replace(content, @"/", " ");
            //content = Regex.Replace(content, @"\b\s{2,}\n", "");
            content = Regex.Replace(content, @"\s+$", "\n");
            content = Regex.Replace(content, @"\s+\n", "\n");
            content = Regex.Replace(content, @"\s{2,}", " ");

            StreamWriter writer = new StreamWriter("data.txt");
            writer.Write(content);
            writer.Close();
        }
        public static void Schedule()
        {
            GetScheduleData();
            ParseAndFix();
            RenameSubjects();
        }
    }
}
