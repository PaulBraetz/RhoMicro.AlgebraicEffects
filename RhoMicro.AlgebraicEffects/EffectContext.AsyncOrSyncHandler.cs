namespace RhoMicro.AlgebraicEffects;
sealed partial class EffectContext<T>
{
    readonly struct AsyncOrSyncHandler
    {
        private readonly Object _handler;
        public readonly Boolean IsAsync;
        public readonly HandlerLifetime Lifetime;

        public AsyncOrSyncHandler(EffectHandler<T> handler, HandlerLifetime lifetime)
        {
            _handler = handler;
            IsAsync = false;
            Lifetime = lifetime;
        }
        public AsyncOrSyncHandler(AsyncEffectHandler<T> handler, HandlerLifetime lifetime)
        {
            _handler = handler;
            IsAsync = true;
            Lifetime = lifetime;
        }

        public EffectHandler<T> SyncHandler => (EffectHandler<T>)_handler;
        public AsyncEffectHandler<T> AsyncHandler => (AsyncEffectHandler<T>)_handler;

        public override String ToString() => IsAsync ? "Async Handler" : "Sync Handler";
    }
}