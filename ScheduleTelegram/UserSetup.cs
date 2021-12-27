using System;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using System.Linq;
public class UserSetup
{
    public static async Task StatusSetup(ITelegramBotClient botClient, Message message)
    {
        ReplyKeyboardMarkup kbMarkup = new(new[]
        {
            new KeyboardButton[] { "Учитель", "Ученик" },
            new KeyboardButton[] { "Классный руководитель", "Ира" },
        })
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Выберите, кто вы",
                    replyMarkup: kbMarkup);
    }
    //public static async Task GradeSetup(ITelegramBotClient botClient, Message message)
    //{
    //    Message sentMessage = await botClient.SendTextMessageAsync(
    //                chatId: message.Chat.Id,
    //                text: "Напишите свой класс. Если у номера класса есть буква, напишите класс и букву без пробелов, например \"6а\"",
    //                parseMode: ParseMode.Markdown);

    //    await 
    //}
}
