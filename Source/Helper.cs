using System;

namespace Surrogate.Helpers
{
	public static class Helpers
	{
		public static object Default(this object Item)
		{
			if (Item == null)
				return null;

			var itemType = Item.GetType();

			if (itemType.IsValueType)
				return Activator.CreateInstance(itemType);

			return null;
		}

		public static object Default(this Type ItemType)
		{
			if (ItemType.IsValueType)
				return Activator.CreateInstance(ItemType);

			return null;
		}
	}
}