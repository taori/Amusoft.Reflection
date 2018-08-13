// Copyright 2018 Andreas Müller
// This file is a part of Amusoft.Reflection and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;
using System.Diagnostics;
using Amusoft.Reflection.Compatibility;

namespace Amusoft.Reflection.Emit
{
	// inspired by http://stackoverflow.com/questions/1960692/practical-example-of-dynamic-method/1960737#1960737

	[DebuggerDisplay("{ToDebug()}")]
	internal class DynamicPropertyAccessor : IDynamicPropertyAccessor
	{
		private string ToDebug()
		{
			return $"{TargetType}.{PropertyName} {(CanRead ? ", readable" : string.Empty)} {(CanWrite ? ", writeable" : string.Empty)}";
		}

		internal DynamicPropertyAccessor(Type targetType, string propertyName)
		{
			TargetType = targetType;
			PropertyName = propertyName;
			var propertyInfo = CompatTypeExtensions.GetProperty(targetType, propertyName);
			if (propertyInfo == null)
			{
				throw new TypeEmitterException(string.Format($"Property \"{propertyName}\" is not found in type \"{targetType}\".", propertyName, targetType));
			}
			CanRead = propertyInfo.CanRead;
			CanWrite = propertyInfo.CanWrite;
			PropertyType = propertyInfo.PropertyType;
			PropertyDelegate = PropertyDelegateFactory.CreateAccessor(targetType, propertyName);
		}

		public string PropertyName { get; }

		protected IPropertyDelegate PropertyDelegate;

		public bool CanRead { get; }

		public bool CanWrite { get; }

		public Type TargetType { get; }

		public Type PropertyType { get; }

		public virtual object Get(object target)
		{
			if (CanRead)
			{
				return PropertyDelegate.Get(target);
			}
			else
			{
				throw new TypeEmitterException($"Property \"{PropertyName}\" does not have method {nameof(Get)}.", PropertyName);
			}
		}
	
		public virtual void Set(object target, object value)
		{
			if (CanWrite)
			{
				PropertyDelegate.Set(target, value);
			}
			else
			{
				throw new TypeEmitterException($"Property \"{PropertyName}\" does not have method {nameof(Set)}.", PropertyName);
			}
		}
	}
}
