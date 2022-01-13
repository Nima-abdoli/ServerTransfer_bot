﻿using System;
using System.IO;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using System.Text;

namespace MyApp // Note: actual namespace depends on the project name.
{

    class Program
    {

        // guid that generated by bot_father bot in telegram. 
        static private string? BotGuid;
        // bot client that connect to telegram server.
        static TelegramBotClient? botClient;


        static void Main(string[] args)
        {
            // check for guid(token) exist in file or not.
            guidChecker();

            botClient = new TelegramBotClient(BotGuid);

            using CancellationTokenSource cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { } // receive all update types
            };

            // start bot 
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken: cts.Token);

            
            Console.WriteLine("Running ...");
            Console.ReadKey();
        }

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type != UpdateType.Message)
                return;
            // Only process text messages
            if (update.Message!.Type != MessageType.Text)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You said:\n" + messageText,
                cancellationToken: cancellationToken);
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        /// <summary>
        /// chack if guid file exit or not. if not exist make one and get guid from user.
        /// </summary>
        static void guidChecker()
        {
            if (System.IO.File.Exists("guid.txt"))
            {
                // read guid from text file.
                BotGuid = System.IO.File.ReadAllText("guid.txt");
            }
            else
            {
                // take guid from user and write it in text file.
                Console.WriteLine("Enter guid :");
                string? s = Console.ReadLine();
                BotGuid = s;
                System.IO.File.WriteAllText("guid.txt", s);
            }
        } // end of guidChacker

    }
}
