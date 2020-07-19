using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public class ILConstructor : BaseILMethodBase
	{
		public ILConstructor(ConstructorBuilder Builder) : base(Builder)
		{ }

		public void CopyParameters(ParameterInfo[] OriginalParameters)
		=> ((ConstructorBuilder)Base).CopyParameters(OriginalParameters);

		public void SetCustomAttribute(CustomAttributeBuilder Attribute)
		=> ((ConstructorBuilder)Base).SetCustomAttribute(Attribute);
	}

	public static partial class Extensions
	{
		/// From : https://stackoverflow.com/questions/6879279/using-typebuilder-to-create-a-pass-through-constructor-for-the-base-class
		/// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
		/// forwards its arguments to the base constructor, and matches the base constructor's signature.
		/// Supports optional values, and custom attributes on constructors and parameters.
		/// Does not support n-ary (variadic) constructors</summary>
		public static void CreatePassThroughConstructors(this TypeBuilder builder, Type baseType)
		{
			foreach (var constructor in baseType.GetConstructors()) {
				builder.CreatePassThroughConstructor(constructor);
			}
		}

		internal static void CreatePassThroughConstructor(this TypeBuilder Builder, ConstructorInfo Constructor)
		{
			var parameters = Constructor.GetParameters();
			var ctor = Builder.DefineConstructor(parameters, Constructor.CallingConvention).ToILConstructor();
			ctor.CopyParameters(parameters);

			foreach (var attribute in Constructor.GetCustomAttributesData().ToCustomAttributeBuilder())
				ctor.SetCustomAttribute(attribute);
			
			ctor.EmitCallBase();
		}

		internal static ILConstructor ToILConstructor(this ConstructorBuilder Builder)
		=> new ILConstructor(Builder);

		internal static ConstructorBuilder DefineConstructor(this TypeBuilder Builder, ParameterInfo[] Parameters, CallingConventions Convention)
		{
			if (Parameters.Length > 0 && Parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
				throw new InvalidOperationException("Variadic constructors are not supported");

			var parameterTypes = Parameters.Select(p => p.ParameterType).ToArray();
			var requiredCustomModifiers = Parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
			var optionalCustomModifiers = Parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

			var ctor = Builder.DefineConstructor(MethodAttributes.Public, Convention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
			ctor.CopyParameters(Parameters);

			return ctor;
		}
	}
}