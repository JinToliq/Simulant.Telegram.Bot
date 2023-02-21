using System;

namespace Simulant.Telegram.Bot.Logging;

public class Log
{
  public readonly DateTime DateTimeUTC;
  public readonly LogLevel Level;
  public readonly string Message;
  public readonly Exception? Exception;

  public Log(LogLevel level, string message, Exception? exception = null)
  {
    DateTimeUTC = DateTime.UtcNow;
    Level = level;
    Message = message;
    Exception = exception;
  }
}