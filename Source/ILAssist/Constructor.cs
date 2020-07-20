using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate.ILAssist
{
	public class ILConstructor
	{
		public ConstructorInfo Base;

		public ILConstructor(ConstructorInfo Info)
		{
			this.Base = Info;
		}

		public ConstructorBuilder CreatePassThroughConstructor(TypeBuilder Builder)
		{
			var ctor = Builder.DefineConstructor(Base.GetParameters(), Base.CallingConvention);

			foreach (var attribute in Base.GetCustomAttributesData().ToCustomAttributeBuilder())
				ctor.SetCustomAttribute(attribute);
			
			ctor.GetILGenerator().EmitCallBaseAndReturn(ctor);

			return ctor;
		}
	}

	public static partial class Extensions
	{
		internal static ILConstructor ToILConstructor(this ConstructorInfo Info)
		=> new ILConstructor(Info);

		internal static ConstructorBuilder DefineConstructor(this TypeBuilder Builder, ParameterInfo[] Parameters, CallingConventions Convention)
		{
			if (Parameters.Length > 0 && Parameters.Last().IsDefined(typeof(ParamArrayAttribute), false))
				throw new InvalidOperationException("Variadic constructors are not supported");

			var parameterTypes = Parameters.Select(p => p.ParameterType).ToArray();
			var requiredCustomModifiers = Parameters.Select(p => p.GetRequiredCustomModifiers()).ToArray();
			var optionalCustomModifiers = Parameters.Select(p => p.GetOptionalCustomModifiers()).ToArray();

			var ctor = Builder.DefineConstructor(MethodAttributes.Public, Convention, parameterTypes, requiredCustomModifiers, optionalCustomModifiers);
			ctor.ApplyParameters(Parameters);

			return ctor;
		}
	}
}