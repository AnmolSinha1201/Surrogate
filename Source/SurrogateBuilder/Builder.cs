using System;
using Surrogate.Base;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		public static Type Build(Type ItemType)
		{
			var builder = ItemType.CreateTypeBuilder();
			var method = ItemType.GetMethod(nameof(Foo.ActualMethod));
			
			foreach (var attribute in Attribute.GetCustomAttributes(method))
				if (typeof(IMethodSurrogate).IsAssignableFrom(attribute.GetType()))
					builder.CreateMethodProxy(method, (Attribute)attribute);

			return builder.CreateType();
		}
	}
}