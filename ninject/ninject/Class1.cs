using Ninject;

namespace ninjecttest
{
    class AuthService : IAuthService
    {
        public string Do()
        {
            return "auth";
        }
    }

    class CrapService : ICrapService
    {
        private readonly string _name;

        public CrapService(string name)
        {
            _name = name;
        }

        public string Do()
        {
            return _name;
        }
    }

    public class Services
    {
        [Inject]
        public IAuthService A { get; set; }

        [Inject]
        public IAuthService B { get; set; }

        public Services()
        {
        }
    }

    public class InjectMe
    {
        private readonly IAuthService _auth;
        private readonly ICrapService _crap;

        public InjectMe(IAuthService auth, ICrapService crap)
        {
            _auth = auth;
            _crap = crap;
        }

        public string DoAuth()
        {
            return _auth.Do();
        }

        public string DoCrap()
        {
            return _crap.Do();
        }
    }

    public interface ICrapService
    {
        string Do();
    }

    public interface IAuthService
    {
        string Do();
    }
}
