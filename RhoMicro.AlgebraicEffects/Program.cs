using RhoMicro.CodeAnalysis;

internal class Program
{
    enum EffectType
    {
        AskName
    }

    static readonly User _arya = new(null, []);
    static readonly User _gendry = new("Gendry", []);

    private static async Task Main(String[] _)
    {
        Effect<String>.Handle(() => EffectHandlerResult<String>.Unit);
        var firstConnect = Task.Run(Connect);
        await Connect();
        await firstConnect;
    }

    static async Task Connect()
    {
        Effect<String>.Handle(() => EffectHandlerResult<String>.Result("Arya Stark"));
        await Task.Yield();
        await MakeFriends(_arya, _gendry);
    }

    sealed record User(String? Name, List<String> FriendNames);

    static async Task<String> GetName(User user)
    {
        var name = user.Name;
        name ??= Effect<String>.Perform();

        return name;
    }
    static async Task MakeFriends(User user1, User user2)
    {
        user1.FriendNames.Add(await GetName(user2));
        user2.FriendNames.Add(await GetName(user1));
    }
}

class UnhandledEffectException(Exception? innerException = null) : Exception("Effect was not handled appropriately.", innerException);
sealed class UnhandledEffectException<T>(Exception? innerException = null) : UnhandledEffectException(innerException);
sealed class MissingEffectContextException : Exception;

readonly struct Unit;
[UnionType(typeof(Unit))]
[UnionType(nameof(T), Alias = "Result")]
readonly partial struct EffectHandlerResult<T>
{
    public static readonly EffectHandlerResult<T> Unit = EffectHandlerResult<T>.CreateFromUnit(new());
    public static EffectHandlerResult<T> Result(T result) => EffectHandlerResult<T>.CreateFromResult(result);
}

static class Effect<T>
{
    private static readonly AsyncLocal<EffectContext<T>> _context = new()
    {
        Value = new()
    };

    public static void Handle(EffectHandler<T> handler) => (_context.Value ?? throw new MissingEffectContextException()).Handle(handler);
    public static void HandleAsync(AsyncEffectHandler<T> handler) => (_context.Value ?? throw new MissingEffectContextException()).Handle(handler);
    public static T Perform() => (_context.Value ?? throw new MissingEffectContextException()).Perform();
    public static ValueTask<T> PerformAsync() => (_context.Value ?? throw new MissingEffectContextException()).PerformAsync();
}

delegate EffectHandlerResult<T> EffectHandler<T>();
delegate Task<EffectHandlerResult<T>> AsyncEffectHandler<T>();

sealed class EffectContext<T>
{
    private const Int32 _idleState = 0;
    private const Int32 _busyState = 1;

    private AsyncEffectHandler<T>? _asyncHandler;
    private EffectHandler<T>? _handler;
    private Int32 _state;

    private void Wait()
    {
        while(Interlocked.CompareExchange(ref _state, _busyState, _idleState) != _idleState)
        { }
    }
    private void Release() => _state = _idleState;

    public void Handle(EffectHandler<T> handler)
    {
        Wait();
        _asyncHandler = null;
        _handler = handler;
        Release();
    }
    public void Handle(AsyncEffectHandler<T> asyncHandler)
    {
        Wait();
        _handler = null;
        _asyncHandler = asyncHandler;
        Release();
    }
    public async ValueTask<T> PerformAsync()
    {
        Wait();
        var handler = _handler;
        var asyncHandler = _asyncHandler;
        Release();

        if(handler != null)
        {
            var result = handler.Invoke();

            if(result.IsResult)
            {
                return result.AsResult;
            }
        } else if(asyncHandler != null)
        {
            var result = await asyncHandler.Invoke();

            if(result.IsResult)
            {
                return result.AsResult;
            }
        }

        throw new UnhandledEffectException<T>();
    }
    public T Perform()
    {
        Wait();
        var handler = _handler;
        var asyncHandler = _asyncHandler;
        Release();

        if(handler != null)
        {
            var result = handler.Invoke();

            if(result.IsResult)
            {
                return result.AsResult;
            }
        } else if(asyncHandler != null)
        {
            using var gate = new SemaphoreSlim(1);
            var resultTask = Task.Run(asyncHandler.Invoke).ContinueWith(t =>
            {
                _ = gate.Release();
                return t.Result;
            });
            gate.Wait();

            if(resultTask.IsCompleted && resultTask.Result.IsResult)
            {
                return resultTask.Result.AsResult;
            } else if(resultTask.IsFaulted)
            {
                throw new UnhandledEffectException<T>(resultTask.Exception);
            }
        }

        throw new UnhandledEffectException<T>();
    }
}