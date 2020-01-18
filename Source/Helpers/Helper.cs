using System;
using System.Reflection;

namespace Surrogate.Helpers
{
	public static class Helpers
	{
		public static object Default(this object Item)
		{
			if (Item == null)
				return null;

			return Item.GetType().Default();
		}

		public static object Default(this Type ItemType)
		{
			if (ItemType.IsValueType)
				return Activator.CreateInstance(ItemType);

			return null;
		}

		public static bool IsNumber(this object Value)
		{
			return Value.GetType().IsNumber();
		}

		public static bool IsNumber(this Type ItemType)
		{
			var typeCode = Type.GetTypeCode(ItemType);
			return typeCode == TypeCode.Decimal || 
				(ItemType.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char); 
		}
	}
}