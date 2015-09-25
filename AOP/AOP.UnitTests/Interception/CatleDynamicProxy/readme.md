# Castle DynamicProxy

## Description
It is not really AOP what it allows it's to generate a class proxy during runtime. Limitations:
+ Can not proxy sealed or non public classes.
+ Can not act in to the intercepted method, yo only can define decide to execute or not intercepted, execute code before and execute code after.

Most of the limitations comes from the fact that the DP approach followed in DynamicProxy is to subclassing given class and overriding it's methods.

### Proxy types
#### Inheritance-based
They are created by inheriting a proxy class
#### Composition-based


## Steps to decorate class
1. Create instance of Castle.DynamicProxy.ProxyGenerator
2. Create instance of class to be intercepted
3. Create an instance of the current interceptor
4. Create proxy ProxyGenerator.CreateInterfaceProxyWithTarget<IElementToBeIntercepted>();
5. call the method you want to execute

## Usages
1. Lazy loading (NHibernate uses it for this)
2. Mocking (Moq or Rhino Mocks)
3. Decorating method class for crosscutting concerns  

## References
+ [Castle project documentation](https://github.com/castleproject/Core/blob/master/docs/dynamicproxy.md)
+ [Proxying in detail](http://kozmic.net/dynamic-proxy-tutorial/) || [project samples](https://onedrive.live.com/?id=6E18E107780D3F4A%21145&cid=6E18E107780D3F4A&group=0&parId=6E18E107780D3F4A%21144&action=locate)
+ [wikipedia proxy pattern](https://en.wikipedia.org/wiki/Proxy_pattern)