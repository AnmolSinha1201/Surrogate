using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		internal static MethodBuilder CreateBackingMethod(this TypeBuilder Builder, MethodInfo OriginalMethod)
		{
			var parameters = OriginalMethod.GetParameters();

			MethodBuilder methodBuilder = Builder.DefineMethod(
				$"<{OriginalMethod.Name}>k__BackingMethod",
				OriginalMethod.Attributes,
				CallingConventions.HasThis,
				OriginalMethod.ReturnType,
				parameters.Select(i => i.ParameterType).ToArray()
			);

			// To copy attributes to backing method
			// methodBuilder.ApplyParameters(parameters);
			// foreach (var attribute in OriginalMethod.GetCustomAttributesData().ToCustomAttributeBuilder())
			// 	methodBuilder.SetCustomAttribute(attribute);
			// methodBuilder.ApplyParameterAt(OriginalMethod.ReturnParameter, 0);
			

			// base.OriginalMethod(args);
			ILGenerator il = methodBuilder.GetILGenerator();
			for (int i = 0; i <= parameters.Count(); i++)
				il.LoadArgument(i);
			il.Emit(OpCodes.Call, OriginalMethod);
			il.Emit(OpCodes.Ret);

			return methodBuilder;
		}

		internal static void EmitCallBaseAndReturn(this ILGenerator Generator, MethodBase Base) // Also loads this
		{
			for (int i = 0; i <= Base.GetParameters().Length; i++)
				Generator.LoadArgument(i);
			
			Generator.Emit(OpCodes.Call, (dynamic)Base);
			Generator.Emit(OpCodes.Ret);
		}

		internal static List<MethodInfo> GetResolvedMethods(this Type ItemType)
		{
			var methodMethods = ItemType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);            
			var objectMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var filteredMethods = methodMethods
				.Except(objectMethods, new MethodEqualityComparer())
				.Where(i => !i.IsSpecialName)
				.ToList();

			return filteredMethods;
		}

		internal class MethodEqualityComparer : IEqualityComparer<MethodInfo>
		{
			public bool Equals(MethodInfo X, MethodInfo Y)
			{
				if (X == null || Y == null)
					return X == Y;

				return X.Name == Y.Name && X.DeclaringType == Y.DeclaringType;
			}

			public int GetHashCode(MethodInfo Item)
			=> Item.Name.GetHashCode();
		}
	}
}