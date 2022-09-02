using System;

namespace Simulant.Telegram.Bot.Logging;

public class Log
{
  public readonly LogLevel Level;
  public readonly string Message;
  public readonly Exception? Exception;

  public Log(LogLevel level, string message, Exception? exception = null)
  {
    Level = level;
    Message = message;
    Exception = exception;
  }
}