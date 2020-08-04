using System;
using System.Reflection;
using Surrogate.Interfaces;

namespace Surrogate.Tests.IPropertySurrogateTest
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertySurrogateSet : Attribute, IPropertySurrogate
	{
		public object InterceptPropertyGet(object Argument)
		=> Argument;

		public object InterceptPropertySet(object Argument)
		=> 789;
	}

	[AttributeUsage(AttributeTargets.Property)]
	public class PropertySurrogateGet : Attribute, IPropertySurrogate
	{
		public object InterceptPropertyGet(object Argument)
		=> 123;

		public object InterceptPropertySet(object Argument)
		=> Argument;
	}
}