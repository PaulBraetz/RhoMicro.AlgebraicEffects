namespace RhoMicro.AlgebraicEffects;
/// <summary>
/// Exposes apis for handling or performing algebraic effects..
/// </summary>
/// <typeparam name="T">The type of effect to handle.</typeparam>
public static class Effect<T>
{
    private static readonly AsyncLocal<EffectContext<T>> _context = new();
    private static Int32 _waiting;
    private static void Wait()
    {
        while(Interlocked.CompareExchange(ref _waiting, 1, 0) != 0)
        { }
    }
    private static void Release() => _waiting = 0;
    private static EffectContext<T> GetContext()
    {
#pragma warning disable IDE0074 // Use compound assignment
        Wait();
        try
        {
            if(_context.Value == null)
            {
                _context.Value = new();
            }

            return _context.Value;
        } finally
        {
            Release();
        }
#pragma warning restore IDE0074 // Use compound assignment
    }
    /// <summary>
    /// Clears all registered effect handlers.
    /// </summary>
    public static void Clear() => GetContext().Clear();
    /// <summary>
    /// Registers a handler for algebraic effects.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public static IDisposable Handle(EffectHandler<T> handler) => GetContext().Handle(handler);
    /// <summary>
    /// Registers an asynchronous handler for algebraic effects.
    /// </summary>
    /// <param name="handler">The handler to register.</param>
    public static IDisposable HandleAsync(AsyncEffectHandler<T> handler) => GetContext().Handle(handler);
    /// <summary>
    /// Requests and executes an algebraic effect handler.
    /// </summary>
    /// <returns>The result of the first handler able to produce a result.</returns>
    public static T Perform() => GetContext().Perform();
    /// <summary>
    /// Requests and executes an algebraic effect handler asynchronously.
    /// </summary>
    /// <returns>The result of the first handler able to produce a result.</returns>
    public static ValueTask<T> PerformAsync() => GetContext().PerformAsync();
}

/// <summary>
/// Represents a handler for algebraic effects of a specific type.
/// </summary>
/// <typeparam name="T">The type of algebraic effect this handler may handle.</typeparam>
/// <returns>A result object containing either a successfully produced result or an instance of <see cref="Unit"/>.</returns>
public delegate EffectHandlerResult<T> EffectHandler<T>();
/// <summary>
/// Represents an asynchronous handler for algebraic effects of a specific type.
/// </summary>
/// <typeparam name="T">The type of algebraic effect this handler may handle.</typeparam>
/// <returns>A result object containing either a successfully produced result or an instance of <see cref="Unit"/>.</returns>
public delegate Task<EffectHandlerResult<T>> AsyncEffectHandler<T>();