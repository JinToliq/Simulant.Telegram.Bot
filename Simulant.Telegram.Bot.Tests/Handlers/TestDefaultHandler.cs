using System.Threading;
using System.Threading.Tasks;
using Simulant.Telegram.Bot.CommandHandling;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Tests
{
  public class AlwaysHandledDefaultHandler : DefaultCommandHandler
  {
    public override Task<bool> Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      var mock = (TelegramBotClientMock)botClient;
      mock.Check2 = true;
      return Task.FromResult(true);
    }
  }

  public class AlwaysNotHandledDefaultHandler : DefaultCommandHandler
  {
    public override Task<bool> Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
      var mock = (TelegramBotClientMock)botClient;
      mock.Check2 = true;
      return Task.FromResult(false);
    }
  }
}