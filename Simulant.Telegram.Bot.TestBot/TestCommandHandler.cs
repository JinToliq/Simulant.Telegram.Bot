using Simulant.Telegram.Bot.CommandHandling;
using Simulant.Telegram.Bot.CommandHandling.Attributes;
using Telegram.Bot;

namespace Simulant.Telegram.Bot.TestBot;

public class TestCommandHandler : CommandHandlerBase
{
  [Command("test")]
  public async Task Test()
  {
    await Client.SendTextMessageAsync(Update.Message!.Chat.Id, "Handled test message");
  }
}