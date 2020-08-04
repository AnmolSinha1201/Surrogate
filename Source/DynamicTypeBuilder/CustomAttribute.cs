using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		public static IEnumerable<CustomAttributeBuilder> ToCustomAttributeBuilder(this IEnumerable<CustomAttributeData> CustomAttributes)
		=> CustomAttributes.Select(attribute => attribute.ToCustomAttributeBuilder());

		public static CustomAttributeBuilder ToCustomAttributeBuilder(this CustomAttributeData CustomAttribute)
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

		/// <summary>
		/// Does NOT allow implicit type casting
		/// </summary>
		public static CustomAttributeBuilder CreateCustomAttributeBuilder<T>(params object[] Arguments) where T :Attribute
		{
			var constructor = typeof(T).GetConstructor(Arguments.Select(i => i.GetType()).ToArray());
			var builder = new CustomAttributeBuilder(constructor, Arguments);

			return builder;
		}

		/// <summary>
		/// Allows implicit type casting as Argument is an Expression
		/// </summary>
		public static CustomAttributeBuilder CreateCustomAttributeBuilder(Expression<Func<Attribute>> attributeExpression)
		{
			if (attributeExpression.Body is NewExpression == false)
				throw new Exception("Only 'new' expressions are supported");

			var expression = (NewExpression)attributeExpression.Body;
			var arguments = expression.Arguments
				.Cast<Expression>()
				.Select(i => Expression.Lambda(i).Compile().DynamicInvoke())
				.ToArray();
			var constructor = expression.Constructor;

			return new CustomAttributeBuilder(constructor, arguments);
		}
	}
}