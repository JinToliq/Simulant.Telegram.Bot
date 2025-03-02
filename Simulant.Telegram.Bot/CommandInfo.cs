using System;
using System.Linq;
using System.Reflection;
using Simulant.Telegram.Bot.Validating;

#pragma warning disable 659

namespace Simulant.Telegram.Bot
{
  public enum CommandType
  {
    Command = 0,
    InlineQuery = 1,
  }

  public class CommandInfo
  {
    public readonly string Route;
    public readonly string? Description;
    public readonly CommandType CommandType;
    public readonly Type DeclaringType;
    public readonly MethodInfo MethodInfo;
    public readonly Type[] Validators;

    public CommandInfo(string route, string? description, CommandType commandType, Type declaringType, MethodInfo methodInfo)
    {
      Route = route;
      Description = description;
      CommandType = commandType;
      DeclaringType = declaringType;
      MethodInfo = methodInfo;
      Validators = declaringType.CustomAttributes
        .Where(a => a.AttributeType.IsAssignableTo(typeof(ValidateAttribute)))
        .Concat(methodInfo.CustomAttributes.Where(a => a.AttributeType.IsAssignableTo(typeof(ValidateAttribute))))
        .Select(a => a.AttributeType.GenericTypeArguments[0])
        .ToArray();
    }

    public override bool Equals(object? obj)
    {
      if (obj is not CommandInfo other)
        return false;

      return CommandType == other.CommandType && string.Equals(Route, other.Route, StringComparison.OrdinalIgnoreCase);
    }
  }
}