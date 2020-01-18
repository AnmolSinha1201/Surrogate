using System.Reflection;
using Surrogate.Helpers;

namespace Surrogate.Internal.Helpers
{
	public static class Helpers
	{
		public static string ReturnError(this MethodInfo Info, string SurrogateName, string Clause = null)
		=> $"[{SurrogateName}] : Return value of {Info.FullMemberName()} MUST be {Clause ?? "non-null"}";

		public static string ParameterError(this ParameterInfo Info, string SurrogateName, string Clause = null)
		=> $"[{SurrogateName}] : {Info.Name} of {Info.FullMemberName()} MUST be {Clause ?? "non-null"}";

		public static string FullMemberName(this ParameterInfo Info)
		=> $"{Info.Member.DeclaringType.Name}.{Info.Member.Name}()";

		public static string FullMemberName(this MemberInfo Info)
		=> $"{Info.DeclaringType.Name}.{Info.Name}()";
	}
}