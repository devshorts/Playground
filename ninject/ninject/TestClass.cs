using System;
using Ninject;
using Xunit;

namespace ninjecttest
{
    public class TestClass
    {
        [Fact]
        public void Inject()
        {
            var kernel = new StandardKernel();

            kernel.Bind<IAuthService>().To<AuthService>();

            kernel.Bind<ICrapService>().ToConstructor(args => new CrapService("bar"));

            var inject = kernel.Get<InjectMe>();

            Console.WriteLine(inject.DoAuth());

            Console.WriteLine(inject.DoCrap());

            var service = kernel.Get<Services>();

            Console.WriteLine(service.A.Do());
        }
    }
}