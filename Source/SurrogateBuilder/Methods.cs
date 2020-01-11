using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;
using Surrogate.Helpers;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static LocalBuilder CreateMethodProxy(this TypeBuilder Builder, MethodInfo OriginalMethod)
		{
			var parameterTypes = OriginalMethod.GetParameters().Select(i => i.ParameterType).ToArray();
			MethodBuilder methodBuilder = Builder.DefineMethod(
				OriginalMethod.Name,
				OriginalMethod.Attributes,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameterTypes
			);
			var backingMethod = Builder.CreateBackingMethod(OriginalMethod);
			ILGenerator il = methodBuilder.GetILGenerator();
			var args = il.CreateParameterProxy(OriginalMethod);
			
			// Attribute.InterceptMethod(MethodSurrogateInfo)
			var attributes = AttributeFinder.FindAttributes(OriginalMethod, typeof(IMethodSurrogate));
			var ILAttributes = il.ILLoadAttributes<IMethodSurrogate>(OriginalMethod);
			var returnValue = il.DeclareLocal(typeof(object));

			for (int i = 0; i < attributes.Count(); i++)
			{
				ILAttributes.LoadElementAt(i);		
				var info = il.CreateMethodSurrogateInfo(backingMethod, args, returnValue);
				il.Emit(OpCodes.Ldloc, info);
				il.Emit(OpCodes.Call, attributes[i].GetType().GetMethod(nameof(IMethodSurrogate.InterceptMethod), new [] { typeof(MethodSurrogateInfo) }));

				il.CopyArrayToArgs(OriginalMethod, args);
				il.ReturnMethodSurrogateInfoValue(info, OriginalMethod.ReturnType, returnValue);
			}

			il.CreateReturnProxy(OriginalMethod, returnValue);

			il.Emit(OpCodes.Ldloc, returnValue);	
			il.Emit(OpCodes.Unbox_Any, OriginalMethod.ReturnType);
			il.Emit(OpCodes.Ret);

			Builder.DefineMethodOverride(methodBuilder, OriginalMethod);

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

		private static LocalBuilder ReturnMethodSurrogateInfoValue(this ILGenerator IL, LocalBuilder SurrogateInfoVariable, Type ReturnType, LocalBuilder ReturnValue)
		{
			IL.Emit(OpCodes.Ldloc, SurrogateInfoVariable);
			IL.Emit(OpCodes.Ldfld, typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue)));

			IL.Emit(OpCodes.Stloc, ReturnValue);
			return ReturnValue;
		}
	}
}