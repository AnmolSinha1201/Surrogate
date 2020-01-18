using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;
using Surrogate.Samples;
using System.Collections.Generic;
using Surrogate.Internal.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static void CreateReturnProxy(this ILGenerator IL, MethodInfo Method, LocalBuilder ReturnValue)
		{
			var ILAttributes = IL.ILLoadAttributes<IReturnSurrogate>(Method);

			ILAttributes.ForEach(attribute =>
			{
				attribute.Load();
				var info = IL.CreateReturnSurrogateInfo(Method, ReturnValue);
				IL.Emit(OpCodes.Ldloc, info);
				
				IL.Emit(OpCodes.Callvirt, typeof(IReturnSurrogate).GetMethod(nameof(IReturnSurrogate.InterceptReturn), new[] { typeof(ReturnSurrogateInfo) }));
				
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Ldfld, typeof(ReturnSurrogateInfo).GetField(nameof(ReturnSurrogateInfo.Value)));
				IL.Emit(OpCodes.Stloc, ReturnValue);
			});
		}

		private static LocalBuilder CreateReturnSurrogateInfo(this ILGenerator IL, MethodInfo Method, LocalBuilder ReturnValue)
		{
			IL.LoadExternalMethodInfo(Method);
			IL.Emit(OpCodes.Ldloc, ReturnValue);

			return IL.CreateExternalType(typeof(ReturnSurrogateInfo), new[] { typeof(MethodInfo), typeof(object) });
		}
	}
}