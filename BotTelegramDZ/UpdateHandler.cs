using System.Net.Http.Json;
using Telegram.Bot;
using Newtonsoft.Json;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


namespace BotTelegramDZ
{
    internal class UpdateHandler : IUpdateHandler
    {
        public event Action<string> OnHandleUpdateStarted;
        public event Action<string> OnHandleUpdateCompleted;
        private static readonly HttpClient client = new HttpClient();
        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                OnHandleUpdateStarted?.Invoke(update.Message.Text);
                switch (update.Message.Text)
                {
                    case "/start":

                        await botClient.SendMessage(update.Message.Chat.Id, """
                         <b><u>Меню команд</u></b>:
                        /cat - Получить факт о кошках                
                        """, parseMode: ParseMode.Html, linkPreviewOptions: true,
                            replyMarkup: new ReplyKeyboardRemove());

                        break;
                    case "/cat":

                        CatFactDto catFact = await GetRandomCatFactAsync(cancellationToken);
                        string translatedFact = await TranslateTextAsync(catFact.Fact, "ru");
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, translatedFact);

                        break;

                    default: await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Сообщение успешно принято"); break;
                }               

                OnHandleUpdateCompleted?.Invoke(update.Message.Text);
            }
        }        

        // Факты о кошках и подкрутил перевод фактов.
        private async Task<string> TranslateTextAsync(string text, string targetLanguage)
        {
            var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair=en|{targetLanguage}";

            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            dynamic result = JsonConvert.DeserializeObject(response);
            return result.responseData.translatedText;
        }
        private async Task<CatFactDto> GetRandomCatFactAsync(CancellationToken cancellationToken)
        {
            using var client = new HttpClient();
            return await client.GetFromJsonAsync<CatFactDto>("https://catfact.ninja/fact", cancellationToken);
        } 
        
    }
    public record CatFactDto(string Fact, int Length);
}
