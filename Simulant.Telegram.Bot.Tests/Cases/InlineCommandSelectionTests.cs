using System.Threading;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Simulant.Telegram.Bot.Tests.Handlers;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Tests.Cases
{
  [TestFixture]
  public class InlineCommandSelectionTests
  {
    private static ILogger<BotUpdateHandler> _logger;

    static InlineCommandSelectionTests()
    {
      var loggerFactory = LoggerFactory.Create(builder =>
      {
        builder.AddConsole();
      });

      _logger = loggerFactory.CreateLogger<BotUpdateHandler>();
    }

    [Test]
    public void HandledSimpleInlineTest()
    {
      var handler = new BotUpdateHandler(_logger);
      var client = new TelegramBotClientMock();
      handler.AddTransients<TestHandler>();
      var update = new Update {InlineQuery = new() {Id = "test foo"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }

    [Test]
    public void HandledComplexInlineTest()
    {
      var handler = new BotUpdateHandler(_logger);
      var client = new TelegramBotClientMock();
      handler.AddTransients<TestHandler>();
      var update = new Update {InlineQuery = new() {Id = "test foo bar"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }
  }
}