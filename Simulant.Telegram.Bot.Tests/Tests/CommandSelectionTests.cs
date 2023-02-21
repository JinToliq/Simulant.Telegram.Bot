using System.Threading;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Tests
{
  [TestFixture]
  public class CommandSelectionTests
  {
    [Test]
    public void HandledSimpleCommandTest()
    {
      var handler = new UpdateHandlerBase();
      var client = new TelegramBotClientMock();
      handler.AddTransients<TestHandler>();
      var update = new Update {Message = new Message {Text = "test foo"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }

    [Test]
    public void HandledComplexCommandTest()
    {
      var handler = new UpdateHandlerBase();
      var client = new TelegramBotClientMock();
      handler.AddTransients<TestHandler>();
      var update = new Update {Message = new Message {Text = "test foo bar"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
    }

    [Test]
    public void HandledComplexCommandWithTextTest()
    {
      const string text = "test text";
      var client = new TelegramBotClientMock();
      var handler = new UpdateHandlerBase();
      handler.AddTransients<TestHandler>();
      var update = new Update {Message = new Message {Text = $"test foo bar text {text}"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
      StringAssert.AreEqualIgnoringCase(text, client.Text);
    }

    [Test]
    public void HasNotHandledComplexCommandWithTextWithMarkerTest()
    {
      const string text = "test text";
      var client = new TelegramBotClientMock();
      var handler = new UpdateHandlerBase() {Marker = '$'};
      handler.AddTransients<TestHandler>();
      var update = new Update {Message = new Message {Text = $"test foo bar marker {text}"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsFalse(client.Check1);
      StringAssert.AreNotEqualIgnoringCase(text, client.Text);
    }

    [Test]
    public void HandledComplexCommandWithTextWithMarkerTest()
    {
      const string text = "test text";
      var client = new TelegramBotClientMock();
      var handler = new UpdateHandlerBase() {Marker = '$'};
      handler.AddTransients<TestHandler>();
      var update = new Update {Message = new Message {Text = $"$test foo bar marker {text}"}};
      handler.HandleAsync(client, update, CancellationToken.None);
      Assert.IsTrue(client.Check1);
      StringAssert.AreEqualIgnoringCase(text, client.Text);
    }
  }
}