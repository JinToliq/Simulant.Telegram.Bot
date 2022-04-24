using System;
using System.Reflection;
#pragma warning disable 659

namespace Simulant.Telegram.Bot
{
  public enum CommandType
  {
    Command = 0,
    InlineCommand = 1,
  }

  public class CommandInfo
  {
    public readonly string Route;
    public readonly string? Description;
    public readonly CommandType CommandType;
    public readonly Type DeclaringType;
    public readonly MethodInfo MethodInfo;

    public CommandInfo(string route, string? description, CommandType commandType, Type declaringType, MethodInfo methodInfo)
    {
      Route = route;
      Description = description;
      CommandType = commandType;
      DeclaringType = declaringType;
      MethodInfo = methodInfo;
    }

    public override bool Equals(object? obj)
    {
      if (obj is not CommandInfo other)
        return false;

      return CommandType == other.CommandType && string.Equals(Route, other.Route, StringComparison.OrdinalIgnoreCase);
    }
  }
}