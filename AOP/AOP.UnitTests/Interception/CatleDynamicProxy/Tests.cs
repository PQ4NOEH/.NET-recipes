using Castle.DynamicProxy;
using System.Linq;
using Xunit;

namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public class Tests
    {
        [Fact]
        public void When_calling_the_method_it_gets_intercepted()
        {
            var dp = new ProxyGenerator();
            var outputResult = new OutputToList();
            var target = new ElementToBeIntercepted(outputResult);
            var interceptor = new LogInterceptorWithoutProceed(outputResult);
            var proxy = dp.CreateInterfaceProxyWithTarget<IElementToBeIntercepted>(target, interceptor);
            proxy.MethodOne();
            Assert.Equal(outputResult.OutputText.Count, 1);
            Assert.Contains("The method MethodOne has been intercepted", outputResult.OutputText.First());
        }
        [Fact]
        public void If_the_interceptor_do_not_call_proceed_the_method_it_not_called()
        {
            var dp = new ProxyGenerator();
            var outputResult = new OutputToList();
            var target = new ElementToBeIntercepted(outputResult);
            var interceptor = new LogInterceptorWithoutProceed(outputResult);
            var proxy = dp.CreateInterfaceProxyWithTarget<IElementToBeIntercepted>(target, interceptor);
            proxy.MethodOne();
            Assert.Equal(outputResult.OutputText.Count, 1);
            Assert.Contains("The method MethodOne has been intercepted", outputResult.OutputText.First());
        }
        [Fact]
        public void If_the_interceptor_calls_proceed_the_method_is_called()
        {
            var dp = new ProxyGenerator();
            var outputResult = new OutputToList();
            var target = new ElementToBeIntercepted(outputResult);
            var interceptor = new LogInterceptorWithProceed(outputResult);
            var proxy = dp.CreateInterfaceProxyWithTarget<IElementToBeIntercepted>(target, interceptor);
            proxy.MethodOne();
            Assert.Equal(outputResult.OutputText.Count, 2);
            Assert.Contains("The method MethodOne has been intercepted", outputResult.OutputText.First());
            Assert.Contains("Called MethodOne", outputResult.OutputText.Last());
        }
        [Fact]
        public void Many_interceptors_can_be_applied()
        {
            var dp = new ProxyGenerator();
            var outputResult = new OutputToList();
            var target = new ElementToBeIntercepted(outputResult);
            var interceptor = new LogInterceptorWithProceed(outputResult);
            var secondInterceptor = new OtherLogInterceptor(outputResult);
            var proxy = dp.CreateInterfaceProxyWithTarget<IElementToBeIntercepted>(target, interceptor, secondInterceptor);
            proxy.MethodOne();
            Assert.Equal(outputResult.OutputText.Count, 3);
            Assert.Contains("The method MethodOne has been intercepted", outputResult.OutputText.First());
            Assert.Contains("with OtherLogInterceptor", outputResult.OutputText.Skip(1).First());
            Assert.Contains("Called MethodOne", outputResult.OutputText.Last());
        }
    }

    
}
