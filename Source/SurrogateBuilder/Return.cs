using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;
using Surrogate.Samples;
using System.Collections.Generic;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static void CreateReturnProxy(this ILGenerator IL, MethodInfo Method, LocalBuilder ReturnValue)
		{
			var attributes = AttributeFinder.FindAttribute(Method, typeof(IReturnSurrogate));
			var ILAttributes = IL.CreateArray<IReturnSurrogate>(() =>
			{
				IL.ILFindAttribute(Method, typeof(IReturnSurrogate));
			});


			for (int i = 0; i < attributes.Count(); i++)
			{
				ILAttributes.LoadElementAt(i);
				var info = IL.CreateReturnSurrogateInfo(Method, ReturnValue);
				IL.Emit(OpCodes.Ldloc, info);
				
				IL.Emit(OpCodes.Call, attributes[i].GetType().GetMethod(nameof(IReturnSurrogate.InterceptReturn), new[] { typeof(ReturnSurrogateInfo) }));
				
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Ldfld, typeof(ReturnSurrogateInfo).GetField(nameof(ReturnSurrogateInfo.Value)));
				IL.Emit(OpCodes.Stloc, ReturnValue);
			}
		}

		private static LocalBuilder CreateReturnSurrogateInfo(this ILGenerator IL, MethodInfo Method, LocalBuilder ReturnValue)
		{
			IL.LoadExternalMethodInfo(Method);
			IL.Emit(OpCodes.Ldloc, ReturnValue);

			return IL.CreateExternalType(typeof(ReturnSurrogateInfo), new[] { typeof(MethodInfo), typeof(object) });
		}
	}
}