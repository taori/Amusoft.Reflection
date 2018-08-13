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
				throw new ReflectionOptimizerException($"Unable to create {nameof(IPropertyDelegate)} for property {propertyName} of {targetType.FullName}.");
			}

			return accessor;
		}
	}
}