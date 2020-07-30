using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		/// From : https://stackoverflow.com/questions/6879279/using-typebuilder-to-create-a-pass-through-constructor-for-the-base-class
		/// <summary>
		/// Creates one constructor for each public constructor in the base class. Each constructor simply
		/// forwards its arguments to the base constructor, and matches the base constructor's signature.
		/// Supports optional values, and custom attributes on constructors and parameters.
		/// </summary>

		internal static ConstructorBuilder CreatePassThroughConstructor(this TypeBuilder Builder, ConstructorInfo Constructor)
		{
			var ctor = Builder.DefineConstructor(Constructor.GetParameters(), Constructor.CallingConvention);

			foreach (var attribute in Constructor.GetCustomAttributesData().ToCustomAttributeBuilder())
				ctor.SetCustomAttribute(attribute);
			
			ctor.GetILGenerator().EmitCallBaseAndReturn(Constructor);

			return ctor;
		}

		internal static ConstructorBuilder DefineConstructor(this TypeBuilder Builder, ParameterInfo[] Parameters, CallingConventions Convention)
		{
			var parameterTypes = Parameters.Select(p => p.ParameterType).ToArray();
			var requiredCustomModifiers = Parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
			var optionalCustomModifiers = Parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

			var ctor = Builder.DefineConstructor(MethodAttributes.Public, Convention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
			ctor.ApplyParameters(Parameters);

			return ctor;
		}
	}
}