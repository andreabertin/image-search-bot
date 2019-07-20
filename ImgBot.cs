using System;
using System.IO;
using ImageSearchBot.Config;
using ImageSearchBot.ImageSearch;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ImageSearchBot
{
    public class ImgBot
    {
        private const int MaxImages = 500;
        
        private readonly TelegramBotClient _bot;
        private readonly User _me;
        private readonly BotConfig _config;
        private readonly IImageSearch _imageSearch;
        private readonly Random _random;

        public ImgBot(BotConfig config, IImageSearch imageSearch)
        {
            _random = new Random();
            
            _config = config;
            _imageSearch = imageSearch;
            _bot = new TelegramBotClient(Environment.GetEnvironmentVariable($"{_config.Prefix}_TELEGRAM_KEY"));
            
            _me = _bot.GetMeAsync().Result;
            
            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            _bot.OnInlineQuery += BotOnInlineQueryReceived;
            _bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            _bot.OnReceiveError += BotOnReceiveError;
        }

        private void BotOnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            
        }

        private void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs e)
        {
            
        }

        private void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs e)
        {
            
        }

        private void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            
        }

        private async void BotOnMessageReceived(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            var isPrivate = e.Message.Chat.Type == ChatType.Private;

            if (isPrivate || message.Text.Contains($"@{_me.Username}"))
            {
                // TODO: configurable keywords
                if (message.Text.ToLower().Contains("photo"))
                {
                    var image = _imageSearch.GetImage(_random.Next(MaxImages), MaxImages);

                    using (var memoryStream = new MemoryStream(image))
                    {
                        await _bot.SendPhotoAsync(
                            message.Chat.Id,
                            memoryStream,
                            _config.Responses[_random.Next(_config.Responses.Count)]);
                    }
                }
                else
                {
                    await _bot.SendTextMessageAsync(message.Chat.Id,
                        _config.GreetingsMessages[_random.Next(_config.GreetingsMessages.Count)]);
                }
            }
        }

        public void Run()
        {
            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"@{_me.Username}: started");
        }

        public void Stop()
        {
            Console.WriteLine($"@{_me.Username}: stopped");
            _bot.StopReceiving();
        }
    }
}