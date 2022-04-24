namespace Simulant.Telegram.Bot.CommandHandling.Attributes
{
  public class InlineCommandAttribute : CommandAttributeBase
  {
    public override CommandType CommandType => CommandType.InlineCommand;

    public InlineCommandAttribute(string route) : base(route)
    { }
  }
}