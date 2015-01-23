using System;
using System.Threading;

namespace Future
{
    public abstract class Future<T>
    {
        private bool _isComplete;

        private ManualResetEvent _mutex = new ManualResetEvent(false);

        private Exception _ex;

        private readonly object _lock = new object();

        private T _result;

        public Future(Func<T> function)
        {
            Execute(Wrapped(function));
        }

        protected abstract void Execute(Action wrapped);

        private Action Wrapped(Func<T> function)
        {
            return () =>
            {
                try
                {
                    _result = function();

                    lock (_lock)
                    {
                        _isComplete = true;
                    }
                }
                catch (Exception ex)
                {
                    _ex = ex;
                }
                finally
                {
                    _mutex.Set();
                }
            };
        }

        public T Resolve()
        {
            lock (_lock)
            {
                if (_isComplete)
                {
                    return _result;
                }
            }

            _mutex.WaitOne();

            if (_ex != null)
            {
                throw _ex;
            }

            return _result;        
        }

        public abstract Future<T> Then(Func<T> next);

        protected Func<T> ThenWithoutResult(Func<T> next)
        {
            return () =>
            {
                Resolve();

                return next();
            };
        }

        protected Func<Y> ThenWithResult<Y>(Func<T, Y> next)
        {
            return () =>
            {
                var previousResult = Resolve();

                return next(previousResult);
            };
        }

        public abstract Future<Y> Then<Y>(Func<T, Y> next);
    }
}
