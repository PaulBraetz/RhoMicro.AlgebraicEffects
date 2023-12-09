namespace RhoMicro.AlgebraicEffects;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

sealed partial class EffectContext<T>
{
    private const Int32 _handlerGateTimeout = 5000;
    readonly SemaphoreSlim _handlerGate = new(1);
    Stack<AsyncOrSyncHandler> _handlers = [];
    readonly HashSet<Int32> _lifetimes = [];

    void Release() => _handlerGate.Release();
    void ReleaseMove(Stack<AsyncOrSyncHandler> handlers)
    {
        while(_handlers.TryPop(out var handler))
        {
            handlers.Push(handler);
        }

        _handlers = handlers;
        Release();
    }
    void Wait()
    {
        if(!_handlerGate.Wait(_handlerGateTimeout))
        {
            ThrowTimeout();
        }
    }
    async ValueTask WaitAsync()
    {
        if(!await _handlerGate.WaitAsync(_handlerGateTimeout))
        {
            ThrowTimeout();
        }
    }

    private HandlerLifetime GetLifetime()
    {
        var id = _lifetimes.Count;
        while(!_lifetimes.Add(id))
            id++;

        var result = new HandlerLifetime(_lifetimes, id);

        return result;
    }
    public IDisposable Handle(EffectHandler<T> handler)
    {
        Wait();
        try
        {
            var lifetime = GetLifetime();
            _handlers.Push(new(handler, lifetime));
            return lifetime;
        } finally
        {
            Release();
        }
    }
    public IDisposable Handle(AsyncEffectHandler<T> asyncHandler)
    {
        Wait();
        try
        {
            var lifetime = GetLifetime();
            _handlers.Push(new(asyncHandler, lifetime));
            return lifetime;
        } finally
        {
            Release();
        }
    }
    public async ValueTask<T> PerformAsync()
    {
        await WaitAsync();
        var handlers = new Stack<AsyncOrSyncHandler>();
        try
        {
            while(_handlers.TryPop(out var handler))
            {
                if(handler.Lifetime.IsDisposed)
                {
                    continue;
                }

                handlers.Push(handler);
                if(handler.IsAsync)
                {
                    var result = await RunHandler(handler.AsyncHandler);

                    if(result.IsResult)
                    {
                        return result.AsResult;
                    }
                } else
                {
                    var result = RunHandler(handler.SyncHandler);

                    if(result.IsResult)
                    {
                        return result.AsResult;
                    }
                }
            }

            throw new UnhandledEffectException<T>();
        } finally
        {
            ReleaseMove(handlers);
        }
    }
    public T Perform()
    {
        Wait();
        var handlers = new Stack<AsyncOrSyncHandler>();
        try
        {
            while(_handlers.TryPop(out var handler))
            {
                if(handler.Lifetime.IsDisposed)
                {
                    continue;
                }

                handlers.Push(handler);
                if(handler.IsAsync)
                {
                    var asyncHandlerGate = new SemaphoreSlim(0, 1);
                    var resultTask = RunHandler(handler.AsyncHandler, asyncHandlerGate);
                    asyncHandlerGate.Wait();

                    if(resultTask.IsCompleted && resultTask.Result.IsResult)
                    {
                        return resultTask.Result.AsResult;
                    }
                } else
                {
                    var result = RunHandler(handler.SyncHandler);

                    if(result.IsResult)
                    {
                        return result.AsResult;
                    }
                }
            }

            throw new UnhandledEffectException<T>();
        } finally
        {
            Release();
        }
    }
    private static EffectHandlerResult<T> RunHandler(EffectHandler<T> handler)
    {
        try
        {
            var result = handler.Invoke();

            return result;
        } catch
        {
            return EffectHandlerResult<T>.CreateFromUnit(new());
        }
    }
    private static async Task<EffectHandlerResult<T>> RunHandler(AsyncEffectHandler<T> handler)
    {
        try
        {
            var result = await handler.Invoke();

            return result;
        } catch
        {
            return EffectHandlerResult<T>.CreateFromUnit(new());
        }
    }
    private static async Task<EffectHandlerResult<T>> RunHandler(AsyncEffectHandler<T> handler, SemaphoreSlim gate)
    {
        try
        {
            var result = await handler.Invoke();

            return result;
        } catch
        {
            return EffectHandlerResult<T>.CreateFromUnit(new());
        } finally
        {
            _ = gate.Release();
        }
    }
    public void Clear()
    {
        Wait();
        try
        {
            while(_handlers.TryPop(out var handler))
            {
                handler.Lifetime.Dispose();
            }
        } finally
        {
            Release();
        }
    }
    [DoesNotReturn]
    private static void ThrowTimeout() =>
        throw new TimeoutException($"The effect synchronization mechanism for {typeof(Effect<T>)} timed out after {_handlerGateTimeout}ms.");
}