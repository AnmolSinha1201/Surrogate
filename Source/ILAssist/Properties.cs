using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Internal.ILConstructs;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		internal static void OverrideProperty(this TypeBuilder Builder, PropertyInfo Property)
		{
			Builder.OverridePropertyGetter(Property);
			Builder.OverridePropertySetter(Property);
		}

		internal static void OverridePropertyGetter(this TypeBuilder Builder, PropertyInfo Property)
		{
			var oldMethod = Property.GetGetMethod();
			if (oldMethod == null)
				return;

			var methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
			var newMethod = Builder.DefineMethod(oldMethod.Name, methodAttributes, oldMethod.ReturnType, Type.EmptyTypes);
			var il = newMethod.GetILGenerator();
			var backingMethod = Builder.CreateBackingMethod(oldMethod);
			
			il.LoadArgument(0);

			// PropertyInfo
			il.LoadExternalType(Property.DeclaringType);
			il.Emit(OpCodes.Ldstr, Property.Name);
			il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetProperty), new [] { typeof(string) }));
			
			il.LoadExternalMethodInfo(backingMethod);
			il.Emit(OpCodes.Call, typeof(Extensions).GetMethod(nameof(Extensions.SurrogateHookPropertyGet)));
			
			il.Emit(OpCodes.Unbox_Any, Property.PropertyType);
			il.Emit(OpCodes.Ret);

			// Builder.DefineMethodOverride(newMethod, oldMethod);
		}

		internal static void OverridePropertySetter(this TypeBuilder Builder, PropertyInfo Property)
		{
			var oldMethod = Property.GetSetMethod();
			if (oldMethod == null)
				return;

			var methodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
			var newMethod = Builder.DefineMethod(oldMethod.Name, methodAttributes, null, new [] { Property.PropertyType });
			var il = newMethod.GetILGenerator();
			var backingMethod = Builder.CreateBackingMethod(oldMethod);
			
			il.LoadArgument(0);

			// PropertyInfo
			il.LoadExternalType(Property.DeclaringType);
			il.Emit(OpCodes.Ldstr, Property.Name);
			il.Emit(OpCodes.Call, typeof(Type).GetMethod(nameof(Type.GetProperty), new [] { typeof(string) }));
			
			il.LoadExternalMethodInfo(backingMethod);
			
			//Value
			il.LoadArgument(1);
			il.Emit(OpCodes.Box, Property.PropertyType);

			il.Emit(OpCodes.Call, typeof(Extensions).GetMethod(nameof(Extensions.SurrogateHookPropertySet)));
			il.Emit(OpCodes.Ret);

			// Builder.DefineMethodOverride(newMethod, oldMethod);
		}

		internal static List<PropertyInfo> GetResolvedProperties(this Type ItemType)
		{
			var typeProperties = ItemType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);            
			var objectProperties = typeof(object).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var filteredProperties = typeProperties
				.Except(objectProperties, new PropertyEqualityComparer())
				.Where(i => !i.IsSpecialName)
				.ToList();

			return filteredProperties;
		}

		internal class PropertyEqualityComparer : IEqualityComparer<PropertyInfo>
		{
			public bool Equals(PropertyInfo X, PropertyInfo Y)
			{
				if (X == null || Y == null)
					return X == Y;

				return X.Name == Y.Name && X.DeclaringType == Y.DeclaringType;
			}

			public int GetHashCode(PropertyInfo Item)
			=> Item.Name.GetHashCode();
		}
	}
}