using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Simulant.Telegram.Bot.CommandHandling;
using Simulant.Telegram.Bot.CommandHandling.Attributes;
using Simulant.Telegram.Bot.Logging;
using Stashbox;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using LogLevel = Simulant.Telegram.Bot.Logging.LogLevel;

namespace Simulant.Telegram.Bot
{
  public class UpdateHandler : IDisposable
  {
    public event Func<ITelegramBotClient, Update, CancellationToken, Task<bool>>? BeforeUpdate;
    public event Func<ITelegramBotClient, Update, CancellationToken, Task>? AfterUpdate;

    internal event Action<Log>? Log;
    private List<CommandInfo> _commands = new();
    private readonly StashboxContainer _container = new();
    private readonly Type _defaultHandlerType = typeof(DefaultCommandHandler);
    private readonly Type _baseHandlerType = typeof(CommandHandlerBase);
    private bool _isDefaultCommandHandlerRegistered;

    public char? Marker { get; set; }
    public IReadOnlyList<CommandInfo> Commands => _commands;

    public void Dispose() => _container.Dispose();

    public UpdateHandler AddHandler<THandler>() where THandler : CommandHandlerBase
    {
      AssertServiceIsNotRegistered<THandler>();
      AssertServiceIsCommandHandler<THandler>();
      AssertServiceIsNotDefaultHandler<THandler>();

      _container.Register<THandler>();
      TryRegisterCommands<THandler, CommandAttributeBase>();
      return this;
    }

    public UpdateHandler AddDefaultHandler<THandler>() where THandler : DefaultCommandHandler
    {
      AssertServiceIsNotRegistered<THandler>();
      AssertServiceIsNotCommandHandler<THandler>();
      AssertServiceIsDefaultHandler<THandler>();

      _container.Register<THandler>();
      _isDefaultCommandHandlerRegistered = true;
      return this;
    }

    public UpdateHandler AddTransients<TService>() where TService : class
    {
      AssertServiceIsNotRegistered<TService>();
      AssertServiceIsNotCommandHandler<TService>();
      AssertServiceIsNotDefaultHandler<TService>();

      _container.Register<TService>();
      return this;
    }

    public UpdateHandler AddSingleton<TService>() where TService : class
    {
      AssertServiceIsNotRegistered<TService>();
      AssertServiceIsNotCommandHandler<TService>();
      AssertServiceIsNotDefaultHandler<TService>();

      _container.RegisterSingleton<TService>();
      return this;
    }

    public UpdateHandler AddInstance<TService>(TService instance) where TService : class
    {
      ArgumentNullException.ThrowIfNull(instance);
      AssertServiceIsNotRegistered<TService>();
      AssertServiceIsNotCommandHandler<TService>();
      AssertServiceIsNotDefaultHandler<TService>();

      _container.RegisterInstance(instance);
      return this;
    }

    public async Task Handle(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
      try
      {
        if (BeforeUpdate != null && await BeforeUpdate.Invoke(client, update, cancellationToken))
          return;

        if (_isDefaultCommandHandlerRegistered)
        {
          var defaultHandler = (DefaultCommandHandler)_container.Resolve(_defaultHandlerType);
          var shouldReturn = await defaultHandler.Handle(client, update, cancellationToken);
          if (shouldReturn)
            return;
        }

        switch (update.Type)
        {
          case UpdateType.InlineQuery:
            await HandleInlineQuery(client, update, cancellationToken);
            break;
          case UpdateType.Message:
            await HandleMessage(client, update, cancellationToken);
            break;
        }

        if (AfterUpdate != null)
          await AfterUpdate.Invoke(client, update, cancellationToken);
      }
      catch (Exception e)
      {
        Log?.Invoke(new(LogLevel.Error, "An error has occured while handling update", e));
      }
    }

    private async Task HandleInlineQuery(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
      var query = update.InlineQuery!.Id;
      if (!TryGetCommand(query, CommandType.InlineCommand, out var command))
      {
        Log?.Invoke(new(LogLevel.Warning, $"Requested inline command not found: {query}"));
        return;
      }

      await HandleCommand(client, update, command!, null, cancellationToken);
    }

