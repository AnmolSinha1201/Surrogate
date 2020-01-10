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
		private static bool EligibleParameterProxy(this ParameterInfo PInfo)
		{
			var attributes = PInfo.GetCustomAttributes();
			foreach (var attribute in attributes)
			{
				if (typeof(IParameterSurrogate).IsAssignableFrom(attribute.GetType()))
					return true;
			}

			return false;
		}

		private static LocalBuilder CreateParameterProxy(this MethodBuilder Builder, MethodInfo OriginalMethod)
		{
			var il = Builder.GetILGenerator();
			var parameters = OriginalMethod.GetParameters();

			// var argsArray = new object[Size]
			var argsArray = il.DeclareLocal(typeof(object[]));
			il.LoadConstantInt32(parameters.Count());
			il.Emit(OpCodes.Newarr, typeof(object));
			il.Emit(OpCodes.Stloc, argsArray);

			var ILparameters = il.DeclareLocal(typeof(ParameterInfo[]));
			foreach (var parameter in parameters)
			{
				if (parameter.EligibleParameterProxy())
				{
					il.LoadExternalMethodInfo(OriginalMethod);
					il.Emit(OpCodes.Callvirt, typeof(MethodInfo).GetMethod(nameof(MethodInfo.GetParameters)));
					il.Emit(OpCodes.Stloc, ILparameters);
					break;
				}
			}

			// argsArray[i] = Args[i]
			for (int i = 0; i < parameters.Count(); i++)
			{
				il.Emit(OpCodes.Ldloc, argsArray);
				il.LoadConstantInt32(i);
				
				if (parameters[i].EligibleParameterProxy())
				{
					il.Emit(OpCodes.Ldloc, ILparameters);
					il.LoadConstantInt32(i);
					il.Emit(OpCodes.Ldelem_Ref);

					il.Emit(OpCodes.Call, typeof(Attribute).GetMethod(nameof(Attribute.GetCustomAttributes), new [] { typeof(ParameterInfo) }));
					var localAttributes = il.DeclareLocal(typeof(IParameterSurrogate));
					il.Emit(OpCodes.Stloc, localAttributes);

					var attributes = Attribute.GetCustomAttributes(parameters[i]);
					for (int o = 0; o < attributes.Count(); o++)
					{
						if (!typeof(IParameterSurrogate).IsAssignableFrom(attributes[o].GetType()))
							continue;
						
						il.Emit(OpCodes.Ldloc, localAttributes);
						il.LoadConstantInt32(o);
						il.Emit(OpCodes.Ldelem_Ref);

						il.Emit(OpCodes.Ldloc, ILparameters);
						il.LoadConstantInt32(i);
						il.Emit(OpCodes.Ldelem_Ref);

						il.LoadArgument(i);
						if (parameters[i].IsByRefOrOut())
							il.LoadFromAddress(parameters[i].ParameterType);
						il.Emit(OpCodes.Box, parameters[i].ActualParameterType());
						
						var info = il.CreateExternalType(typeof(ParameterSurrogateInfo), new[] { typeof(ParameterInfo), typeof(object) });
						il.Emit(OpCodes.Ldloc, info);

						il.Emit(OpCodes.Call, attributes[o].GetType().GetMethod(nameof(IParameterSurrogate.InterceptParameter), new[] { typeof(ParameterSurrogateInfo) }));
						
						il.Emit(OpCodes.Ldloc, info);
						il.Emit(OpCodes.Ldfld, typeof(ParameterSurrogateInfo).GetField(nameof(ParameterSurrogateInfo.ParamValue)));
					}
				}
				else
				{
					il.LoadArgument(i);
					if (parameters[i].IsByRefOrOut())
						il.LoadFromAddress(parameters[i].ParameterType);
				}
				
				il.Emit(OpCodes.Box, parameters[i].ActualParameterType());
				il.Emit(OpCodes.Stelem_Ref);
			}

			return argsArray;
		}
	}
}