using System;

namespace Amusoft.Reflection.Emit
{
	public class ReflectionOptimizerException : Exception
	{
		public ReflectionOptimizerException(string message, string propertyName)
			: base(message)
		{
			PropertyName = propertyName;
		}

		public ReflectionOptimizerException(string message)
			: base(message)
		{
		}

		public string PropertyName { get; private set; }
	}
}