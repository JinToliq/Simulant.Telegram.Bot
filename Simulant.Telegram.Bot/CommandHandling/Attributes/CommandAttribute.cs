namespace Simulant.Telegram.Bot.CommandHandling.Attributes
{
  public class CommandAttribute : CommandAttributeBase
  {
    public override CommandType CommandType => CommandType.Command;

    public CommandAttribute(string name) : base(name)
    {
    }
  }
}