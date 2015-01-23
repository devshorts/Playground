using System;
using System.Threading;

namespace Future
{
    public class NewThreadFuture<T> : Future<T>
    {
        public NewThreadFuture(Func<T> function) : base(function)
        {
        }

        protected override void Execute(Action wrapped)
        {
            var runner = new Thread(new ThreadStart(wrapped));

            runner.Start();
        }

        public override Future<T> Then(Func<T> next)
        {
            return new NewThreadFuture<T>(ThenWithoutResult(next));
        }

        public override Future<Y> Then<Y>(Func<T, Y> next)
        {
            return new NewThreadFuture<Y>(ThenWithResult(next));
        }
    }
}