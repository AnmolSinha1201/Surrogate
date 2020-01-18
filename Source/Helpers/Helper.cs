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

	}
}