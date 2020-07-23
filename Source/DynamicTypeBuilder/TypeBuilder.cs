using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public static partial class DynamicTypeBuilder
	{
		internal static TypeBuilder ToTypeBuilder(this Type BaseType)
		=> DynamicTypeBuilder.ToTypeBuilder(BaseType, BaseType.Name);

		public static TypeBuilder ToTypeBuilder(this Type BaseType, string TypeName)
		{
			AssemblyName assemblyName = new AssemblyName(Guid.NewGuid().ToString());
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
			var typeBuilder = moduleBuilder.DefineType(TypeName, TypeAttributes.Public, BaseType);

			if (BaseType != null)
			{
				foreach (var attribute in BaseType.GetCustomAttributesData())
					typeBuilder.SetCustomAttribute(attribute.ToCustomAttributeBuilder());
			}

			typeBuilder.CreatePassThroughConstructors2(BaseType);
			return typeBuilder;
		}

		public static void AddAttributes(this TypeBuilder Builder, List<CustomAttributeData> CustomAttributes)
		{
			foreach (var attribute in CustomAttributes)
				Builder.SetCustomAttribute(attribute.ToCustomAttributeBuilder());
		}

		/// From : https://stackoverflow.com/questions/6879279/using-typebuilder-to-create-a-pass-through-constructor-for-the-base-class
		/// <summary>Creates one constructor for each public constructor in the base class. Each constructor simply
		/// forwards its arguments to the base constructor, and matches the base constructor's signature.
		/// Supports optional values, and custom attributes on constructors and parameters.
		/// Does not support n-ary (variadic) constructors</summary>
		public static void CreatePassThroughConstructors2(this TypeBuilder Builder, Type BaseType)
		{
			foreach (var constructor in BaseType.GetConstructors()) 
            {
				Builder.CreatePassThroughConstructor(constructor);
			}
		}

		// public static void CreatePassThroughConstructor(this TypeBuilder Builder, ConstructorInfo Constructor)
		// {
		// 	var parameters = Constructor.GetParameters();
		// 	if (parameters.Length > 0 && parameters.Last().IsDefined(typeof(ParamArrayAttribute), false)) 
		// 	{
		// 		throw new InvalidOperationException("Variadic constructors are not supported");
		// 	}

		// 	var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();
		// 	var requiredCustomModifiers = parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
		// 	var optionalCustomModifiers = parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();
		// 	var ctor = Builder.DefineConstructor(MethodAttributes.Public, Constructor.CallingConvention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
			
		// 	for (var i = 0; i < parameters.Length; ++i)
		// 	{
		// 		var parameter = parameters[i];
		// 		var parameterBuilder = ctor.DefineParameter(i + 1, parameter.Attributes, parameter.Name);
		// 		if (((int)parameter.Attributes & (int)ParameterAttributes.HasDefault) != 0) 
		// 			parameterBuilder.SetConstant(parameter.RawDefaultValue);

		// 		foreach (var attribute in parameter.GetCustomAttributesData().Select(i => i.ToCustomAttributeBuilder()))
		// 			parameterBuilder.SetCustomAttribute(attribute);
		// 	}

		// 	foreach (var attribute in Constructor.GetCustomAttributesData().Select(i => i.ToCustomAttributeBuilder())) 
		// 		ctor.SetCustomAttribute(attribute);

		// 	var emitter = ctor.GetILGenerator();
		// 	emitter.Emit(OpCodes.Nop);

		// 	// Load `this` and call base constructor with arguments
		// 	emitter.Emit(OpCodes.Ldarg_0);
		// 	for (var i = 1; i <= parameters.Length; ++i) {
		// 		emitter.Emit(OpCodes.Ldarg, i);
		// 	}
		// 	emitter.Emit(OpCodes.Call, Constructor);

		// 	emitter.Emit(OpCodes.Ret);
		// }
	}
}