    private async Task HandleMessage(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
      await TryHandleAsText(client, update, cancellationToken);
    }

    private async Task TryHandleAsText(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
      var text = update.Message!.Text;
      if (text is null)
        return;

      if (Marker is not null)
      {
        if (!text.StartsWith(Marker.Value))
          return;

        text = text.Trim(Marker.Value);
      }

      if (!TryGetCommand(text, CommandType.Command, out var command))
      {
        Log?.Invoke(new(LogLevel.Warning, $"Requested command not found: {text}"));
        return;
      }

      text = command!.Route.Length < text.Length
        ? text[(command.Route.Length + 1)..]
        : null;

      await HandleCommand(client, update, command, text, cancellationToken);
    }

    private bool TryGetCommand(string input, CommandType type, out CommandInfo? command)
    {
      command = _commands.FirstOrDefault(c => c.CommandType == type && input.StartsWith(c.Route));
      return command is not null;
    }

    private async Task HandleCommand(ITelegramBotClient client, Update update, CommandInfo command, string? text, CancellationToken cancellationToken)
    {
      var handler = (CommandHandlerBase)_container.Resolve(command.DeclaringType);
      handler.Initialize(client, update, text, cancellationToken);
      await (Task)command.MethodInfo.Invoke(handler, null)!;
    }

    private void TryRegisterCommands<TService, TCommand>() where TCommand : CommandAttributeBase
    {
      var entries = GetCommands<TService, TCommand>();
      if (entries.Length == default)
        return;

      foreach (var item in entries)
      {
        var existingCommand = _commands.FirstOrDefault(c => c.Equals(item));
        if (existingCommand is not null)
          throw new($"{item.CommandType} {item.Route} from {item.DeclaringType} already exists in handler {existingCommand.DeclaringType.Name}");
      }

      _commands.AddRange(entries);
      _commands = _commands
        .OrderByDescending(c => c.Route.Split(' ').Length)
        .ToList();
    }

    private static CommandInfo[] GetCommands<TService, TCommand>() where TCommand : CommandAttributeBase
    {
      return typeof(TService)
        .GetMethods(CommandAttributeBase.PossibleMethodAttributes)
        .Where(m => m.GetCustomAttribute<TCommand>() is not null)
        .Select(m =>
        {
          var attribute = m.GetCustomAttribute<TCommand>()!;
          attribute.TryPatchParent(m.DeclaringType!);
          return new CommandInfo(attribute.Route, attribute.Description, attribute.CommandType, m.DeclaringType!, m);
        })
        .ToArray();
    }

    private void AssertServiceIsNotCommandHandler<TService>([CallerMemberName] string? member = null) where TService : class
    {
      var serviceType = typeof(TService);
      if (_baseHandlerType.IsAssignableFrom(serviceType))
        throw new ArgumentException($"Using {_baseHandlerType.Name} inheritors are not allowed for {member}. Use {nameof(AddHandler)} instead");
    }

    private void AssertServiceIsCommandHandler<TService>([CallerMemberName] string? member = null) where TService : class
    {
      var serviceType = typeof(TService);
      if (!_baseHandlerType.IsAssignableFrom(serviceType))
        throw new ArgumentException($"Only {_baseHandlerType.Name} inheritors are allowed for {member}");
    }

    private void AssertServiceIsNotDefaultHandler<TService>([CallerMemberName] string? member = null) where TService : class
    {
      var serviceType = typeof(TService);
      if (_defaultHandlerType.IsAssignableFrom(serviceType))
        throw new ArgumentException($"Using {_defaultHandlerType.Name} inheritors are not allowed for {member}. Use {nameof(AddDefaultHandler)} instead");
    }

    private void AssertServiceIsDefaultHandler<TService>([CallerMemberName] string? member = null) where TService : class
    {
      var serviceType = typeof(TService);
      if (_defaultHandlerType.IsAssignableFrom(serviceType))
        throw new ArgumentException($"Only {_defaultHandlerType.Name} inheritors are allowed for {member}");
    }

    private void AssertServiceIsNotRegistered<TService>() where TService : class
    {
      if (_container.IsRegistered<TService>())
        throw new ArgumentException($"Service {typeof(TService).Name} is already registered");
    }
  }
}