
namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public interface IFreezable
    {
        bool IsFrozen { get; }
        void Freeze();
    }
}
