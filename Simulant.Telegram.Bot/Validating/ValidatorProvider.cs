using System;
using System.Collections.Concurrent;

namespace Simulant.Telegram.Bot.Validating;

public static class ValidatorProvider
{
  private static readonly ConcurrentDictionary<Type, ValidatorBase> Registry = new();

  public static TValidator Get<TValidator>() where TValidator : ValidatorBase, new()
  {
    return (TValidator)Registry.GetOrAdd(typeof(TValidator), _ => new TValidator());
  }
}