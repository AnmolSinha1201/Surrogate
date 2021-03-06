using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Interfaces;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		internal static List<Attribute> FindAttributes(this MethodInfo Method, Type AttributeType)
		{
			var retVal = new List<Attribute>();
			var attributes = 
				AttributeType == typeof(IMethodSurrogate) ?  Method.GetCustomAttributes(true) :
				AttributeType == typeof(IReturnSurrogate) ? Method.ReturnParameter.GetCustomAttributes(true):
				new object[] { };
			
			foreach (var attribute in attributes.Cast<Attribute>())
			{
				if (AttributeType.IsAssignableFrom(attribute.GetType()))
					retVal.Add(attribute);
			}

			return retVal;
		}

		internal static List<T> FindAttributes<T>(this MethodInfo Method)
		=> Method.FindAttributes(typeof(T)).Cast<T>().ToList();

		internal static List<Attribute> FindAttributes(this ParameterInfo Parameter, Type AttributeType)
		{
			var retVal = new List<Attribute>();
			var attributes = Parameter.GetCustomAttributes(true);
			
			foreach (var attribute in attributes.Cast<Attribute>())
			{
				if (AttributeType.IsAssignableFrom(attribute.GetType()))
					retVal.Add(attribute);
			}

			return retVal;
		}

		internal static List<T> FindAttributes<T>(this ParameterInfo Parameter)
		=> Parameter.FindAttributes(typeof(T)).Cast<T>().ToList();


		internal static List<Attribute> FindAttributes(this PropertyInfo Property, Type AttributeType)
		{
			var retVal = new List<Attribute>();
			var attributes = Property.GetCustomAttributes();
			
			foreach (var attribute in attributes.Cast<Attribute>())
			{
				if (AttributeType.IsAssignableFrom(attribute.GetType()))
					retVal.Add(attribute);
			}

			return retVal;
		}

		internal static List<T> FindAttributes<T>(this PropertyInfo Property)
		=> Property.FindAttributes(typeof(T)).Cast<T>().ToList();
	}
}