![Test](https://github.com/AnmolSinha1201/Surrogate/workflows/Test/badge.svg?branch=master)

[Documentation (Wiki)](https://github.com/AnmolSinha1201/Surrogate/wiki)

# Surrogate
Surrogate is a `Runtime Weaving Aspect Oriented Programming` framework. It helps developers write cleaner code. As the name description suggest, Surrogate (unlike `PostSharp`) does it's work during Runtime which means, it can deal with types which didn't exist during compilation time.

This means
- Surrogate does not rely on changing the source code that can potentially create problems with reflection.
- In order to use Surrogate, one just needs the `.dll`.
- No further compilation step is needed, so it can work with even more editing setups.

Surrogate uses c# `Attributes` create aspects which allows for "reusable components".
Using Surrogate is easy as well. A typical code usually looks like:
```cs
public partial class Foo
{
	[return: Clamp(5, 10)]
	[Log]
	public virtual int ActualMethod([UpperCase] string InputText, ref int InputNum)
	{
		Console.WriteLine("Actual Method");
		Console.WriteLine($"Received : {InputText}");
		InputNum = 123;
		return InputNum + 1;
	}
}
```

and instead of creating objects the regular way, we do it
```cs
var instance = SurrogateBuilder.Build<Foo>("Param1", 12345);
```

and then we use it like any other C# object
```cs
var retVal = instance.ActualMethod(inputText, ref num);
```

# Planned Features
Feature| Status
---|---
PropertySurrogate|WIP
ClassSurrogate|WIP
Better DynamicTypeBuilder|WIP
