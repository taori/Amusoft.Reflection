// Copyright 2018 Andreas Müller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

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