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
using ScheduleTelegram;

namespace ScheduleTelegram
{
    class Program
    {
        
        public static async Task Main()
        {
            var botClient = new TelegramBotClient("1998802934:AAETZNUZQZ1h_QB8yclMgXncAEfwchmArrM");
            var me = await botClient.GetMeAsync();
            Console.Title = me.Username;
            using var cts = new CancellationTokenSource();



            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            botClient.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, cancellationToken: cts.Token);


            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            //Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            //{
            //    var ErrorMessage = exception switch
            //    {
            //        ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            //        _ => exception.ToString()
            //    };

            //    Console.WriteLine(ErrorMessage);
            //    return Task.CompletedTask;
            //}
            //Spreadsheet Sheets = new Spreadsheet();

            //async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            //{

            //    if (update.Type != UpdateType.Message)
            //        return;
            //    if (update.Message.Type != MessageType.Text)
            //        return;

            //    var chatId = update.Message.Chat.Id;
            //    new BotCommand();
            //    Console.WriteLine($"Received a '{update.Message.Text}' message in chat {chatId}.");

            //    System.IO.File.Delete("data.txt");
            //    Spreadsheet.Schedule();
            //    string content;
            //    using (StreamReader middleData = new("data.txt", true))
            //    {
            //        content = middleData.ReadToEnd();
            //    }

            //    await botClient.SendTextMessageAsync(
            //        chatId: chatId,
            //        text: content,
            //        parseMode: ParseMode.Markdown
            //    );
            //}
        }
    }
}
