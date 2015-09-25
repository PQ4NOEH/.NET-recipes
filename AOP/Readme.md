# AOP
AOP is the acronym of Aspect oriented programming and it's programming paradigm that aims to increase code modularity by allowing the separation of cross-cutting concerns.

## Principal approach
+ Remoting proxies MarshalByRefObject
  + Advantages
    + Easy to implement .NET native support
  + Disadvantages
    + Heavy
	+ Only on MarshalByRefObject
+ ContextBoundObject
  + Advantages
    + Very easy to implement
	+ Native support for call implementation
  + Disadvantages
    + Performance costly
+ compile-time subclassing (Rhino proxy)
  + Advantages
    + Easiest to understand
  + Disadvantages
    + Interfaces or virtual methods only
+ Run-time subclassing (Castle Dynamic proxy)
  + Advantages
    + Easiest to understand
	+ Very flexible
  + Disadvantages
    + Complex implementation
	+ Interfaces or virtual methods only
+ Hooking into the profiler API (Typemock)
  + Advantages
    + Extremely powerful 
  + Disadvantages
    + Complex implementation
+ Compile time Il-weaving (post sharp || cecil)
  + Advantages
    + Very powerful 
	+ Good performance
  + Disadvantages
    + Very hard to implement
+ Run time Il-weaving (post sharp || cecil)
  + Advantages
    + Very powerful 
	+ Good performance
  + Disadvantages
    + Very hard to implement

## tools
+ On top of CECIL
  + [FODY](https://github.com/Fody/Fody)
  + [linfu](https://github.com/philiplaureano/LinFu)
+ On top of Typemock CThru] (http://cthru.codeplex.com/)
+ [Postsharp](https://www.postsharp.net/)
+ Castle dynamicProxy
  + 

## REF
+ [Ayende rahien article](http://ayende.com/blog/2615/7-approaches-for-aop-in-net)
+ [AOP with FODY](http://simoncropp.com/simpleaopwithfody)