using System;
using System.Threading;

namespace Future
{
    public class NewThreadPoolFuture<T> : Future<T>
    {
        public NewThreadPoolFuture(Func<T> function)
            : base(function)
        {
        }

        protected override void Execute(Action wrapped)
        {
            ThreadPool.QueueUserWorkItem(state => wrapped());
        }

        public override Future<T> Then(Func<T> next)
        {
            return new NewThreadPoolFuture<T>(ThenWithoutResult(next));
        }

        public override Future<Y> Then<Y>(Func<T, Y> next)
        {
            return new NewThreadPoolFuture<Y>(ThenWithResult(next));
        }
    }
}