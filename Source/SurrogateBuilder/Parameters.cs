using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;
using Surrogate.Internal.ILConstructs;
using Surrogate.Internal.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static ILArray CreateArgumentsArray(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();
			var ILArguments = IL.CreateArray<object>(parameters.Count(), (i) =>
			{
				IL.LoadArgument(i, parameters[i]);
				IL.Emit(OpCodes.Box, parameters[i].ActualParameterType());
			});
			
			return ILArguments;
		}
	}
}