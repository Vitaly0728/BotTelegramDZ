using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.Configuration;

namespace BotTelegramDZ
{
    class Program
    {
        private static CancellationTokenSource _cts = new CancellationTokenSource();
        private static TelegramBotClient? _botClient;

        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
             .AddUserSecrets<Program>() 
             .Build();

            string token = configuration["TelegramBot:Token"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Ошибка: токен бота не установлен.");
                return; 
            }
            _botClient = new TelegramBotClient(token);

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[] { UpdateType.Message },
                DropPendingUpdates = true
            };

            var handler = new UpdateHandler();
            handler.OnHandleUpdateStarted += (message) =>
                Console.WriteLine($"Началась обработка сообщения '{message}'");
            handler.OnHandleUpdateCompleted += (message) =>
                Console.WriteLine($"Закончилась обработка сообщения '{message}'");

            _botClient.StartReceiving(handler, receiverOptions);

            var me = await _botClient.GetMe();
            Console.WriteLine($"{me.FirstName} запущен!");

            Console.WriteLine("Нажмите клавишу A для выхода");
            while (true)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.A)
                {
                    _cts.Cancel();
                    break;
                }
                else
                {
                    var botInfo = await _botClient.GetMe();
                    Console.WriteLine($"Информация о боте: {botInfo.FirstName} ({botInfo.Username})");
                }
            }
            
        }
    }
}




