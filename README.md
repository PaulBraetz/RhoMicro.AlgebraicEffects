# RhoMicro.AlgebraicEffects

Provides an implementation of algebraic effects for C#.

If you are new to algebraic effects, here is a great explanation: https://overreacted.io/algebraic-effects-for-the-rest-of-us/

# How to Use
Effects can be handles using the `Effect<>` type:

`Effect<T>.Handle(EffectHandler<T> handler)`

`Effect<T>.HandleAsync(AsyncEffectHandler<T> handler)`

`Handle` and `HandleAsync` both return an instance of `IDisposable` that manages the lifetime of the handler registration:

```cs
EffectHandler<T> myHandler = static () => EffectHandlerResult<T>.CreateFromResult("Some Result");
using(var lt = Effect<T>.Handle(myHandler)){
    //methods called here may receive the handler
}
//the handler is unregistered, methods called here may not receive it
```

Effects may be handled and performed synchronously or asynchronously:
```cs
EffectHandler<T> syncHandler = static () => EffectHandlerResult<T>.CreateFromResult("Some Result");
using var _ = Effect<T>.Handle(syncHandler);

EffectHandler<T> asyncHandler = static () => Task.FromResult(EffectHandlerResult<T>.CreateFromResult("Some Result"));
using var _ = Effect<T>.HandleAsync(asyncHandler);
```

```cd
var missingData = Effect<T>.Perform();

var moreMissingData = await Effect<T>.PerformAsync();
```

Effect handlers may elect not to produce a value by returning `EffectHandlerResult<T>.Unit`:
```cs
EffectHandler<T> myHandler = static () => EffectHandlerResult<T>.Unit;
```

Handlers returning `EffectHandlerResult<T>.Unit` will be ignored. If, upon performing an effect, there are no handlers returning a result available, an instance of `UnhandledEffectException<T>` will be thrown.
```cs
try{
    var missingData = Effect<T>.Perform();
}catch(UnhandledEffectException<T> specificEx){
    //handle our effect being unhandled
}catch(UnhandledEffectException generalizedEx){
    //handle any effect being unhandled
}
```