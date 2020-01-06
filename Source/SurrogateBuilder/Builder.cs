using System;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		public static Type Build(Type ItemType)
		{
			var builder = ItemType.CreateTypeBuilder();
			var method = ItemType.GetMethod(nameof(Foo.ActualMethod));
			builder.CreateMethodProxy(method);

			return builder.CreateType();
		}
	}
}