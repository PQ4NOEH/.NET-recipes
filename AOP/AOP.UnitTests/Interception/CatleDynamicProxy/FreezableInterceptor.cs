using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public class FreezableInterceptor: IInterceptor, IFreezable
    {
        public int InterceptedCallsCount { get; private set; }
        bool _isFrozen;
        public bool IsFrozen
        {
            get { return _isFrozen; }
        }

        public void Freeze()
        {
            _isFrozen = true;
        }

        public void Intercept(IInvocation invocation)
        {
            InterceptedCallsCount++;
            if (_isFrozen && invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
            {
                throw new ObjectFrozenException();
            }

            invocation.Proceed();
        }
    }

    public class ObjectFrozenException : Exception
    {
        readonly string _methodName;
        public ObjectFrozenException()
        {
            _methodName = "A methodName";
        }
        public string TellmeTheMethodName()
        {
            return _methodName;
        }
    }
}
