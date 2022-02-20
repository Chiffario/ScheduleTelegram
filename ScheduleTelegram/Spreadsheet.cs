using System;
using System.Collections.Generic;

using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

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

        // Ключевой метод для всей этой штуки. Получаем данные с таблицы и сохраняем их в data.txt
        public static IList<IList<Object>> GetScheduleData(string commandText)
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

            var serializerOpt = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };


            Dates ScheduleDate = new();
            // Define request parameters.
            String spreadsheetId = "placeholder";
            String range = Dates.GetNeededDate(commandText) + "!B2:O10";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);
            request.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.COLUMNS;
            ValueRange response = request.Execute();            
            IList<IList<Object>> values = response.Values;
            return values;
        }
         public static void FormatInitialSchedule(IList<IList<Object>> values)
         {
            var serializerOpt = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };

            foreach (List<Object> subList in values)
            {

                string tempFileName = "grade" + (string)subList[0] + ".json";
                while (subList.Count < 9)
                {
                    subList.Add("");
                }
                Formats.LessonsReformatted lessons = new()
                {
                    Grade = (string)subList[0],
                    ClassOne = (string)subList[1],
                    ClassTwo = (string)subList[2],
                    ClassThree = (string)subList[3],
                    ClassFour = (string)subList[4],
                    ClassFive = (string)subList[5],
                    ClassSix = (string)subList[6],
                    ClassSeven = (string)subList[7],
                    ClassEight = (string)subList[8]
                };

                string jsonTest = JsonSerializer.Serialize<Formats.LessonsReformatted>(lessons, serializerOpt);
                System.IO.File.WriteAllText(tempFileName, jsonTest);
                ParseAndFix(tempFileName);
                RenameSubjects(tempFileName);
            }
        }

        /// <summary>
        /// Метод, заменяющий все неполноценные названия школьных предметов на полные их формы
        /// </summary>
        /// <param name="tempFilePath"> - Путь к файлу с расписанием</param>
        public static void RenameSubjects(string tempFilePath)
        {

            //fileName = CommandCheck(textMessage);

            StreamReader reader = new StreamReader(tempFilePath);
            string content = reader.ReadToEnd();
            reader.Close();

            string[] initialSubjectArray = new string[]
            {
                "(Английски)",
                "(Литер.чт)\b",
                "Матем",
                "(Окр мир)\b",
                "(Техн..)\b",
                "(Русский)\b",
                "(Литер.)\b",
                "(литер)",
                "(экономика)",
                "фин.гр.  ",
                "(эл.англ.яз)\b",
                "эл.англ,",
                "консультация [()]химия[()]",
                "(физика)",
                "(химия)",
                "(история)",
                @"^/n",
                "акт зал",
                "акт.зал"
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
                "Экономика",
                "Финансовая грамотность",
                "Английский язык (электив)",
                "Английский язык (электив)",
                "Консультация (химия)",
                "Физика",
                "Химия",
                "История",
                "Окно",
                "",
                ""
            };

            for (int i = 0; i < initialSubjectArray.Length; i++)
            {
                content = Regex.Replace(content, initialSubjectArray[i], replacementSubjectArray[i]);
            }

            StreamWriter writer = new StreamWriter(tempFilePath);
            writer.Write(content);
            writer.Close();
        }

        /// <summary>
        /// Метод, заменяющий лишние символы в тексте расписания с помощью Regex.Replace
        /// </summary>
        /// <param name="tempFileName"></param>
        public static void ParseAndFix(string tempFileName)
        {
            //string regexPattern = @"\s[Кк].*";
            string regexPattern = @"([Кк]аб.)\s?\w+";
            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            //string fileName = CommandCheck(textMessage);

            StreamReader reader = new StreamReader(tempFileName);
            string content = reader.ReadToEnd();
            reader.Close();

            content = Regex.Replace(content, regexPattern, " ");
            content = Regex.Replace(content, @"/", " ");
            content = Regex.Replace(content, @"\b\s{2,}\n", "");
            content = Regex.Replace(content, @"\s+$", "\n");
            content = Regex.Replace(content, @"\s+\n", "\n");
            content = Regex.Replace(content, @"\s{2,}\b", " ");
            content = Regex.Replace(content, @"\b\s{2,}", "");

            StreamWriter writer = new StreamWriter(tempFileName);
            writer.Write(content);
            writer.Close();
        }
        public static string Schedule(string textMessage)
        {
            var serializerOpt = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            //System.IO.File.Delete("today.txt");
            //System.IO.File.Delete("tomorrow.txt");

            var values = GetScheduleData(textMessage);
            FormatInitialSchedule(values);

            string messageReply;

            switch (textMessage)
            {
                case "/today":
                    using (StreamReader middleData = new("grade11.json", true))
                    {
                        string text = middleData.ReadToEnd();
                        Formats.LessonsReformatted lessons = JsonSerializer.Deserialize<Formats.LessonsReformatted>(text);
                        messageReply = ($"{lessons.ClassOne}\n{lessons.ClassTwo}\n{lessons.ClassThree}\n{lessons.ClassFour}\n{lessons.ClassFive}\n{lessons.ClassSix}\n{lessons.ClassSeven}\n{lessons.ClassEight}");

                    }
                    return messageReply;

                case "/tomorrow":
                    using (StreamReader middleData = new("grade11.json", true))
                    {
                        string text = middleData.ReadToEnd();
                        Formats.LessonsReformatted lessons = JsonSerializer.Deserialize<Formats.LessonsReformatted>(text);
                        messageReply = ($"{lessons.ClassOne}\n{lessons.ClassTwo}\n{lessons.ClassThree}\n{lessons.ClassFour}\n{lessons.ClassFive}\n{lessons.ClassSix}\n{lessons.ClassSeven}\n{lessons.ClassEight}");
                    }
                    return messageReply;
                default:
                    goto case "/today";
            }
        }
    }
}
