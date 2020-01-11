using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Helpers;
using Surrogate.Interfaces;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		public static Type Build(Type ItemType)
		{
			var builder = ItemType.CreateTypeBuilder();
			// var method = ItemType.GetMethod(nameof(Foo.ActualMethod));
			var methods = ItemType.GetMethods();
			foreach (var method in methods)
			{
				if (!method.IsVirtual)
					continue;

				MethodWorkflowDispatch(builder, method);
			}

			return builder.CreateType();
		}

		

		private static void MethodWorkflowDispatch(TypeBuilder Builder, MethodInfo Method)
		{
			var methodAttributes = AttributeFinder.FindAttributes(Method, typeof(IMethodSurrogate));
			var parameterAttributes = AttributeFinder.FindAttributes(Method.GetParameters(), typeof(IParameterSurrogate));
			var returnAttributes = AttributeFinder.FindAttributes(Method, typeof(IReturnSurrogate));

			if (methodAttributes.Length == 0 && parameterAttributes.Length == 0 && returnAttributes.Length == 0)
				return;
			
			Builder.CreateMethodProxy(Method);
		}
	}
}