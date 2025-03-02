using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot
{
  public class PollingBotClient : BotClientBase
  {
    private CancellationTokenSource? _pollingCancellationTokenSource;

    public PollingBotClient(string token, BotUpdateHandler handler, ILogger<PollingBotClient> logger) : base(token, handler, logger)
    { }

    public void StartPolling(ReceiverOptions? receiverOptions = null)
    {
      _pollingCancellationTokenSource = new();
      receiverOptions ??= new();
      Client.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, _pollingCancellationTokenSource.Token);
      Logger.LogInformation("Started polling");
    }

    public void StopPolling()
    {
      _pollingCancellationTokenSource?.Cancel();
      Logger.LogInformation("Stopped polling");
    }

    private async Task HandleUpdateAsync(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
    {
      await HandleUpdateAsync(update, cancellationToken);
    }

    private async Task HandleErrorAsync(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken)
    {
      await HandleErrorAsync(exception, cancellationToken);
    }

    public override void Dispose() => _pollingCancellationTokenSource?.Dispose();
  }
}