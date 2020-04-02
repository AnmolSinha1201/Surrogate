using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;
using Surrogate.Internal.BaseSurrogates;
using Surrogate.Internal.ILConstructs;
using Surrogate.Internal.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static LocalBuilder CreateMethodInterceptor(this ILGenerator IL, MethodInfo Method, MethodBuilder BackingMethod, ILArray Arguments)
		{
			var attributes = AttributeFinder.FindAttributes(Method, typeof(IMethodSurrogate));
			var ILAttributes = IL.ILLoadAttributes<IMethodSurrogate>(Method);
			var returnValue = IL.DeclareLocal(typeof(object));

			if (attributes.Count() == 0)
				ILAttributes = IL.CreateArray<IMethodSurrogate>(1, (i) => IL.CreateExternalType(typeof(MethodSurrogate), new Type[] {}));
			
			ILAttributes.ForEach((attribute) =>
			{
				attribute.Load();
				var info = IL.CreateMethodSurrogateInfo(BackingMethod, Arguments, returnValue);
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Callvirt, typeof(IMethodSurrogate).GetMethod(nameof(IMethodSurrogate.InterceptMethod), new [] { typeof(MethodSurrogateInfo) }));

				IL.CopyArrayToArgs(Method, Arguments);
				IL.ReturnMethodSurrogateInfoValue(info, returnValue);
			});

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Call, Method);
			IL.Emit(OpCodes.Box, Method.ReturnType);
			IL.Emit(OpCodes.Stloc, returnValue);

			return returnValue;
		}

		private static MethodBuilder CreateBackingMethod(this TypeBuilder Builder, MethodInfo OriginalMethod)
		{
			var parameters = OriginalMethod.GetParameters();

			MethodBuilder methodBuilder = Builder.DefineMethod(
				$"<{OriginalMethod.Name}>k__BackingMethod",
				OriginalMethod.Attributes,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameters.Select(i => i.ParameterType).ToArray()
			);
			
			// base.OriginalMethod(args);
			ILGenerator il = methodBuilder.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			for (int i = 0; i < parameters.Count(); i++)
				il.LoadArgument(i);
			il.Emit(OpCodes.Call, OriginalMethod);
			il.Emit(OpCodes.Ret);

			return methodBuilder;
		}

		private static LocalBuilder CreateMethodSurrogateInfo(this ILGenerator IL, MethodInfo BackingMethod, ILArray ArgumentsArray, LocalBuilder ReturnValue)
		{
			IL.Emit(OpCodes.Ldarg_0);
			IL.LoadExternalMethodInfo(BackingMethod);
			IL.Emit(OpCodes.Ldloc, ArgumentsArray.Address);
			IL.Emit(OpCodes.Ldloc, ReturnValue);

			var info = IL.CreateExternalType(typeof(MethodSurrogateInfo), new [] { typeof(object), typeof(MethodInfo), typeof(object[]), typeof(object) });
			return info;
		}

		// Array is object[]
		private static void CopyArrayToArgs(this ILGenerator IL, MethodInfo Method, ILArray ArgumentsArray)
		{
			var parameters = Method.GetParameters();

			// Args[i] = Value returned from Surrogate
			for (int i = 0; i < parameters.Count(); i++)
			{
				if (!parameters[i].IsByRefOrOut())
					continue;
			
				IL.LoadArgument(i);
				ArgumentsArray.LoadElementAt(i);
				IL.Emit(OpCodes.Unbox_Any, parameters[i].ActualParameterType());
				IL.StoreIntoAddress(parameters[i].ParameterType);
				// IL.Emit(OpCodes.Stind_I4);
			}
		}

		private static void ReturnMethodSurrogateInfoValue(this ILGenerator IL, LocalBuilder SurrogateInfoVariable, LocalBuilder ReturnValue)
		{
			IL.Emit(OpCodes.Ldloc, SurrogateInfoVariable);
			IL.Emit(OpCodes.Ldfld, typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue)));

			IL.Emit(OpCodes.Stloc, ReturnValue);
		}
	}
}