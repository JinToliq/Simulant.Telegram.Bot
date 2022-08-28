using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;

    public BotClient(string token)
    {
      Client = new (token);
      _errorHandler = new DefaultErrorHandler();
      _loggerFactory = LoggerFactory.Create(builder =>
      {
#if DEBUG
        builder.SetMinimumLevel(LogLevel.Debug);
#else
        builder.SetMinimumLevel(LogLevel.Info);
#endif
        builder.AddSimpleConsole(options =>
        {
          options.IncludeScopes = true;
          options.SingleLine = true;
          options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        });
      });

      _logger = _loggerFactory.CreateLogger(typeof(BotClient));
    }

    public BotClient WithUpdateHandler(UpdateHandler value)
    {
      ArgumentNullException.ThrowIfNull(value);
      _updateHandler = value;
      _updateHandler.Logger = _loggerFactory.CreateLogger(value.GetType());
      _logger.LogInformation("Set Update handler {Type}", value.GetType().Name);
      return this;
    }

    public BotClient WithErrorHandler(IErrorHandler value)
    {
      ArgumentNullException.ThrowIfNull(value);
      _errorHandler = value;
      _logger.LogInformation("Set Error handler {Type}", value.GetType().Name);
      return this;
    }

    public void StartPolling(ReceiverOptions? receiverOptions = null)
    {
      if (_updateHandler is null)
        throw new InvalidOperationException("MessageHandler is null");

      _pollingCancellationTokenSource = new CancellationTokenSource();
      receiverOptions ??= new ReceiverOptions();
      Client.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, _pollingCancellationTokenSource.Token);
      _logger.LogInformation("Started polling");
    }

    public void StopPolling()
    {
      _pollingCancellationTokenSource?.Cancel();
      _logger.LogInformation("Stopped polling");
    }

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