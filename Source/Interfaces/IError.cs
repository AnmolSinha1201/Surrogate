using System;
using System.Reflection;

namespace Surrogate.Interfaces
{
	public interface IErrorAction
	{
		Action<string> ErrorAction { get; set; }
	}
}