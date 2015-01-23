using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy;

namespace NoNulls
{
    #region Primitive Checker

    /*
     * Code to check primitive types, taken from stack overflow user
     * Ronnie Overby: http://stackoverflow.com/a/15578098/310196
     */

    internal static class PrimitiveTypes
    {
        public static readonly Type[] List;

        static PrimitiveTypes()
        {
            var types = new[]
                        {
                            typeof (Enum),
                            typeof (String),
                            typeof (Char),

                            typeof (Boolean),
                            typeof (Byte),
                            typeof (Int16),
                            typeof (Int32),
                            typeof (Int64),
                            typeof (Single),
                            typeof (Double),
                            typeof (Decimal),

                            typeof (SByte),
                            typeof (UInt16),
                            typeof (UInt32),
                            typeof (UInt64),

                            typeof (DateTime),
                            typeof (DateTimeOffset),
                            typeof (TimeSpan),
                        };


            var nullTypes = from t in types
                            where t.IsValueType
                            select typeof(Nullable<>).MakeGenericType(t);

            List = types.Concat(nullTypes).ToArray();
        }

        public static bool Test(Type type)
        {
            if (List.Any(x => x.IsAssignableFrom(type)))
                return true;

            var nut = Nullable.GetUnderlyingType(type);
            return nut != null && nut.IsEnum;
        }
    }

    #endregion

    #region Marker interface to unbox proxy

    public interface IUnBoxProxy
    {
        object Value { get; }
    }

    #endregion

    #region Interceptor

    public enum CastType
    {
        List,
        Dictionary,
        Default
    }

    public class NeverNullInterceptor : IInterceptor
    {
        private static readonly MethodInfo ProxyGenericIteratorMethod =
            typeof(NeverNullInterceptor)
                .GetMethod(
                    "ProxyGenericIterator",
                    BindingFlags.NonPublic | BindingFlags.Static);
 

        private object Source { get; set; }

        public NeverNullInterceptor(object source)
        {
            Source = source;
        }

        private bool IsGenericType(Type source, Type target)
        {
            try
            {
                if (!target.IsGenericType)
                {
                    return false;
                }

                return source.GetInterfaces().Any(t => t.GetGenericTypeDefinition() == target);               
            }
            catch
            {
            }

            return false;
        }

        public void Intercept(IInvocation invocation)
        {            
            if (invocation.Method.DeclaringType == typeof (IUnBoxProxy))
            {
                invocation.ReturnValue = Source;
                return;
            }

            var returnType = invocation.Method.ReturnType;

            if (invocation.Method.ReturnType.IsSubclassOf(typeof (IEnumerable)))
            {
                HandleNonGenericIteratorInvocation(invocation);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
            {
                HandleGenericIteratorInvocation(invocation, CastType.Default);
            }
            else if (IsGenericType(returnType, typeof (IList<>)))
            {
                HandleGenericIteratorInvocation(invocation, CastType.List);
            }
            else
            {
                invocation.Proceed();
                
                var returnValue = Convert.ChangeType(invocation.ReturnValue, invocation.Method.ReturnType);

                if (!PrimitiveTypes.Test(invocation.Method.ReturnType))
                {
                    invocation.ReturnValue = returnValue == null
                                                 ? ProxyExtensions.NeverNullProxy(invocation.Method.ReturnType)
                                                 : ProxyExtensions.NeverNull(returnValue,
                                                                             invocation.Method.ReturnType);
                }
            }
        }

        private static void HandleNonGenericIteratorInvocation(IInvocation invocation)
        {
            invocation.Proceed();
            invocation.ReturnValue = ProxyNonGenericIterator(
                invocation.InvocationTarget,
                invocation.ReturnValue as IEnumerable);
        }

        private static void HandleGenericIteratorInvocation(IInvocation invocation, CastType cast)
        {
            invocation.Proceed();

            var genericType = invocation.Method.ReturnType.GetGenericArguments()[0];

            var method = ProxyGenericIteratorMethod.MakeGenericMethod(genericType);

            var result = method.Invoke(null, new[] {invocation.InvocationTarget, invocation.ReturnValue, cast});

            invocation.ReturnValue = result;
        }

        private static IEnumerable<T> ProxyGenericIterator<T>(
            object target, IEnumerable enumerable, CastType cast)
        {
            var proxied = ProxyNonGenericIterator(target, enumerable).Cast<T>();

            switch(cast)
            {
                case CastType.List:
                    return proxied.ToList();                
                                
                default:
                    return proxied;
            }
        }

        private static IEnumerable ProxyNonGenericIterator(object target, IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                yield return ProxyExtensions.NeverNullProxy(target.GetType());
                yield break;
            }

            foreach (var element in enumerable)
            {
                yield return ProxyExtensions.NeverNull(element, target.GetType());
            }
        }

    }

    #endregion

    #region Proxy extensions to create never null proxies

    public static class ProxyExtensions
    {
        
        private static ProxyGenerator _generator = new ProxyGenerator();        

        public static object NeverNullInterfaceProxy(Type t)
        {
            return _generator.CreateInterfaceProxyWithoutTarget(t, new[] { typeof(IUnBoxProxy) }, new NeverNullInterceptor(null));
        }

        public static object NeverNullInterface(object source, Type t)
        {
            return _generator.CreateInterfaceProxyWithTarget(t, new[] { typeof(IUnBoxProxy) }, source,  new NeverNullInterceptor(null));
        }

        public static object NeverNullProxy(Type t)
        {            
            return _generator.CreateClassProxy(t, 
                                                new[] { typeof(IUnBoxProxy) }, 
                                                new NeverNullInterceptor(null));
        }

        public static object NeverNull(object source, Type type)
        {
            return _generator.CreateClassProxyWithTarget(type, new[] { typeof(IUnBoxProxy) }, source,
                                                         new NeverNullInterceptor(source));
        }

        public static T NeverNull<T>(this T source) where T : class
        {
            return
                (T) 
                _generator.CreateClassProxyWithTarget(typeof(T), new[] { typeof(IUnBoxProxy) }, source,
                                                      new NeverNullInterceptor(source));
        }

        public static T Final<T>(this T source)
        {
            var proxy = (source as IUnBoxProxy);
            if (proxy == null)
            {
                return source;
            }

            return (T)proxy.Value;
        }
    }

    #endregion

}
