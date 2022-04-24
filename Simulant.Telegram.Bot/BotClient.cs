using System;
using System.Threading;
using System.Threading.Tasks;
using Simulant.Telegram.Bot.ErrorHandling;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot
{
  public class BotClient
  {
    public readonly TelegramBotClient Client;
    private CancellationTokenSource? _pollingCancellationTokenSource;
    private UpdateHandler? _updateHandler;
    private IErrorHandler _errorHandler;

    public BotClient(string token)
    {
      Client = new (token);
      _errorHandler = new DefaultErrorHandler();
    }

    public BotClient WithUpdateHandler(UpdateHandler value)
    {
      ArgumentNullException.ThrowIfNull(value);
      _updateHandler = value;
      return this;
    }

    public BotClient WithErrorHandler(IErrorHandler value)
    {
      ArgumentNullException.ThrowIfNull(value);
      _errorHandler = value;
      return this;
    }

    public void StartPolling(ReceiverOptions? receiverOptions = null)
    {
      if (_updateHandler is null)
        throw new InvalidOperationException("MessageHandler is null");

      _pollingCancellationTokenSource = new CancellationTokenSource();
      receiverOptions ??= new ReceiverOptions();
      Client.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, _pollingCancellationTokenSource.Token);
    }

    public void StopPolling() => _pollingCancellationTokenSource?.Cancel();

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      if (_updateHandler is null)
        throw new InvalidOperationException("MessageHandler is null");

      await _updateHandler.Handle(botClient, update, cancellationToken);
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
      await _errorHandler.HandleErrorAsync(botClient, exception, cancellationToken);
    }
  }
}