using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

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
        //private static async Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Message message)
        //{

        //}
        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {

            Console.WriteLine($"Receive message: {message.Text}");
            if (message.Type != MessageType.Text)
                return;


            var action = message.Text.Split(' ').First() switch
            {
                "/today" => TodayCommand(botClient, message),
                "/tomorrow" => TomorrowCommand(botClient, message),
                // "/start" => SetupSystem(botClient, message),
                //regexPattern => SendClassSchedule(botClient, message),
                string => SendClassSchedule(botClient, message),
                _ => SendErrorMessage(botClient, message)
            };
            // Task для получения расписания на сегодня
            static async Task TodayCommand(ITelegramBotClient botClient, Message message)
            {
                string text = Spreadsheet.Schedule("/today");

                new BotCommand();

                await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: "Выберите класс",
                    parseMode: ParseMode.Markdown
                    );
                //await SendClassSchedule(botClient, message);
            }

            static async Task TomorrowCommand(ITelegramBotClient botClient, Message message)
            {
                string text = Spreadsheet.Schedule("/tomorrow");

                new BotCommand();

                await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: "Выберите класс",
                    parseMode: ParseMode.Markdown
                    );
            }

            static async Task<Message> SendClassSchedule(ITelegramBotClient botClient, Message message)
            {
                string messageReply;
                
                string grade = message.Text.ToString();
                Console.WriteLine();
                using (StreamReader middleData = new($"grade{grade}.json", true))
                {
                    string text = middleData.ReadToEnd();
                    Formats.LessonsReformatted lessons = JsonSerializer.Deserialize<Formats.LessonsReformatted>(text);
                    messageReply = ($"{lessons.ClassOne}\n{lessons.ClassTwo}\n{lessons.ClassThree}\n{lessons.ClassFour}\n{lessons.ClassFive}\n{lessons.ClassSix}\n{lessons.ClassSeven}\n{lessons.ClassEight}");

                }
                return await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: messageReply,
                    parseMode: ParseMode.Markdown
                    );
            }
            static async Task<Message> SendErrorMessage(ITelegramBotClient botClient, Message message)
            {
                return await botClient.SendTextMessageAsync
                    (
                    chatId: message.Chat.Id,
                    text: "you fucked up, message: " + message.Text,
                    parseMode: ParseMode.Markdown
                    );
            }

            //static async Task<Message> Test(ITelegramBotClient botClient, Message message)
            //{
            //    await BotOnMessageReceived(botClient, message);

            //    return await botClient.SendTextMessageAsync
            //        (
            //        chatId: message.Chat.Id,
            //        text: message.Text,
            //        parseMode: ParseMode.Markdown
            //        );
            //}

        }
    }
}
