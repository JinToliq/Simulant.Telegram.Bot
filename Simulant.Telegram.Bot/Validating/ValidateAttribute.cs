using System;

namespace Simulant.Telegram.Bot.Validating;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public abstract class ValidateAttribute : Attribute
{
  public ValidatorBase Validator { get; protected set; }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class ValidateAttribute<TValidator> : ValidateAttribute where TValidator : ValidatorBase, new()
{
  public ValidateAttribute() => Validator = ValidatorProvider.Get<TValidator>();
}