using AOP.UnitTests;
using Castle.DynamicProxy;

namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public class LogInterceptorWithoutProceed : IInterceptor
    {
        protected readonly IOutput _output;
        public LogInterceptorWithoutProceed(IOutput output)
        {
            _output = output;
        }
        public virtual void Intercept(IInvocation invocation)
        {
            _output.WriteLine(string.Format("The method {0} has been intercepted", invocation.Method.Name));
        }
    }
    public class LogInterceptorWithProceed : LogInterceptorWithoutProceed
    {
        public LogInterceptorWithProceed(IOutput output)
            : base(output){}
        public override void Intercept(IInvocation invocation)
        {
            base.Intercept(invocation);
            invocation.Proceed();
        }
    }

    public class OtherLogInterceptor : LogInterceptorWithoutProceed
    {
        public OtherLogInterceptor(IOutput output)
            : base(output) { }
        public override void Intercept(IInvocation invocation)
        {
            _output.WriteLine(string.Format("The method {0} has been intercepted with OtherLogInterceptor.", invocation.Method.Name));
            invocation.Proceed();
        }
    }
}
