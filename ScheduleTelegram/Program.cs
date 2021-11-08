using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

namespace ScheduleTelegram
{
    class Program
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";
        static string NextSchoolDay;
        static string GetNeededDate()
        // Функция, определяющая следующий рабочий день
        {
            DateTime today = DateTime.Now;
            DateTime tomorrow;
            if (today.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime nextday = today.AddDays(2);
                tomorrow = nextday;
            }
            else
            {
                DateTime nextday = today.AddDays(1);
                tomorrow = nextday;
            }
            return NextSchoolDay = (string)tomorrow.ToString("dd.MM.yy");
        }

        //static void Main(string[] args)
        static void Schedule()
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


            // Define request parameters.
            String spreadsheetId = "1qn5WoQTrMtmCtkhz9Lrjy88ZnLFHM19dCha2WuBBZ4k";
            String range = GetNeededDate() + "!A3:P10";
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
        public static async Task Main()
        {
            var botClient = new TelegramBotClient("1998802934:AAETZNUZQZ1h_QB8yclMgXncAEfwchmArrM");
            var me = await botClient.GetMeAsync();
            Console.WriteLine(
              $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );
            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                Console.WriteLine(ErrorMessage);
                return Task.CompletedTask;
            }

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                if (update.Type != UpdateType.Message)
                    return;
                if (update.Message.Type != MessageType.Text)
                    return;

                var chatId = update.Message.Chat.Id;
                new BotCommand();
                Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");

                System.IO.File.Delete("E:/Sirius.Severstal/db/data.txt");
                Program.Schedule();
                string content;
                using (StreamReader middleData = new("E:/Sirius.Severstal/db/data.txt", true))
                {
                    content = middleData.ReadToEnd();
                }

                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: content,
                    parseMode: ParseMode.Markdown
                );
                System.IO.File.Delete("E:/Sirius.Severstal/db/data.txt");
            }
        }
    }
}
