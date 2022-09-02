using Simulant.Telegram.Bot;
using Simulant.Telegram.Bot.Logging;
using Simulant.Telegram.Bot.TestBot;

var token = File.ReadAllText("token.dat");
var client = new BotClient(token, new UpdateHandler().AddHandler<TestCommandHandler>(), OnLog);
client.StartPolling();

while (true)
{
  await Task.Delay(TimeSpan.FromSeconds(1));
}

void OnLog(Log log) => Console.WriteLine($"[{log.Level}] - {log.Message}{(log.Exception is null ? string.Empty : $" {log.Exception.Message}")}");