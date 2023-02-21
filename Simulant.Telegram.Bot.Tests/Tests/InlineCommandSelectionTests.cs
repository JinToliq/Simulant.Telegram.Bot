using System.Threading;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Tests
{
  [TestFixture]
  public class InlineCommandSelectionTests
  {
    [Test]
    public void HandledSimpleInlineTest()
    {
      var handler = new UpdateHandlerBase();
      var client = new TelegramBotClientMock();
      handler.AddTransients<TestHandler>();
      var update = new Update {InlineQuery = new InlineQuery() {Id = "test foo"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }

    [Test]
    public void HandledComplexInlineTest()
    {
      var handler = new UpdateHandlerBase();
      var client = new TelegramBotClientMock();
      handler.AddTransients<TestHandler>();
      var update = new Update {InlineQuery = new InlineQuery() {Id = "test foo bar"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }
  }
}