using System;

namespace Simulant.Telegram.Bot.CommandHandling.Attributes
{
  [AttributeUsage(AttributeTargets.Class)]
  public class RouteAttribute : RouteAttributeBase
  {
    public override string Route => SelfRoute;

    public RouteAttribute(string route) : base(route)
    { }
  }
}