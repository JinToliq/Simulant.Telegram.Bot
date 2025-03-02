using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Simulant.Telegram.Bot.ErrorHandling;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot;

public abstract class BotClientBase : IDisposable
{
  public readonly TelegramBotClient Client;
  protected readonly BotUpdateHandler UpdateHandler;
  protected readonly ILogger Logger;
  private IErrorHandler? _errorHandler;

  protected BotClientBase(string token, BotUpdateHandler handler, ILogger logger)
  {
    ArgumentException.ThrowIfNullOrEmpty(token);
    ArgumentNullException.ThrowIfNull(handler);
    ArgumentNullException.ThrowIfNull(logger);

    Client = new(token);
    UpdateHandler = handler;
    Logger = logger;
  }

  public BotClientBase WithErrorHandler(IErrorHandler value)
  {
    ArgumentNullException.ThrowIfNull(value);

    _errorHandler = value;
    return this;
  }

  protected async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
  {
    await UpdateHandler.HandleAsync(Client, update, cancellationToken);
  }

  protected async Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
  {
    if (_errorHandler is null)
      Logger.LogError(exception, "An error has occured");
    else
      await _errorHandler.HandleErrorAsync(Client, exception, cancellationToken);
  }

  public virtual void Dispose()
  {
    UpdateHandler.Dispose();
    Logger.LogInformation("BotClient Disposed");
  }
}