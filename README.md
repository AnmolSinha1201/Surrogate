# Surrogate
Surrogate provides AOP interceptors for C# methods, method's parameters and return values. These can be intercepted and their valus can be read and modified. The system is extendable, and anyone can provide additional attributes which can be used seamlessly.


# MethodSurrogate
This surrogate provides a method-wide hook which can be installed to
- Inspect the parameters going into the method.
- Inspect the ref/out parameters from and to the method.
- Inspect the return value of the method.
- Bypass the method call altogether.
- Reroute the method call to some other method.
- Inspect the current state of the object which "owns" the method.

Please note, that in case `ParameterSurrogate` or `ReturnSurrogate` is installed, a stub `MethodSurrogate` is installed automatically to intercept the parameters or return values of the method. 
Also note that this requires that the method should be declared `virtual` for this interceptor to function propely. 
Furthermore, the order of execution of these surrogates depend on the framework and is not guaranteed by `Surrogte`. What this means is, if any changes are done to the parameter, then the changes should be independent.

`MethodSurrogate` also supports `ref`, `out`, `variadic`/`param` parameter types.

To implement your own `MethodSurrogate` attribute, inherit from `IMethodSurrogate` and implement the appropiate method. It is also required to call `.Execute()` method to actually execute the original method.

# ParameterSurrogate
This surrogate provides an interceptor for the parameters. This surrogate is called before invoking the `MethodSurrogate`s, and therefore all the `ParameterSurrogate`s are processed before continueing to the next step. By installing this hook, one can:
- Inspect the parameter that goes into the method.
- Change the value of paramter that goes into the method.

It supports `ref`, `out`, `variadic`/`param` parameter types.

Please note that the order of execution of `ParameterSurrogate`s depend on the framework and is not guaranteed by `Surrogate`. What this means is, if any changes are done to the parameter, then the changes should be independent. 
Also note that the method on which this surrogate is used should be `virtual` to function properly. 
Furthermore, if this type of hook is installed, it automatically installs a `MethodSurrogate` stub to function properly.

To implement your own `ParameterSurrogate` attribute, inherit from `IParamterSurrogate` and implement the appropiate method.

# ReturnSurrogate
This surrogate provites an interceptor for the return values. This surrogate is called after invoking `MethodSurrogate`, therefore the return value received might be modified by `MethodSurrogate`. By installing this hook, one can:
- Inspect the return value from the method.
- Change the return value of a method.

Please note that the `void` methods return a `null`, just like in `Reflection`. To check whether the `null` is returned from the actual method or is due to `void` return type, consider getting the return type from `MethodInfo` provided in the `MethodSurrogateInfo`. 
Also note that the method on which this is being used must be declared `virtual`. 
Furthermore, order of execution of `ReturnSurrogate`s depend on the framework, and is not guaranteed by `Surrogate`. What this means is, if any changes are done to the parameter, then the changes should be independent. Also note that if this type of hook is installed, it automatically installs a `MethodSurrogate` to function properly.

To implement your own `ReturnSurrogate` attribute, inherit from `IReturnSurrogate` and implement the appropiate method.

# Roadmap / WIP
Feature| Status
---|---
FieldSurrogate|Cancelled *
PropertySurrogate|WIP
ClassSurrogate|WIP **
Ref Returns|Cancelled ***

\* = Not feasible, as `FieldSurrogate`s would require converting them into properties, which means we cannot inherit from the original class (as converting from field to property means there would be two members with the name). Even the classical way of C# notifications require that we use properties and not objects. Even if create an entire new object from scratch, we would not be able to back cast it into original type, which means we would be reliant on reflection for this implementation.

That said, I am still exploring the possibility of using `DynamicObject` API to implement this. But this would mean that we would have to use `dynamic` types and we would lose the strict type checking and autocompletion and all the other goodies.

\*\* = `MethodSurrogate` already provies a decent way of checking the state of the object. Implementing a separate `ClassSurrogate` would mean either
- A class containing only virtual methods and properties, which will quickly become cumbersome.
- A class with any thing allowed, which means that we would have to convert fields into properties and convert non virtual methods into virtual ones. With this, we can't back cast the output instance into original type and we will miss out on the strict type checking and autocompletion. We would be reliant on reflection on this implementation.
- Use `DynamicObject` API which would mean we would have to use `dynamic` data type and thus, every thing would be late bound. This also means there would be no compile time safety. This is still a lot better than the second approach.
- A hybrid approach, where we check the members of the class and based on whether they satisfy the first condition, chose first or second or third approach.

\*\*\* = Ref returns cannot be supported due to limitations of C#, reflection in particular. `System.Reflection`'s `Invoke` does not return a ref, therefore even though the original method returns a ref, it cannot be chained back via surrogates.

Please let me know what you guys think and report any bugs using "issues".
