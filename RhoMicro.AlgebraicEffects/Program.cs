using System.Diagnostics.CodeAnalysis;

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
        Effect<String>.Handle(() => "garble garble");
        var firstConnect = Task.Run(Connect);
        await Connect();
        await firstConnect;
    }

    static async Task Connect()
    {
        Effect<String>.Handle(() => "Arya Stark");
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

class UnhandledEffectException : Exception { }
sealed class UnhandledEffectException<T> : UnhandledEffectException { }
sealed class MissingEffectContextException : Exception { }

static class Effect<T>
{
    sealed class Context
    {
        private Func<T>? _handler;
        public void Handle(Func<T> handler) => _handler = handler;
        public T Perform() => (_handler ?? throw new UnhandledEffectException<T>()).Invoke();
    }

    private static readonly AsyncLocal<Context> _context = new()
    {
        Value = new()
    };

    public static void Handle(Func<T> handler) => (_context.Value ?? throw new MissingEffectContextException()).Handle(handler);
    public static T Perform() => (_context.Value ?? throw new MissingEffectContextException()).Perform();
}
