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
				var preInfo = baseInfo.With(i =>
				{
					i.PreviousResult = postCommand == MethodSurrogatePostCommands.ReEvaluate ? MethodSurrogateResults.ReEvaluated
						: MethodSurrogateResults.Continued;
					i.ResultBy = resultBy;
				});
				foreach (var attribute in methodAttributes)
				{
					preCommand = attribute.PreEvaluate(preInfo);
					resultBy = attribute;

					if (preCommand == MethodSurrogatePreCommands.Abort)
						break;
				}
				

				if (preCommand != MethodSurrogatePreCommands.Abort)
					retVal = Info.BackingMethod.Invoke(Info.TargetObject, Params);


				var postInfo = baseInfo.With(i => 
				{ 
					i.PreviousResult = preCommand == MethodSurrogatePreCommands.Abort ? MethodSurrogateResults.Aborted
						: MethodSurrogateResults.Continued;
					i.ResultBy = resultBy;
				});
				foreach (var attribute in methodAttributes)
				{
					postCommand = attribute.PostEvaluate(postInfo);
					resultBy = attribute;

					if (postCommand == MethodSurrogatePostCommands.ReEvaluate)
						break;
				}

				if (postCommand == MethodSurrogatePostCommands.Continue)
					break;
			}

			return retVal;
		}
	}
}