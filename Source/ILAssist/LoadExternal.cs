using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	internal static partial class ILHelpers
	{
		public static void LoadExternalType(this ILGenerator IL, Type ItemType)
		{
			IL.Emit(OpCodes.Ldtoken, ItemType);
			IL.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetTypeFromHandle), new[] { typeof(RuntimeTypeHandle) }));
		}

		public static void LoadExternalMethodInfo(this ILGenerator IL, MethodInfo Method)
		{
			IL.Emit(OpCodes.Ldtoken, Method);
			IL.Emit(OpCodes.Call, typeof(MethodBase).GetMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) }));
		}

		public static void LoadExternalAttribute(this ILGenerator IL, MethodInfo OriginalMethod, Type AttributeType)
		{
			// Attribute.GetCustomAttribute(OriginalMethod, AttributeInfo.GetType())
			IL.LoadExternalMethodInfo(OriginalMethod);
			IL.LoadExternalType(AttributeType);
			IL.Emit(OpCodes.Call, typeof(Attribute).GetMethod(nameof(Attribute.GetCustomAttribute), new[] { typeof(MemberInfo), typeof(Type) }));
		}

		public static LocalBuilder CreateExternalType(this ILGenerator IL, Type ItemType, Type[] ConstructorTypeArray)
		{
			var local = IL.DeclareLocal(ItemType);
			IL.Emit(OpCodes.Newobj, ItemType.GetConstructor(ConstructorTypeArray));
			IL.Emit(OpCodes.Stloc, local);

			return local;
		}
	}
}