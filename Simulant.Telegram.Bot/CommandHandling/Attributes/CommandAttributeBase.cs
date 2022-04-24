using System;
using System.Reflection;

namespace Simulant.Telegram.Bot.CommandHandling.Attributes
{
  [AttributeUsage(AttributeTargets.Method)]
  public abstract class CommandAttributeBase : RouteAttributeBase
  {
    public const BindingFlags PossibleMethodAttributes = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    public string? Description;

    public abstract CommandType CommandType { get; }
    public RouteAttributeBase? Parent { get; private set; }

    public override string Route => Parent is null ? SelfRoute : $"{Parent.Route} {SelfRoute}";

    protected CommandAttributeBase(string route) : base(route)
    { }

    protected CommandAttributeBase(string route, string description) : base(route)
    {
      Description = description;
    }

    public void TryPatchParent(Type declaringType)
    {
      Parent = declaringType.GetCustomAttribute<RouteAttribute>();
    }
  }
}