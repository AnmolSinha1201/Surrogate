using System;
using Surrogate.Interfaces;

namespace Surrogate.Internal.BaseSurrogates
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertySurrogate : Attribute, IPropertySurrogate
	{
		public object InterceptPropertyGet(object Argument)
		=> Argument;

		public object InterceptPropertySet(object Argument)
		=> Argument;
	}
}