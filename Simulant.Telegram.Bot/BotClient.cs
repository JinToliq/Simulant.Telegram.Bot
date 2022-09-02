using System;
using System.Threading;
using System.Threading.Tasks;
using Simulant.Telegram.Bot.ErrorHandling;
using Simulant.Telegram.Bot.Logging;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using LogLevel = Simulant.Telegram.Bot.Logging.LogLevel;

namespace Simulant.Telegram.Bot
{
  public class BotClient
  {
    public event Action<Log>? Log
    {
      add => _log += value;
      remove => _log -= value;
    }

    public readonly TelegramBotClient Client;
    private CancellationTokenSource? _pollingCancellationTokenSource;
    private readonly UpdateHandler _updateHandler;
    private IErrorHandler? _errorHandler;
    private Action<Log>? _log;

    public BotClient(string token, UpdateHandler handler, Action<Log> onLog)
    {
      if (string.IsNullOrEmpty(token))
        throw new ArgumentNullException(nameof(token));

      ArgumentNullException.ThrowIfNull(handler);
      ArgumentNullException.ThrowIfNull(onLog);

      _log += onLog;
      Client = new(token);
      _updateHandler = handler;
      _updateHandler.Log += OnHandlerLog;
    }

    private void OnHandlerLog(Log value) => _log?.Invoke(value);

    public BotClient AddErrorHandler(IErrorHandler value)
    {
      ArgumentNullException.ThrowIfNull(value);

      _errorHandler = value;
      return this;
    }

    public void StartPolling(ReceiverOptions? receiverOptions = null)
    {
      if (_updateHandler is null)
        throw new InvalidOperationException("MessageHandler is null");

      _pollingCancellationTokenSource = new();
      receiverOptions ??= new();
      Client.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, _pollingCancellationTokenSource.Token);
      _log?.Invoke(new(LogLevel.Info, "Started polling"));
    }

    public void StopPolling()
    {
      _pollingCancellationTokenSource?.Cancel();
      _log?.Invoke(new(LogLevel.Info, "Stopped polling"));
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      if (_updateHandler is null)
        throw new InvalidOperationException("MessageHandler is null");

      await _updateHandler.Handle(botClient, update, cancellationToken);
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
      if (_errorHandler is null)
        _log?.Invoke(new(LogLevel.Error, $"An unhandled error has occured. Provide custom handler with {nameof(AddErrorHandler)}", exception));
      else
        await _errorHandler.HandleErrorAsync(botClient, exception, cancellationToken);
    }
  }
}