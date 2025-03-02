using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Simulant.Telegram.Bot.Tests.Handlers;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Tests.Cases
{
  [TestFixture]
  public class CommandSelectionTests
  {
    private static ILogger<BotUpdateHandler> _logger;

    static CommandSelectionTests()
    {
      var loggerFactory = LoggerFactory.Create(builder =>
      {
        builder.AddConsole();
      });

      _logger = loggerFactory.CreateLogger<BotUpdateHandler>();
    }

    [Test]
    public async Task HandledSimpleCommandTest()
    {
      var handler = new BotUpdateHandler(_logger);
      var client = new TelegramBotClientMock();
      handler.AddHandler<TestHandler>();
      var update = new Update {Message = new() {Text = "test foo"}};
      await handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }

    [Test]
    public async Task HandledComplexCommandTest()
    {
      var handler = new BotUpdateHandler(_logger);
      var client = new TelegramBotClientMock();
      handler.AddHandler<TestHandler>();
      var update = new Update {Message = new() {Text = "test foo bar"}};
      await handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }

    [Test]
    public async Task HandledComplexCommandWithTextTest()
    {
      const string text = "test text";
      var client = new TelegramBotClientMock();
      var handler = new BotUpdateHandler(_logger);
      handler.AddHandler<TestHandler>();
      var update = new Update {Message = new() {Text = $"test foo bar text {text}"}};
      await handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
      StringAssert.AreEqualIgnoringCase(text, client.Text);
    }

    [Test]
    public async Task HasNotHandledComplexCommandWithTextWithMarkerTest()
    {
      const string text = "test text";
      var client = new TelegramBotClientMock();
      var handler = new BotUpdateHandler(_logger) { Marker = '$' };
      handler.AddHandler<TestHandler>();
      var update = new Update {Message = new() {Text = $"test foo bar marker {text}"}};
      await handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsFalse(client.Check1);
      StringAssert.AreNotEqualIgnoringCase(text, client.Text);
    }

    [Test]
    public async Task HandledComplexCommandWithTextWithMarkerTest()
    {
      const string text = "test text";
      var client = new TelegramBotClientMock();
      var handler = new BotUpdateHandler(_logger) { Marker = '$' };
      handler.AddHandler<TestHandler>();
      var update = new Update {Message = new() {Text = $"$test foo bar marker {text}"}};
      await handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
      StringAssert.AreEqualIgnoringCase(text, client.Text);
    }
  }
}