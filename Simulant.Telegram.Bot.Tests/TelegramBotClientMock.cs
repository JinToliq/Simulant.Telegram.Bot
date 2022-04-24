using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;

namespace Simulant.Telegram.Bot.Tests
{
  public class TelegramBotClientMock : ITelegramBotClient
  {
    public bool Check1;
    public bool Check2;
    public string? Text;

    public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken())
    {
      throw new NotImplementedException();
    }

    public Task<bool> TestApiAsync(CancellationToken cancellationToken = new CancellationToken())
    {
      throw new NotImplementedException();
    }

    public Task DownloadFileAsync(string filePath, Stream destination,
      CancellationToken cancellationToken = new CancellationToken())
    {
      throw new NotImplementedException();
    }

    public long? BotId { get; }
    public TimeSpan Timeout { get; set; }
    public IExceptionParser ExceptionsParser { get; set; }
    public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;
  }
}