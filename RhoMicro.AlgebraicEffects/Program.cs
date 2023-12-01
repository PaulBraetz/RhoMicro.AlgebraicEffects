namespace RhoMicro.AlgebraicEffects;
using RhoMicro.CodeAnalysis;

internal partial class Program
{
    enum EffectType
    {
        AskName
    }

    static readonly User _arya = new(null, []);
    static readonly User _gendry = new("Gendry", []);

    private static async Task Main(String[] _0)
    {
        using var _ = Effect<AskForName>.Handle(() => 
            EffectHandlerResult<AskForName>.CreateFromResult("Arya Stark"));
        var firstConnect = Task.Run(Connect);
        await firstConnect;
        await Connect();
    }

    static async Task Connect()
    {
        using var _ = Effect<AskForName>.Handle(() => new Unit());

        await Task.Yield();
        await MakeFriends(_arya, _gendry);
    }

    sealed record User(String? Name, List<String> FriendNames);

    static async Task MakeFriends(User user1, User user2)
    {
        user1.FriendNames.Add(await GetName(user2));
        user2.FriendNames.Add(await GetName(user1));
    }
    [UnionType(typeof(String), Options = UnionTypeOptions.ImplicitConversionIfSolitary)]
    readonly partial struct AskForName;
    static async Task<String> GetName(User user)
    {
        var name = user.Name;
        name ??= Effect<AskForName>.Perform();

        return name;
    }
}
