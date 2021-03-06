// Copyright 2018 Andreas M�ller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;

namespace Amusoft.Reflection.Emit
{
	public class TypeEmitterException : Exception
	{
		public TypeEmitterException(string message, string propertyName)
			: base(message)
		{
			PropertyName = propertyName;
		}

		public TypeEmitterException(string message)
			: base(message)
		{
		}

		public string PropertyName { get; private set; }
	}
}