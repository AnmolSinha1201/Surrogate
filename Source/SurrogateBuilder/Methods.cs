using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;
using Surrogate.Samples;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static LocalBuilder CreateMethodInterceptor(this ILGenerator IL, MethodInfo Method, MethodBuilder BackingMethod, LocalBuilder Arguments)
		{
			var attributes = AttributeFinder.FindAttributes(Method, typeof(IMethodSurrogate));
			if (attributes.Count() == 0)
				return IL.InstallMethodSurrogateStub(Method, BackingMethod, Arguments);

			var ILAttributes = IL.ILLoadAttributes<IMethodSurrogate>(Method);
			var returnValue = IL.DeclareLocal(typeof(object));

			for (int i = 0; i < attributes.Count(); i++)
			{
				ILAttributes.LoadElementAt(i);		
				var info = IL.CreateMethodSurrogateInfo(BackingMethod, Arguments, returnValue);
				IL.Emit(OpCodes.Ldloc, info);
				IL.Emit(OpCodes.Call, attributes[i].GetType().GetMethod(nameof(IMethodSurrogate.InterceptMethod), new [] { typeof(MethodSurrogateInfo) }));

				IL.CopyArrayToArgs(Method, Arguments);
				IL.ReturnMethodSurrogateInfoValue(info, Method.ReturnType, returnValue);
			}

			return returnValue;
		}

		private static LocalBuilder InstallMethodSurrogateStub(this ILGenerator IL, MethodInfo Method, MethodBuilder BackingMethod, LocalBuilder Arguments)
		{
			var ILAttribute = IL.CreateExternalType(typeof(MethodSurrogateStub), new Type[] {});
			var returnValue = IL.DeclareLocal(typeof(object));

			IL.Emit(OpCodes.Ldloc, ILAttribute);
			var info = IL.CreateMethodSurrogateInfo(BackingMethod, Arguments, returnValue);
			IL.Emit(OpCodes.Ldloc, info);
			IL.Emit(OpCodes.Call, typeof(MethodSurrogateStub).GetMethod(nameof(IMethodSurrogate.InterceptMethod), new [] { typeof(MethodSurrogateInfo) }));

			IL.CopyArrayToArgs(Method, Arguments);
			IL.ReturnMethodSurrogateInfoValue(info, Method.ReturnType, returnValue);

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

		private static LocalBuilder CreateMethodSurrogateInfo(this ILGenerator IL, MethodInfo BackingMethod, LocalBuilder ArgumentsArrayVariable, LocalBuilder ReturnValue)
		{
			IL.Emit(OpCodes.Ldarg_0);
			IL.LoadExternalMethodInfo(BackingMethod);
			IL.Emit(OpCodes.Ldloc, ArgumentsArrayVariable);
			IL.Emit(OpCodes.Ldloc, ReturnValue);

			var info = IL.CreateExternalType(typeof(MethodSurrogateInfo), new [] { typeof(object), typeof(MethodInfo), typeof(object[]), typeof(object) });
			return info;
		}

		// Array is object[]
		private static void CopyArrayToArgs(this ILGenerator IL, MethodInfo Method, LocalBuilder LocalArray)
		{
			var parameters = Method.GetParameters();

			// Args[i] = Value returned from Surrogate
			for (int i = 0; i < parameters.Count(); i++)
			{
				if (!parameters[i].IsByRefOrOut())
					continue;
			
				IL.LoadArgument(i);
				IL.Emit(OpCodes.Ldloc, LocalArray);
				IL.LoadConstantInt32(i);
				IL.Emit(OpCodes.Ldelem_Ref);
				IL.Emit(OpCodes.Unbox_Any, parameters[i].ActualParameterType());
				IL.StoreIntoAddress(parameters[i].ParameterType);
				// IL.Emit(OpCodes.Stind_I4);
			}
		}

		private static void ReturnMethodSurrogateInfoValue(this ILGenerator IL, LocalBuilder SurrogateInfoVariable, Type ReturnType, LocalBuilder ReturnValue)
		{
			IL.Emit(OpCodes.Ldloc, SurrogateInfoVariable);
			IL.Emit(OpCodes.Ldfld, typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue)));

			IL.Emit(OpCodes.Stloc, ReturnValue);
		}
	}
}