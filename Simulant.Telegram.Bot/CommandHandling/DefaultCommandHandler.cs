using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.CommandHandling
{
  public abstract class DefaultCommandHandler
  {
    public abstract Task<bool> Handle(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
  }
}