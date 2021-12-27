using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
                "/start" => SetupSystem(botClient, message)
            };
            // Task для получения расписания на сегодня
            static async Task<Message> SendTodaySchedule(ITelegramBotClient botClient, Message message)
            {
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
                string text = Spreadsheet.Schedule("/tomorrow");

                new BotCommand();

                return await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: text,
                    parseMode: ParseMode.Markdown
                    );
            }

            static async Task SetupSystem(ITelegramBotClient botClient, Message message)
            {
                await UserSetup.StatusSetup(botClient, message);
            }

        }
    }
}
