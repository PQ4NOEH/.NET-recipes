# Type Validation
Validate data structures is esential in coding.
As far as i know five strategies exist
1. Guards
  + The simplest startegy
  + It dont allow to get all broken rules as it throws an exception when try to set an invalid state
  + You dont have to validate anything as with modeling but this strategy is less ideal because it is not as explicit as the modelling
2. DataAnotations
  + Very easy.
  + couples the class and the rules which i think is very nice.
  + Allows deserialization scenarios
3. Design by contract
  + Maybe to cumbersome in some scenarios
  + If you install the VSE and asume the extra compiling time wont compile if your code brokes any contract
  + It is posible to atach contracts to interfaces.
4. FluentValidation
  + Nice API
  + Very flexible
  + Allows deserialization scenarios
  + Rules and object are decoupled there might be an scenerio where that is an advantage but i cant figure which.
5. Modeling 
  + Requires an extra effort but once you have committed with it i think provides a cleaner API
  + It wont allow some deserailization scenarios like a simple JsonConvert.Deserializate.
  + The nice thing is that what is an invalid state is not posible you don't have to validate anything because the state to occur is imposible.


To Ilustrate We are going to set a simple use case.
We have a plain Data structure Citizen whith the following properties and rules
 Age : int => cant be lower than 16
 birtDate : DateTime => DateTime.Now - birtDate >= 16
 Name : Cant be null or whiteString && length between 3 and 50


# conclusions to the implementation
+ Code contracts: This scenario is not really suitable for code contract there main focus is on finding code bugs not on data structures validation
+ Data Anotation: I couldn't make them work
+ Fluent API: really Easy and flexible but not part of the framework and the validation is out of the data structure.
+ modeling: 

# refs
+ [DataAnnotations extensions](https://www.nuget.org/packages/DataAnnotationsExtensions)


