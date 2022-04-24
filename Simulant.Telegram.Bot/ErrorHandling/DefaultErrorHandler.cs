using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Simulant.Telegram.Bot.ErrorHandling
{
  public class DefaultErrorHandler : IErrorHandler
  {
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
      throw exception;
    }
  }
}