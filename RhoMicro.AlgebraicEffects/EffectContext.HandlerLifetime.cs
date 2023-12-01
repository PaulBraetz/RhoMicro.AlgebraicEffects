namespace RhoMicro.AlgebraicEffects;
sealed partial class EffectContext<T>
{
    sealed class HandlerLifetime : IDisposable
    {
        private readonly HashSet<Int32> _lifetimeIds;
        public readonly Int32 Id;
        public Boolean IsDisposed;

        public HandlerLifetime(HashSet<Int32> lifetimeIds, Int32 id)
        {
            _lifetimeIds = lifetimeIds;
            Id = id;
        }

        private void Dispose(Boolean disposing)
        {
            if(!IsDisposed)
            {
                if(disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                _ = _lifetimeIds.Remove(Id);
                IsDisposed = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~HandlerLifetime()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public override String ToString() => $"Handler Lifetime {Id} ({(IsDisposed ? "disposed" : "alive")})";
    }
}