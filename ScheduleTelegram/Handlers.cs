using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using ScheduleTelegram;
using System.Linq;

namespace ScheduleTelegram
{
    public class Handlers
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage),
                //UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
                //UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery),
                //UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
                //_ => UnknownUpdateHandlerAsync(botClient, update)
            };

            

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {

            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;


            var action = message.Text.Split(' ').First() switch
            {
                "/today" => SendTodaySchedule(botClient, message),
                "/tomorrow" => SendTomorrowSchedule(botClient, message),
            };

            static async Task<Message> SendTodaySchedule(ITelegramBotClient botClient, Message message)
            {
                System.IO.File.Delete("today.txt");
                string text = Spreadsheet.Schedule("/today");

                new BotCommand();

                return await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: text,
                    parseMode: ParseMode.Markdown
                    );
            }

            static async Task<Message> SendTomorrowSchedule(ITelegramBotClient botClient, Message message)
            {
                System.IO.File.Delete("tomorrow.txt");
                string text = Spreadsheet.Schedule("/tomorrow");

                new BotCommand();

                return await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: text,
                    parseMode: ParseMode.Markdown
                    );
            }

        }
    }
}

