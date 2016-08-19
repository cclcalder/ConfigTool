using System;

namespace Model
{
    /// <summary>
    /// Certain actions and calculations we would like to be non-side-effecting
    /// i.e. not to fire recalculations of parents etc.
    /// </summary>
    class NonSideEffectingScope : IDisposable
    {
        private static readonly object LockObject = new object();
        private static bool _loading;

        private readonly bool _unsetLoading;

        public NonSideEffectingScope()
        {
            if (!_loading)
            {
                lock (LockObject)
                {
                    if (!_loading)
                    {
                        _loading = true;
                        _unsetLoading = true;
                    }
                }
            }
        }

        public static bool IsActive
        {
            get
            {
                lock(LockObject)
                {
                    return _loading;
                }
            }
        }

        public void Dispose()
        {
            if (_unsetLoading)
            {
                lock (LockObject)
                {
                    _loading = false;
                }
            }
        }
    }
}