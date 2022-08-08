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
            _timer = new Timer(5000);
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
            var chatId = update.Message.Chat.Id;
            if (!string.IsNullOrEmpty(messageText))
            {
                if (messageText.Contains("Ян", StringComparison.OrdinalIgnoreCase))
                {
                    using (var db = new MessageContext())
                    {
                        var sentBy = update.Message.From.Username;
                        List<string?> messages;
                        switch (sentBy)
                        {
                            case "Bgdns":
                                messages = await db.Messages
                                    .Where(m => string.IsNullOrEmpty(m.Text))
                                    .Where(m => m.Text.Contains("Бодя", StringComparison.OrdinalIgnoreCase))
                                    .Select(m => m.Text)
                                    .ToListAsync();
                                break;
                            case "Afonichevds":
                                messages = await db.Messages
                                    .Where(m => string.IsNullOrEmpty(m.Text))
                                    .Where(m => m.Text.Contains("Денис", StringComparison.OrdinalIgnoreCase))
                                    .Select(m => m.Text)
                                    .ToListAsync();
                                break;
                            case "Wpwiwysott":
                                messages = await db.Messages
                                    .Where(m => string.IsNullOrEmpty(m.Text))
                                    .Where(m => m.Text.Contains("Жека", StringComparison.OrdinalIgnoreCase))
                                    .Select(m => m.Text)
                                    .ToListAsync();
                                break;
                            case "Gleib":
                                messages = await db.Messages
                                    .Where(m => string.IsNullOrEmpty(m.Text))
                                    .Where(m => m.Text.Contains("Глеб", StringComparison.OrdinalIgnoreCase))
                                    .Select(m => m.Text)
                                    .ToListAsync();
                                break;
                            default:
                                break;
                        }
                    }
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
                if (messageText == @"/stop@Yanishe_GeruchBot")
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
                    //using (MessageContext db = new MessageContext())
                    //{
                    //    var randomIndex = new Random().Next(1, 84956);
                    //    //randomMessageEntity = await db.Messages.FindAsync(randomIndex);
                    //}
                    //var messageText = randomMessageEntity.Text;
                    var messageText = "1";
                    if (!string.IsNullOrEmpty(messageText))
                    {
                        //try
                        //{
                        await botClient.SendTextMessageAsync(update.Message.Chat.Id, messageText);
                        //}
                        //catch
                        //{
                        //    await Task.Delay(10000);
                        //}
                    }
                }
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
