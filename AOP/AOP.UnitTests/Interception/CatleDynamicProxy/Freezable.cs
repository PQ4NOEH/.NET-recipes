using Castle.DynamicProxy;
using System;
using System.Linq;
using System.Collections.Generic;

namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public static class Freezable 
    {

        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        public static bool IsFreezable(object obj)
        {
            return GetFreezableInterceptor(obj) != null;
        }

        static FreezableInterceptor GetFreezableInterceptor(Object obj)
        {
            if (obj == null) return null;
            var hack = obj as IProxyTargetAccessor;
            if (hack == null) return null;
            return (hack.GetInterceptors().Where(i => i is FreezableInterceptor).FirstOrDefault()) as FreezableInterceptor;
        }

        public static void Freeze(object freezable)
        {
            var interceptor = GetFreezableInterceptor(freezable);
            if (interceptor == null) throw new NotFreezableObjectException(freezable);
            interceptor.Freeze();
        }

        public static bool IsFrozen(object freezable)
        {
            var interceptor = GetFreezableInterceptor(freezable);
            return interceptor != null && interceptor.IsFrozen;
        }

        public static TFreezable MakeFreezable<TFreezable>() where TFreezable : class, new()
        {
            var freezableInterceptor = new FreezableInterceptor();
            var proxyOptions = new ProxyGenerationOptions(new FreezableProxyGenerationHook());
            var proxy = _generator.CreateClassProxy<TFreezable>(proxyOptions, freezableInterceptor);
            
            return proxy;
        }
    }

    public class NotFreezableObjectException : Exception
    {
        public NotFreezableObjectException(object freezable) : base() { }
    }
}
