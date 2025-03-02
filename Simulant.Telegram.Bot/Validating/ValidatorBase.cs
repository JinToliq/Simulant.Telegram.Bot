using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simulant.Telegram.Bot.Validating;

public class ValidationResult(bool isSuccess, string? errorMessage = null)
{
  public bool IsSuccess { get; } = isSuccess;
  public string? ErrorMessage { get; } = errorMessage;

  public static ValidationResult Success() => new(true);

  public static ValidationResult Fail(string errorMessage) => new(false, errorMessage);
}

public abstract class ValidatorBase
{
  public abstract Task<ValidationResult> ValidateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken);
}