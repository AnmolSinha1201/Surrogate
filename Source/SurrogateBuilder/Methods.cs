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
		private static void CreateMethodProxy(this TypeBuilder Builder, MethodInfo OriginalMethod, Attribute AttributeInfo)
		{
			var parameterTypes = OriginalMethod.GetParameters().Select(i => i.ParameterType).ToArray();
			MethodBuilder methodBuilder = Builder.DefineMethod(
				OriginalMethod.Name,
				MethodAttributes.Public
				| MethodAttributes.HideBySig
				| MethodAttributes.NewSlot
				| MethodAttributes.Virtual
				| MethodAttributes.Final,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameterTypes
			);

			ILGenerator il = methodBuilder.GetILGenerator();

			var backingMethod = Builder.CreateBackingMethod(OriginalMethod);

			// il.Emit(OpCodes.Ldarg_2);
			// il.Emit(OpCodes.Ldind_I4);
			// var writeLine = typeof(System.Console).GetMethod(nameof(Console.WriteLine), new Type[] { typeof(int) });
			// il.Emit(OpCodes.Call, writeLine);

			il.LoadExternalAttribute(OriginalMethod, AttributeInfo.GetType());
			

			il.Emit(OpCodes.Ldarg_0);
			il.LoadExternalMethodInfo(backingMethod);

			var args = il.CreateArrayFromArgs(OriginalMethod);
			il.Emit(OpCodes.Ldloc, args);

			// il.Emit(OpCodes.Ldloc, args);
			// il.Emit(OpCodes.Ldc_I4_1);
			// il.Emit(OpCodes.Ldelem_Ref);
			// il.Emit(OpCodes.Unbox_Any, typeof(int));
			// il.Emit(OpCodes.Call, writeLine);

			var info = il.CreateExternalType(typeof(MethodSurrogateInfo), new [] { typeof(object), typeof(MethodInfo), typeof(object[]) });
			il.Emit(OpCodes.Ldloc, info);
			var interceptMethodInfo = AttributeInfo.GetType().GetMethod(nameof(IMethodSurrogate.InterceptMethod), new [] { typeof(MethodSurrogateInfo) });
			il.Emit(OpCodes.Call, interceptMethodInfo);

			il.CopyArrayToArgs(OriginalMethod, args);

			il.Emit(OpCodes.Ldloc, info);
			var retValInfo = typeof(MethodSurrogateInfo).GetField(nameof(MethodSurrogateInfo.ReturnValue));
			il.Emit(OpCodes.Ldfld, retValInfo);
			il.Emit(OpCodes.Unbox_Any, OriginalMethod.ReturnType);


			il.Emit(OpCodes.Ret);

			Builder.DefineMethodOverride(methodBuilder, OriginalMethod);
			
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

		private static LocalBuilder CreateArrayFromArgs(this ILGenerator IL, MethodInfo Method)
		{
			var parameters = Method.GetParameters();

			// var argsArray = new object[Size]
			var argsArray = IL.DeclareLocal(typeof(object[]));
			IL.LoadConstantInt32(parameters.Count());
			IL.Emit(OpCodes.Newarr, typeof(object));
			IL.Emit(OpCodes.Stloc, argsArray);

			// argsArray[i] = Args[i]
			for (int i = 0; i < parameters.Count(); i++)
			{
				IL.Emit(OpCodes.Ldloc, argsArray);
				IL.LoadConstantInt32(i);
				
				IL.LoadArgument(i);
				if (parameters[i].IsByRefOrOut())
					IL.LoadFromAddress(parameters[i].ParameterType);
				
				IL.Box(parameters[i]);
				IL.Emit(OpCodes.Stelem_Ref);
			}

			return argsArray;
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
				IL.Unbox(parameters[i]);
				IL.StoreIntoAddress(parameters[i].ParameterType);
				// IL.Emit(OpCodes.Stind_I4);
			}
		}
	}
}