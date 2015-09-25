using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace AOP.UnitTests.Interception.CatleDynamicProxy
{
    public class FreezableProxyGenerationHook : IProxyGenerationHook
    {
        public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
        {
            return !memberInfo.Name.StartsWith("get_", StringComparison.Ordinal)
                && !memberInfo.Name.Equals("Equals")
                && !memberInfo.Name.Equals("GetHashCode");
        }

        public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public void MethodsInspected()
        {
        }


        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }
    }
}
