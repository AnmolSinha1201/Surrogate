using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Surrogate.Helpers;
using Surrogate.Interfaces;

namespace Surrogate.ILAssist
{
	public static partial class Extensions
	{
		public static object MethodSurrogateHook(HookInfo Info, object[] Params)
		{
			var methodAttributes = Info.OriginalMethod.FindAttributes<IMethodSurrogate>().Order();

			var baseInfo = new MethodSurrogateInfo
			{
				Item = Info.TargetObject, 
				Method = Info.BackingMethod, 
				Arguments = Params, 
				PreviousResult = MethodSurrogateResults.Continued, 
				ResultBy = null
			};
			
			var preCommand = MethodSurrogatePreCommands.Continue;
			var postCommand = MethodSurrogatePostCommands.Continue;
			object resultBy = null;

			object retVal = Info.OriginalMethod.ReturnType.Default();
			while (true)
			{
				(preCommand, resultBy) = methodAttributes.PreCommandHook(baseInfo.PatchResult(postCommand.ToResult(), resultBy));
				
				if (preCommand != MethodSurrogatePreCommands.Abort)
					retVal = Info.BackingMethod.Invoke(Info.TargetObject, Params);

				(postCommand, resultBy) = methodAttributes.PostCommandHook(baseInfo.PatchResult(preCommand.ToResult(), resultBy));

				if (postCommand == MethodSurrogatePostCommands.Continue)
					break;
			}

			return retVal;
		}

		static (MethodSurrogatePreCommands, object) PreCommandHook(this List<IMethodSurrogate> AttributeList, MethodSurrogateInfo Info)
		{
			var command = MethodSurrogatePreCommands.Continue;
			object commandBy = null;
			
			foreach (var attribute in AttributeList)
			{
				command = attribute.PreEvaluate(Info.Clone());
				commandBy = attribute;

				if (command == MethodSurrogatePreCommands.Abort)
					break;
			}

			return (command, commandBy);
		}

		static (MethodSurrogatePostCommands, object) PostCommandHook(this List<IMethodSurrogate> AttributeList, MethodSurrogateInfo Info)
		{
			var command = MethodSurrogatePostCommands.Continue;
			object commandBy = null;
			
			foreach (var attribute in AttributeList)
			{
				command = attribute.PostEvaluate(Info.Clone());
				commandBy = attribute;

				if (command == MethodSurrogatePostCommands.ReEvaluate)
					break;
			}

			return (command, commandBy);
		}

		
	}

	public static partial class Helpers
	{
		public static MethodSurrogateInfo PatchResult(this MethodSurrogateInfo BaseInfo, MethodSurrogateResults PreviousResult, object ResultBy)
		=> BaseInfo.With(i => 
		{
			i.PreviousResult = PreviousResult;
			i.ResultBy = ResultBy;
		});

		public static MethodSurrogateResults ToResult(this MethodSurrogatePreCommands Command)
		=> Command == MethodSurrogatePreCommands.Abort ? MethodSurrogateResults.Aborted
			: MethodSurrogateResults.Continued;

		public static MethodSurrogateResults ToResult(this MethodSurrogatePostCommands Command)
		=> Command == MethodSurrogatePostCommands.ReEvaluate ? MethodSurrogateResults.ReEvaluated
			: MethodSurrogateResults.Continued;
	}
}