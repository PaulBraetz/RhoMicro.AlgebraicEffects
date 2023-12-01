namespace RhoMicro.AlgebraicEffects.Tests;

public class UnitTest1
{
    [Fact]
    public void ClearClearsHandlers()
    {
        Effect<Int32>.Clear();
        var value = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(value));
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(value * 2));
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(value * 3));
        Effect<Int32>.Clear();
        _ = Assert.Throws<UnhandledEffectException<Int32>>(() => Effect<Int32>.Perform());
    }
    [Fact]
    public void DisposeRemovesHandler()
    {
        Effect<Int32>.Clear();
        var value = 123;
        using(var _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(value)))
        {
        }

        _ = Assert.Throws<UnhandledEffectException<Int32>>(() => Effect<Int32>.Perform());
    }
    [Fact]
    public void DisposeRemovesHandlers()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        using var _0 = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        using(var _1 = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected * 2)))
        {

        }

        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void PerformGetsUnhandledOnAsyncHandler()
    {
        var innerException = new InvalidOperationException();
        Effect<Int32>.Clear();
        _ = Effect<Int32>.HandleAsync(() => Task.FromException<EffectHandlerResult<Int32>>(innerException));
        _ = Assert.Throws<UnhandledEffectException<Int32>>(() => Effect<Int32>.Perform());
    }
    [Fact]
    public void PerformGetsUnhandledOnHandler()
    {
        var innerException = new InvalidOperationException();
        Effect<Int32>.Clear();
        _ = Effect<Int32>.Handle(() => throw innerException);
        _ = Assert.Throws<UnhandledEffectException<Int32>>(() => Effect<Int32>.Perform());
    }
    [Fact]
    public void PerformGetsSingleHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void PerformGetsLatestHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected * 2));
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void PerformGetsLatestNonUnitHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        _ = Effect<Int32>.Handle(() => new Unit());
        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public async Task PerformAsyncGetsSingleHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        var actual = await Effect<Int32>.PerformAsync();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public async Task PerformAsyncGetsLatestHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected * 2));
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        var actual = await Effect<Int32>.PerformAsync();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public async Task PerformAsyncGetsLatestNonUnitHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.Handle(() => EffectHandlerResult<Int32>.CreateFromResult(expected));
        _ = Effect<Int32>.Handle(() => new Unit());
        var actual = await Effect<Int32>.PerformAsync();
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void PerformGetsSingleAsyncHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected)));
        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void PerformGetsLatestAsyncHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected * 2)));
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected)));
        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public void PerformGetsLatestNonUnitAsyncHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected)));
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromUnit(new())));
        var actual = Effect<Int32>.Perform();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public async Task PerformAsyncGetsSingleAsyncHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected)));
        var actual = await Effect<Int32>.PerformAsync();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public async Task PerformAsyncGetsLatestAsyncHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected * 2)));
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected)));
        var actual = await Effect<Int32>.PerformAsync();
        Assert.Equal(expected, actual);
    }
    [Fact]
    public async Task PerformAsyncGetsLatestNonUnitAsyncHandler()
    {
        Effect<Int32>.Clear();
        var expected = 123;
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromResult(expected)));
        _ = Effect<Int32>.HandleAsync(() => Task.FromResult(EffectHandlerResult<Int32>.CreateFromUnit(new())));
        var actual = await Effect<Int32>.PerformAsync();
        Assert.Equal(expected, actual);
    }
}