using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Tests
{
  public class TelegramBotClientMock : ITelegramBotClient
  {
    public bool Check1;
    public bool Check2;
    public string? Text;
    public string? Response;

    public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task<bool> TestApi(CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task DownloadFile(string filePath, Stream destination, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public Task DownloadFile(TGFile file, Stream destination, CancellationToken cancellationToken = new CancellationToken()) => throw new NotImplementedException();

    public bool LocalBotServer => throw new NotImplementedException();

    public long BotId => throw new NotImplementedException();

    public TimeSpan Timeout
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public IExceptionParser ExceptionsParser
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public event AsyncEventHandler<ApiRequestEventArgs> OnMakingApiRequest;
    public event AsyncEventHandler<ApiResponseEventArgs> OnApiResponseReceived;
  }
}