using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public class ILConstructor
	{
		public ILGenerator Generator;
		public ConstructorBuilder Base;

		public ILConstructor(ConstructorBuilder Builder)
		{
			Base = Builder;
			Generator = Builder.GetILGenerator();
		}
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
			if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
				throw new InvalidOperationException("Variadic constructors are not supported");

			var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
			var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
			var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

			var ctor = Builder.DefineConstructor(MethodAttributes.Public, Constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
			ctor.CopyParameters(parameters);

			foreach (var attribute in ToCustomAttributeBuilder(Constructor.GetCustomAttributesData())) {
				ctor.SetCustomAttribute(attribute);
			}

			var emitter = ctor.GetILGenerator();
			emitter.Emit(OpCodes.Nop);

			// Load `this` and call base constructor with arguments
			emitter.Emit(OpCodes.Ldarg_0);
			for (var i = 1; i <= parameters.Length; ++i) {
				emitter.Emit(OpCodes.Ldarg, i);
			}
			emitter.Emit(OpCodes.Call, Constructor);

			emitter.Emit(OpCodes.Ret);
		}
	}
}