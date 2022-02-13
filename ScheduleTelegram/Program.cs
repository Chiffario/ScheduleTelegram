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
            var botClient = new TelegramBotClient("placeholder_bot_token");
            var me = await botClient.GetMeAsync();
            Console.Title = me.Username;
            using var cts = new CancellationTokenSource();



            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            botClient.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, cancellationToken: cts.Token);


            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();

            
        }
    }
}
