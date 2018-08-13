// Copyright 2018 Andreas Müller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;

namespace Amusoft.Reflection.Emit
{
	internal static class PropertyDelegateFactory
	{
		public static IPropertyDelegate CreateAccessor(Type targetType, string propertyName)
		{
			var type = DynamicTypeEmitter.EmitType(targetType, propertyName);
			var accessor = Activator.CreateInstance(type) as IPropertyDelegate;
			if (accessor == null)
			{
				throw new TypeEmitterException($"Unable to create {nameof(IPropertyDelegate)} for property {propertyName} of {targetType.FullName}.");
			}

			return accessor;
		}
	}
}