using System;
using System.Reflection;
using Surrogate.Helpers;

namespace Surrogate.Interfaces
{
	public interface IMethodSurrogate
	{
		MethodSurrogatePreCommands PreEvaluate(MethodSurrogateInfo Info);
		MethodSurrogatePostCommands PostEvaluate(MethodSurrogateInfo Info);
	}

	public enum MethodSurrogatePreCommands
	{
		Abort, Continue, 
	}

	public enum MethodSurrogatePostCommands
	{
		ReEvaluate, Continue,
	}

	public enum MethodSurrogateResults
	{
		Aborted, ReEvaluated, Continued
	}

	public class MethodSurrogateInfo : IClonable<MethodSurrogateInfo>
	{
		public object Item;
		public MethodInfo Method;
		public object[] Arguments;
		public MethodSurrogateResults PreviousResult;
		public object ResultBy;

		public MethodSurrogateInfo Clone()
		=> (MethodSurrogateInfo) this.MemberwiseClone();
    }
}