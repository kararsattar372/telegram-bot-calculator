using System;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace CalculatorBot
{
    class Program
    {
        private static TelegramBotClient? Bot;

        public static async Task Main()
        {
            Bot = new TelegramBotClient("5050633462:AAGxMWopoHGK5IY3LEllztiMNE6dUr2LMwY");

            User me = await Bot.GetMeAsync();
            Console.Title = me.Username ?? "CalculatorBot";
            using var cts = new CancellationTokenSource();

            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };
            Bot.StartReceiving(HandleUpdateAsync,
                               HandleErrorAsync,
                               receiverOptions,
                               cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();

            cts.Cancel();
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
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


            var action = message.Text switch
            {
                "/help" or "/start" => help(botClient, message),
                _ => calculator(botClient, message)
            };
            Message sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");


            static async Task<Message> help(ITelegramBotClient botClient, Message message)
            {


                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: 
                                                                  "/help - Get help\n" +
                                                                  "Type the calculation you want to perform.\n" +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b",
                                                            replyMarkup: new ReplyKeyboardRemove());
            }



            static async Task<Message> calculator(ITelegramBotClient botClient, Message message)
            {
                char[] chars = { 'A', 'a', 'B', 'b', 'C', 'c',
                       'D', 'd', 'E', 'e', 'F', 'f', 'G', 'g', 'H', 'h', 'I', 'i', 'J', 'j', 'K',
                        'k', 'L', 'l', 'M', 'm', 'N', 'n', 'O', 'o', 'P', 'p', 'Q', 'q', 'R', 'r',
                         'S', 's', 'T', 't', 'U', 'u', 'V', 'v', 'Y', 'y', 'Z', 'z',
                            '!', '@', '#', '$', '%', '^', '&', '(', ')', '_', '=',
                            '!', '?', '>', '<', '"', ':', '}', '{', '؟', '_', '-', '=',';'};
                char x = 'c';
                int temp = message.Text.IndexOfAny(chars);
                int temp2 = message.Text.IndexOf(x);
                Console.WriteLine($"temp: {temp}");
                Console.WriteLine($"temp2: {temp2}");



                if (temp == -1)
                {
                    if (message.Text.Contains("+") &&
                        message.Text.Split("+").Length == 2 &&
                        !message.Text.Contains("*") &&
                        !message.Text.Contains("/") &&
                        !message.Text.Contains("-"))
                    {
                        string pernum = perNumber(message.Text, "+");
                        string subnum = subNumber(message.Text, "+");
                        if (pernum != null && subnum != null)
                        {
                            float Fpernum = (float)Convert.ToDouble(pernum);
                            float Fsubnum = (float)Convert.ToDouble(subnum);
                            float res = Fpernum + Fsubnum;
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: pernum + " + " + subnum + "=" + res);
                        }
                        else
                        {
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                               text: "There was an error typing the calculation.\n " +
                                                                     "The calculation should be of the following form:\n" +
                                                                     "a + b | a - b | a*b | a/b");
                        }

                    }
                    else if (message.Text.Contains("-") &&
                        message.Text.Split("-").Length == 2 &&
                        !message.Text.Contains("*") &&
                        !message.Text.Contains("/") &&
                        !message.Text.Contains("+"))
                    {
                        string pernum = perNumber(message.Text, "-");
                        string subnum = subNumber(message.Text, "-");
                        if (pernum != null && subnum != null)
                        {
                            float Fpernum = (float)Convert.ToDouble(pernum);
                            float Fsubnum = (float)Convert.ToDouble(subnum);
                            float res = Fpernum - Fsubnum;
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: pernum + " - " + subnum + "=" + res);
                        }
                        else
                        {
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "There was an error typing the calculation.\n " +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b");
                        }

                    }
                    else if (message.Text.Contains("*") &&
                        message.Text.Split("*").Length == 2 &&
                        !message.Text.Contains("-") &&
                        !message.Text.Contains("/") &&
                        !message.Text.Contains("+"))
                    {
                        string pernum = perNumber(message.Text, "*");
                        string subnum = subNumber(message.Text, "*");
                        if (pernum != null && subnum != null)
                        {
                            float Fpernum = (float)Convert.ToDouble(pernum);
                            float Fsubnum = (float)Convert.ToDouble(subnum);
                            float res = Fpernum * Fsubnum;
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: pernum + " * " + subnum + "=" + res);
                        }
                        else
                        {
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "There was an error typing the calculation.\n " +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b");
                        }
                    }
                    else if (message.Text.Contains("/") &&
                        message.Text.Split("/").Length == 2 &&
                        !message.Text.Contains("-") &&
                        !message.Text.Contains("*") &&
                        !message.Text.Contains("+"))
                    {
                        string pernum = perNumber(message.Text, "/");
                        string subnum = subNumber(message.Text, "/");
                        if (pernum != null && subnum != null)
                        {
                            float Fpernum = (float)Convert.ToDouble(pernum);
                            float Fsubnum = (float)Convert.ToDouble(subnum);
                            float res = Fpernum / Fsubnum;
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                                text: pernum + " / " + subnum + "=" + res);
                        }
                        else
                        {
                            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "There was an error typing the calculation.\n " +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b");
                        }
                    }
                    else
                    {
                        return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "There was an error typing the calculation.\n " +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b");
                    }


                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "There was an error typing the calculation.\n " +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b"
                                                            );
                }


                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "There was an error typing the calculation.\n " +
                                                                  "The calculation should be of the following form:\n" +
                                                                  "a + b | a - b | a*b | a/b"
                                                            );
            }
        }


        public static string perNumber(string strSource, string sign)
        {
            try
            {
                string pre = strSource.Split(sign, StringSplitOptions.RemoveEmptyEntries)[0];

                return pre;
            }
            catch
            {
                return null;
            }

        }
        public static string subNumber(string strSource, string sign)
        {
            try
            {
                string sub = strSource.Split(sign, StringSplitOptions.RemoveEmptyEntries)[1];
                return sub;
            }
            catch
            {
                return null;
            }

        }


        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }



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

    }
}
