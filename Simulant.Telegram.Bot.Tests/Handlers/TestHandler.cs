﻿using System.Threading.Tasks;
using Simulant.Telegram.Bot.CommandHandling;
using Simulant.Telegram.Bot.CommandHandling.Attributes;

namespace Simulant.Telegram.Bot.Tests.Handlers
{
  [Route("test")]
  public class TestHandler : CommandHandlerBase
  {
    private TelegramBotClientMock Mock => Client as TelegramBotClientMock;

    [Command("foo")]
    public void HandleSimpleCommand()
    {
      Mock.Check1 = true;
      Mock.Text = Text;
    }

    [Command("foo bar")]
    public void HandleComplexCommand()
    {
      Mock.Check1 = true;
      Mock.Text = Text;
    }

    [Command("foo bar text")]
    public void HandleComplexCommandWithText()
    {
      Mock.Check1 = true;
      Mock.Text = Text;
    }

    [Command("foo bar marker")]
    public async Task HandleComplexCommandWithTextAndMarker()
    {
      Mock.Check1 = true;
      Mock.Text = Text;
      await Task.CompletedTask;
      return;
    }

    [InlineCommand("foo")]
    public void HandleSimpleInline()
    {
      Mock.Check1 = true;
      Mock.Text = Text;
    }

    [InlineCommand("foo bar")]
    public void HandleComplexInline()
    {
      Mock.Check1 = true;
      Mock.Text = Text;
    }
  }
}