using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IParameterSurrogate
	{
		// void InterceptParameter(ParameterSurrogateInfo Info);
		void InterceptParameter(ref object Argument);
	}


	// ParameterInfo.Member : Provides MethodInfo of Declaring method.
	// ParameterInfo.Member.DeclaringType : Provides Type of Declaring class.
	public class ParameterSurrogateInfo
	{
		public object Value;
		public ParameterInfo ParamInfo;
		
		public ParameterSurrogateInfo(ParameterInfo ParamInfo, object ParamValue)
		{
			this.ParamInfo = ParamInfo;
			this.Value = ParamValue;
		}
	}
}