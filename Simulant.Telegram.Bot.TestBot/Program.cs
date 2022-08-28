using Simulant.Telegram.Bot;
using Simulant.Telegram.Bot.TestBot;

var token = string.Empty;
var client = new BotClient(token);
client.WithUpdateHandler(new UpdateHandler().AddTransients<TestCommandHandler>());
client.StartPolling();

while (true)
{
  await Task.Delay(TimeSpan.FromSeconds(1));
}