// Copyright 2018 Andreas Müller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

namespace Amusoft.Reflection.Emit
{
	public interface IPropertyDelegate
	{
		object Get(object target);
		void Set(object target, object value);
	}
}