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
		internal static CustomAttributeBuilder[] ToCustomAttributeBuilder(this IEnumerable<CustomAttributeData> CustomAttributes)
		=> CustomAttributes.Select(attribute => attribute.ToCustomAttributeBuilder()).ToArray();

		internal static CustomAttributeBuilder ToCustomAttributeBuilder(this CustomAttributeData CustomAttribute)
		{
			var attributeArgs = CustomAttribute.ConstructorArguments.Select(a => a.Value).ToArray();

			var propertyArgs = CustomAttribute.NamedArguments.Where(i => i.MemberInfo is PropertyInfo);
			var propertyInfos = propertyArgs.Select(a => (PropertyInfo)a.MemberInfo).ToArray();
			var propertyValues = propertyArgs.Select(a => a.TypedValue.Value).ToArray();
			
			var fieldArgs = CustomAttribute.NamedArguments.Where(i => i.MemberInfo is FieldInfo);
			var fieldInfos = fieldArgs.Select(a => (FieldInfo)a.MemberInfo).ToArray();
			var fieldValues = fieldArgs.Select(a => a.TypedValue.Value).ToArray();
			
			return new CustomAttributeBuilder(CustomAttribute.Constructor, attributeArgs, propertyInfos, propertyValues, fieldInfos, fieldValues);
		}

		internal static Attribute[] FindAttributes(this MethodInfo Method, Type AttributeType)
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

			return retVal.ToArray();
		}

		internal static T[] FindAttributes<T>(this MethodInfo Method)
		=> Method.FindAttributes(typeof(T)).Cast<T>().ToArray();

		internal static Attribute[] FindAttributes(this ParameterInfo Parameter, Type AttributeType)
		{
			var retVal = new List<Attribute>();
			var attributes = Parameter.GetCustomAttributes(true);
			
			foreach (var attribute in attributes.Cast<Attribute>())
			{
				if (AttributeType.IsAssignableFrom(attribute.GetType()))
					retVal.Add(attribute);
			}

			return retVal.ToArray();
		}

		internal static T[] FindAttributes<T>(this ParameterInfo Parameter)
		=> Parameter.FindAttributes(typeof(T)).Cast<T>().ToArray();
	}
}