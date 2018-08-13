using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Amusoft.Reflection.Compatibility;

namespace Amusoft.Reflection.Emit
{
	public class DynamicPropertyAccessorFactory
	{
		public static DynamicPropertyAccessor Create<TType>(Expression<Func<TType, object>> propertyExpression)
		{
			if (propertyExpression == null)
				throw new ArgumentException($"{nameof(propertyExpression)}", nameof(propertyExpression));
			if (propertyExpression.Body == null)
				throw new ArgumentException($"{nameof(propertyExpression)}.{nameof(propertyExpression.Body)}", nameof(propertyExpression));

			var bodyType = propertyExpression.Body.GetType();
			MemberExpression memberExpression;

			if (typeof(UnaryExpression) == bodyType)
			{
				var unary = propertyExpression.Body as UnaryExpression;
				memberExpression = unary.Operand as MemberExpression;
				if (memberExpression == null)
					throw new NotSupportedException(string.Format($"{nameof(UnaryExpression)} only support {nameof(MemberExpression)} at this point. {0}", unary.Operand.GetType()));

				return new DynamicPropertyAccessor(typeof(TType), memberExpression.Member.Name);
			}
			else if (CompatTypeExtensions.IsAssignableFrom(typeof(MemberExpression), bodyType))
			{
				memberExpression = propertyExpression.Body as MemberExpression;

				if (memberExpression == null)
					throw new ArgumentException(nameof(memberExpression), nameof(memberExpression));

				return Create(typeof(TType), memberExpression.Member.Name);
			}
			else
			{
				throw new NotSupportedException($"{bodyType} not supported.");
			}
		}
		
		private static readonly Dictionary<KeyValuePair<Type, string>, DynamicPropertyAccessor> Cache = new Dictionary<KeyValuePair<Type, string>, DynamicPropertyAccessor>();

		/**
		 * OptimizationTests : Dictionary lookup takes longer than creating a new object and attempting to regenerate the type.
		 */
		public static DynamicPropertyAccessor Create(Type targetType, string propertyName)
		{
			var key = new KeyValuePair<Type, string>(targetType, propertyName);
			if (Cache.TryGetValue(key, out var accessor))
				return accessor;
			
			accessor = new DynamicPropertyAccessor(targetType, propertyName);
			Cache.Add(key, accessor);
			
			return accessor;
		}
	}
}