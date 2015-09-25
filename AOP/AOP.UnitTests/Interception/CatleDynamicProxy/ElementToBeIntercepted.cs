
namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public class ElementToBeIntercepted: AOP.UnitTests.IElementToBeIntercepted 
    {
        readonly IOutput _output;
       
        public ElementToBeIntercepted(IOutput output)
        {
            _output = output;
        }
        public void MethodOne()
        {
            _output.WriteLine("Called MethodOne");
        }

        public void MethodWithArguments(int intArgument, string stringArgument)
        {
            _output.WriteLine("Called MethodWithArguments");
        }
    }
}
