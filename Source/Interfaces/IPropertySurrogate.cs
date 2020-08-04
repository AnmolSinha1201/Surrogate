using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IPropertySurrogate
	{
		object InterceptPropertyGet(object Argument);
		object InterceptPropertySet(object Argument);
	}
}