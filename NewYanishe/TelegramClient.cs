using Microsoft.EntityFrameworkCore;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Timer = System.Timers.Timer;

namespace NewYanishe
{
    public class TelegramClient : IDisposable
    {
        TelegramBotClient _botClient;
        CancellationTokenSource _cts;
        ReceiverOptions _receiverOptions;
        Timer _timer;
        object _lock = new object();
        bool isStarted = false;
        public TelegramClient()
        {
            _timer = new Timer(3.6e+6);
            _botClient = new TelegramBotClient("5531659641:AAGboVzSk2Af2SpUdwEm0cqel8VsWDVN4kE");
            _receiverOptions = new ReceiverOptions()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };
            _cts = new CancellationTokenSource();
        }

        public void Start()
        {
            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: _receiverOptions,
                cancellationToken: _cts.Token
                );
            Console.ReadLine();
        }

        public void Stop()
        {
            _cts.Cancel();
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var messageText = update.Message?.Text;
            var chatId = update.Message?.Chat.Id;
            if (!string.IsNullOrEmpty(messageText))
            {
                List<string?> messages = new List<string?>();
                if (messageText.Contains("Ян", StringComparison.OrdinalIgnoreCase))
                {
                    var sentBy = update.Message.From.Username;
                    switch (sentBy)
                    {
                        case "Bgdns":
                            messages = await GetMessagesByName("бодя", "Бодя");
                            break;
                        case "Afonichevds":
                            messages = await GetMessagesByName("денис", "Денис");
                            break;
                        case "Wpwiwysott":
                            messages = await GetMessagesByName("жека", "Жека");
                            break;
                        case "Gleib":
                            messages = await GetMessagesByName("глеб", "Глеб");
                            break;
                        default:
                            break;
                    }

                }
                else if (messageText.Contains("кочалк", StringComparison.OrdinalIgnoreCase)
                    || messageText.Contains("кончалк", StringComparison.OrdinalIgnoreCase)
                    || messageText.Contains("дроч", StringComparison.OrdinalIgnoreCase))
                {
                    using (var db = new MessageContext())
                    {
                        messages = await db.Messages
                                .Where(m => !string.IsNullOrEmpty(m.Text))
                                .Where(m =>
                                m.Text.Contains("Чел ")
                                || m.Text.Contains("чел ")
                                || m.Text.Contains("Пацаны ")
                                || m.Text.Contains("пацаны "))
                                .Select(m => m.Text)
                                .ToListAsync();
                    }
                }
                if (messages.Any())
                {
                    var randomIndex = new Random().Next(0, messages.Count() - 1);
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, messages[randomIndex], replyToMessageId: update.Message.MessageId);
                }

                if (messageText == @"/start@Yanishe_GeruchBot")
                {
                    if (isStarted)
                    {
                        await botClient.SendTextMessageAsync(chatId, "Я уже начал долбаеб");
                        return;
                    }

                    await botClient.SendTextMessageAsync(chatId, "Погнали ебать");
                    isStarted = true;
                    _timer.Elapsed += SendYansMessageEvent;
                    _timer.AutoReset = true;
                    _timer.Enabled = true;
                    _timer.Start();
                }
                else if (messageText == @"/stop@Yanishe_GeruchBot")
                {
                    if (!isStarted)
                    {
                        await botClient.SendTextMessageAsync(chatId, "Я уже закончил долбаеб");
                        return;
                    }

                    await botClient.SendTextMessageAsync(chatId, "Стоп ебать");
                    _timer.Stop();
                    isStarted = false;
                }

                async void SendYansMessageEvent(object source, ElapsedEventArgs e)
                {
                    Entities.Message randomMessageEntity;
                    using (MessageContext db = new MessageContext())
                    {
                        var randomIndex = new Random().Next(1, 84956);
                        randomMessageEntity = await db.Messages.FindAsync(randomIndex);
                    }
                    var messageText = randomMessageEntity.Text;
                    //var messageText = "1";
                    if (!string.IsNullOrEmpty(messageText))
                    {
                        try
                        {
                            await botClient.SendTextMessageAsync(update.Message.Chat.Id, messageText);
                        }
                        catch
                        {
                            await Task.Delay(10000);
                        }
                    }
                }
            }
        }

        private async Task<List<String>> GetMessagesByName(string lowercaseName, string uppercaseName)
        {
            using (var db = new MessageContext())
            {
                return await db.Messages
                        .Where(m => !string.IsNullOrEmpty(m.Text))
                        .Where(m => m.Text.Length > 5)
                        .Where(m => m.Text.Contains(uppercaseName) || m.Text.Contains(lowercaseName))
                        .Select(m => m.Text)
                        .ToListAsync();
            }
        }

        // Echo received message text
        //Message sentMessage = await botClient.SendTextMessageAsync("dffd",
        //    cancellationToken: cancellationToken)
        async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
        }

        public void Dispose()
        {
            _cts.Dispose();
        }
    }
}
