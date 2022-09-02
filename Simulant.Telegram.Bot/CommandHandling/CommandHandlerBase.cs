using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;

#pragma warning disable 8618
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Simulant.Telegram.Bot.CommandHandling
{
  public abstract class CommandHandlerBase
  {
    protected ITelegramBotClient Client { get; private set; }
    protected Update Update { get; private set; }
    protected User? From { get; private set; }
    protected Chat? Chat { get; private set; }
    protected string? Text { get; private set; }
    protected CancellationToken CancellationToken { get; private set; }

    internal void Initialize(ITelegramBotClient client, Update update, string? text, CancellationToken token)
    {
      Client = client;
      Update = update;
      From = update.Message?.From;
      Chat = update.Message?.Chat;
      Text = text;
      CancellationToken = token;
    }
  }
}