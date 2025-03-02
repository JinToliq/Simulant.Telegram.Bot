using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Simulant.Telegram.Bot;

public class WebhookBotClient : BotClientBase
{
  public WebhookBotClient(string token, string webhookUrl, BotUpdateHandler handler, ILogger<WebhookBotClient> logger) : base(token, handler, logger)
  {
    Client.SetWebhookAsync(webhookUrl).GetAwaiter().GetResult();
  }
}