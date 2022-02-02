﻿using System;
using System.IO;
using System.Text;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerTransfer_bot // Note: actual namespace depends on the project name.
{
    class Program
    {
        #region global Property and variable

        // guid that generated by bot_father bot in telegram. 
        static private string? BotGuid;
        // bot client that connect to telegram server.
        static TelegramBotClient? botClient;
        // every update from user interact
        static Update mUpdate;
        static CancellationTokenSource Cts = new CancellationTokenSource();
        static CancellationToken canceltoken;

        static FileExplorer fx = new FileExplorer();

        #endregion

        #region Main(Start)

        static void Main(string[] args)
        {
            Console.WriteLine(fx.CurrentPath);

            // check for guid(token) exist in file or not.
            guidChecker();
            // Check for UserFile Exist
            UserFileCheck();

            // Create Telegram client. this object acted as bot itself, it mean every thing come to this object.
            // all messages, all files. 
            botClient = new TelegramBotClient(BotGuid);

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
                cancellationToken: Cts.Token);

            Console.WriteLine("Running ...");
            Console.ReadKey();
        }// End of main

        #endregion

        #region Update Handling

        static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            mUpdate = update;
            canceltoken = cancellationToken;

            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Type == UpdateType.Message)
            {
                if (UserCheck(update.Message.Chat.Username))
                {
                    // Only process text messages
                    if (update.Message!.Type == MessageType.Text)
                    {
                        var chatId = update.Message.Chat.Username;
                        var messageText = update.Message.Text;

                        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

                        CommandHandler(messageText);
                    }
                    //Only process documents(mostly all kind of file pdf,exe,txt,zip and ...)
                    else if (update.Message!.Type == MessageType.Document)
                    {
                        var chatIdf = update.Message.Chat.Id;

                        Message sentMessagef = await botClient.SendTextMessageAsync(
                        chatId: chatIdf,
                        text: "You send:\n" + update.Message.Document.FileName + " - " +
                            update.Message.Document.MimeType + " - " + "File",
                        cancellationToken: cancellationToken);

                        FileDownloader(update.Message.Document.FileName, update.Message.Document.FileId);
                    }

                }// UserCheck if
                else
                {
                    SendMessage("⛔⚠ You are not Authorize to access this Robot ⛔⚠");
                }
            }//end of Only message type Statement.

            // only process CallbackQuery Updates.
            else if (update.Type == UpdateType.CallbackQuery)
            {
                if (UserCheck(update.CallbackQuery.From.Username))
                {
                    if (update.CallbackQuery.Data == "back")
                    {
                        fx.BackinPath();
                    }
                }
                else
                {
                    SendMessage("⛔⚠ You are not Authorize to access this Robot ⛔⚠");
                }
            }// end of only Callback query type Statement.

            else
            {
                return;
            }
        }// end of Handle Update Async

        #endregion

        #region CommandHandling

        static async void CommandHandler(string command)
        {
            if (command == "/start")
            {
                SendMessage("Welcome to File Transfer Robot");
            }
            else if (command == "/getfile")
            {
                SendMessage("Getting File is Under Maintenance ..." + fx.GetFiles());
            }
            else if (command == "/sendfile")
            {
                SendMessage("Sending File is Under Maintenance ...");

                InlineKeyboardMarkup inlineKeyboard = new(new[]
                    {
                        // first row
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "Back", callbackData: "back"),
                            InlineKeyboardButton.WithCallbackData(text: "Select", callbackData: "Select"),
                        },
                        // second row
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(text: "Delete", callbackData: "del"),
                            InlineKeyboardButton.WithCallbackData(text: "Create", callbackData: "22"),
                        },
                    });

                Message sentMessage = await botClient.SendTextMessageAsync(
                        chatId: mUpdate.Message.Chat.Id,
                        text: "A message with an inline keyboard markup",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: canceltoken);

            }
            else if (command == "/explore")
            {
                SendMessage("File Exploration is Under Maintenance ...");
            }
            else
            {
                SendMessage("Use Proper Command");
            }
        }// end of CommandHandling

        /// <summary>
        /// send message to user
        /// </summary>
        /// <param name="message">text of message</param>
        static async void SendMessage(string message)
        {
            Message SentMessage = await botClient.SendTextMessageAsync(
                chatId : mUpdate.Message.Chat.Id,
                text : message,
                cancellationToken: canceltoken);
        }

        #endregion

        #region File Downloader

        /// <summary>
        /// download file that user send to bot in server side
        /// </summary>
        /// <param name="FileName">name of file that user send</param>
        /// <param name="FileId">a string that telegram give to each file uploaded to telegram server</param>
        static async void FileDownloader(string FileName,string FileId)
        {
            try
            {
                //Get file info from telegram server
                Telegram.Bot.Types.File file = await botClient.GetFileAsync(FileId);

                // downloaded file from telegram server and stream(save) it to file
                FileStream fs = new FileStream(FileName, FileMode.Create);

                // downloading file
                await botClient.DownloadFileAsync(file.FilePath, fs);

                //flush fileStream process.
                fs.Close();
                fs.Dispose();
            }
            catch (Exception e)
            {
                ErrorLog(e.Message);
            }
            
        }// end of FileDownloader.

        #endregion

        #region ErrorHandling and check

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            //Console.WriteLine(ErrorMessage);
            ErrorLog(ErrorMessage);
            return Task.CompletedTask;
        }

        /// <summary>
        /// check if guid file exit or not. if not exist make one and get guid from user.
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

        /// <summary>
        /// log all error into a file.
        /// </summary>
        /// <param name="str">error string</param>
        static void ErrorLog(string str)
        {
            System.IO.File.AppendAllText("log.txt", "# Log - " + DateTime.Now + "\n" + str + "\n-------------------------------------\n");
        }

        /// <summary>
        /// check if user that send message or file authorize to access this bot or not
        /// </summary>
        /// <param name="UserName">incoming username that send message</param>
        /// <returns>boolean to that authorize user or not</returns>
        static bool UserCheck(string UserName)
        {
            bool UserValidate = false;
            // check if user file that hold authorize user exist or not.
            UserFileCheck();
            
            // all user in file
            string[] users = System.IO.File.ReadAllLines("User.txt");

            foreach (string user in users)
            {
                if (UserName == user)
                {
                    UserValidate = true;
                }
                else
                {
                    UserValidate = false;
                }
            }

            return UserValidate;
        }// end of UserCheck

        /// <summary>
        /// Check if Users File exist or not. if file don't exist request from user to add username.
        /// </summary>
        static void UserFileCheck()
        {
            if (System.IO.File.Exists("User.txt") != true)
            {
                // make user file, and request username from user.
                MakeUserFile();
            }
        }// end of UserFileCheck

        /// <summary>
        /// make username file.
        /// </summary>
        static void MakeUserFile()
        {
            bool Counter = false;

            do
            {
                Console.WriteLine("Enter UserName you want to access the your Server : ");
                // get user name from user and add it to 'Usert.txt' File.
                System.IO.File.AppendAllText("User.txt", Console.ReadLine());
                Console.WriteLine("Do you Want add other user ?");
                Console.WriteLine(" y - Yes  | n - No");

                if (Console.ReadLine() == "y")
                {
                    Counter = true;
                }
                else
                {
                    Counter = false;
                }

            } while (Counter);
        }// End of Make User File

        #endregion

    }// End of Program Class
}// End of MyApp namespace
