using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
  [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
  public class UpdateHandlerBase : IDisposable
  {
    internal event Action<Log>? Log;
    private List<CommandInfo> _commands = new();
    private readonly StashboxContainer _container = new();
    private readonly Type _baseHandlerType = typeof(CommandHandlerBase);

    public char? Marker { get; set; }
    public IReadOnlyList<CommandInfo> Commands => _commands;

    public void Dispose() => _container.Dispose();

    public UpdateHandlerBase AddHandler<THandler>() where THandler : CommandHandlerBase
    {
      AssertServiceIsNotRegistered<THandler>();
      AssertServiceIsCommandHandler<THandler>();

      _container.Register<THandler>();
      TryRegisterCommands<THandler, CommandAttributeBase>();
      return this;
    }

    public UpdateHandlerBase AddTransients<TService>() where TService : class
    {
      AssertServiceIsNotRegistered<TService>();
      AssertServiceIsNotCommandHandler<TService>();

      _container.Register<TService>();
      return this;
    }

    public UpdateHandlerBase AddSingleton<TService>() where TService : class
    {
      AssertServiceIsNotRegistered<TService>();
      AssertServiceIsNotCommandHandler<TService>();

      _container.RegisterSingleton<TService>();
      return this;
    }

    public UpdateHandlerBase AddInstance<TService>(TService instance) where TService : class
    {
      ArgumentNullException.ThrowIfNull(instance);
      AssertServiceIsNotRegistered<TService>();
      AssertServiceIsNotCommandHandler<TService>();

      _container.RegisterInstance(instance);
      return this;
    }

    public async Task HandleAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
      try
      {
        if (await BeforeUpdateAsync(client, update, cancellationToken))
          return;

        switch (update.Type)
        {
          case UpdateType.Unknown:
            throw new InvalidOperationException($"Unknown update type: {update}");

          case UpdateType.InlineQuery:
            await HandleInlineQueryAsync(client, update, cancellationToken);
            break;

          case UpdateType.Message:
            await HandleMessageAsync(client, update, cancellationToken);
            break;

          case UpdateType.ChosenInlineResult:
            await HandleChosenInlineResultAsync(client, update, cancellationToken);
            break;

          case UpdateType.CallbackQuery:
            await HandleCallbackQueryAsync(client, update, cancellationToken);
            break;

          case UpdateType.EditedMessage:
            await HandleEditedMessageAsync(client, update, cancellationToken);
            break;

          case UpdateType.ChannelPost:
            await HandleChannelPostAsync(client, update, cancellationToken);
            break;

          case UpdateType.EditedChannelPost:
            await HandleEditedChannelPostAsync(client, update, cancellationToken);
            break;

          case UpdateType.ShippingQuery:
            await HandleShippingQueryAsync(client, update, cancellationToken);
            break;

          case UpdateType.PreCheckoutQuery:
            await HandlePreCheckoutQueryAsync(client, update, cancellationToken);
            break;

          case UpdateType.Poll:
            await HandlePollAsync(client, update, cancellationToken);
            break;

          case UpdateType.PollAnswer:
            await HandlePollAnswerAsync(client, update, cancellationToken);
            break;

          case UpdateType.MyChatMember:
            await HandleMyChatMemberAsync(client, update, cancellationToken);
            break;

          case UpdateType.ChatMember:
            await HandleChatMemberAsync(client, update, cancellationToken);
            break;

          case UpdateType.ChatJoinRequest:
            await HandleChatJoinRequestAsync(client, update, cancellationToken);
            break;

          default:
            throw new ArgumentOutOfRangeException(nameof(update.Type), update.Type, "Unsupported update type");
        }

        await AfterUpdateAsync(client, update, cancellationToken);
      }
      catch (Exception e)
      {
        Log?.Invoke(new(LogLevel.Error, "An error has occured while handling update", e));
      }
    }

    protected virtual Task<bool> BeforeUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.FromResult(false);

    protected virtual Task AfterUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleChosenInlineResultAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleCallbackQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleEditedMessageAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleChannelPostAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleEditedChannelPostAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleShippingQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandlePreCheckoutQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandlePollAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandlePollAnswerAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleMyChatMemberAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleChatMemberAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual Task HandleChatJoinRequestAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task HandleInlineQueryAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
      var query = update.InlineQuery!.Id;
      if (!TryGetCommand(query, CommandType.InlineQuery, out var command))
      {
        Log?.Invoke(new(LogLevel.Warning, $"Requested inline command not found: {query}"));
        return;
      }

      await HandleCommand(client, update, command!, null, cancellationToken);
    }

    private async Task HandleMessageAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
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

    private void AssertServiceIsNotRegistered<TService>() where TService : class
    {
      if (_container.IsRegistered<TService>())
        throw new ArgumentException($"Service {typeof(TService).Name} is already registered");
    }
  }
}