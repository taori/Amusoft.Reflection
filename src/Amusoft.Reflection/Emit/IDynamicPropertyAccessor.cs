// Copyright 2018 Andreas Müller
// This file is a part of Amusoft.Reflection and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;

namespace Amusoft.Reflection.Emit
{
	public interface IDynamicPropertyAccessor
	{
		string PropertyName { get; }
		bool CanRead { get; }
		bool CanWrite { get; }
		Type TargetType { get; }
		Type PropertyType { get; }
		object Get(object target);
		void Set(object target, object value);
	}
}