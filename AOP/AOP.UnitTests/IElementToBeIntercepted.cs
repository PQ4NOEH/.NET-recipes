using System;
namespace AOP.UnitTests
{
    public interface IElementToBeIntercepted
    {
        void MethodOne();
        void MethodWithArguments(int intArgument, string stringArgument);
    }
}
