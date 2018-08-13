// Copyright 2018 Andreas Müller
// This file is a part of Amusoft.Reflection and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;
using System.Diagnostics;
using Amusoft.Reflection.Compatibility;

namespace Amusoft.Reflection.Emit
{
	// inspired by http://stackoverflow.com/questions/1960692/practical-example-of-dynamic-method/1960737#1960737

	[DebuggerDisplay("DPA: {PropertyName}")]
	public class DynamicPropertyAccessor
	{
		internal DynamicPropertyAccessor(Type targetType, string propertyName)
		{
			_targetType = targetType;
			_propertyName = propertyName;
			var propertyInfo = CompatTypeExtensions.GetProperty(targetType, propertyName);
			if (propertyInfo == null)
			{
				throw new ReflectionOptimizerException(string.Format($"Property \"{propertyName}\" is not found in type {targetType}.", propertyName, targetType));
			}
			_canRead = propertyInfo.CanRead;
			_canWrite = propertyInfo.CanWrite;
			_propertyType = propertyInfo.PropertyType;
			PropertyDelegate = PropertyDelegateFactory.CreateAccessor(targetType, propertyName);
		}

		private readonly string _propertyName;
		public string PropertyName
		{
			get { return _propertyName; }
		}

		protected IPropertyDelegate PropertyDelegate;

		private readonly bool _canRead;
		public bool CanRead
		{
			get { return _canRead; }
		}

		private readonly bool _canWrite;
		public bool CanWrite
		{
			get { return _canWrite; }
		}

		private readonly Type _targetType;
		public Type TargetType
		{
			get { return _targetType; }
		}

		private readonly Type _propertyType;
		public Type PropertyType
		{
			get { return _propertyType; }
		}

		public virtual object Get(object target)
		{
			if (_canRead)
			{
				return PropertyDelegate.Get(target);
			}
			else
			{
				throw new ReflectionOptimizerException($"Property \"{_propertyName}\" does not have method {nameof(Get)}.", _propertyName);
			}
		}
	
		public virtual void Set(object target, object value)
		{
			if (_canWrite)
			{
				PropertyDelegate.Set(target, value);
			}
			else
			{
				throw new ReflectionOptimizerException($"Property \"{_propertyName}\" does not have method {nameof(Set)}.", _propertyName);
			}
		}
	}
}
