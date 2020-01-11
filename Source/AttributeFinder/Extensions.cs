using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.Helpers
{
	public static partial class AttributeFinder
	{
		public static ILArray ILLoadAttributes<TAttribute>(this ILGenerator IL, Action ParameterAction, Type ActionType)
		{
			ParameterAction();
			IL.LoadExternalType(typeof(TAttribute));

			var array = IL.CreateArray<TAttribute>(() =>
			{
				IL.Emit(OpCodes.Call, typeof(AttributeFinder).GetMethod(nameof(AttributeFinder.FindAttributes), new[] { ActionType, typeof(Type) }));
			});

			return array;
		}

		public static ILArray ILLoadAttributes<TAttribute>(this ILGenerator IL, MethodInfo Method)
		{			
			return IL.ILLoadAttributes<TAttribute>(() => IL.LoadExternalMethodInfo(Method), typeof(MethodInfo));
		}
	}
}