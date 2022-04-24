using System;

namespace Simulant.Telegram.Bot.CommandHandling.Attributes
{
  public abstract class RouteAttributeBase : Attribute
  {
    protected readonly string SelfRoute;

    public abstract string Route { get; }

    protected RouteAttributeBase(string route) => SelfRoute = route;
  }
}