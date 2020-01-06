using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Surrogate
{
	public static partial class SurrogateBuilder
	{
		private static TypeBuilder CreateTypeBuilder(this Type ItemType)
		{
			var assemblyName = new AssemblyName(Guid.NewGuid().ToString());
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
			
			TypeBuilder tb = moduleBuilder.DefineType(ItemType.Name, TypeAttributes.Public, ItemType);
			
			return tb;
		}
	}
}