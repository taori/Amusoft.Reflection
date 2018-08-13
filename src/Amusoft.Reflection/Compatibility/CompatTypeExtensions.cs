// Copyright 2018 Andreas Müller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Amusoft.Reflection.Compatibility
{
	internal static class CompatTypeExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MethodInfo GetMethod(Type source, string name)
		{
#if NETSTANDARD1_1
			return source.GetRuntimeMethods().FirstOrDefault(d => string.Equals(name, d.Name, StringComparison.CurrentCultureIgnoreCase));
#elif NETSTANDARD2_0
			return source.GetMethod(name);
#else
			return source.GetMethod(name);
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PropertyInfo GetProperty(Type source, string name)
		{
#if NETSTANDARD1_1
			return source.GetRuntimeProperty(name);
#elif NETSTANDARD2_0
			return source.GetProperty(name);
#else
			return source.GetProperty(name);
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAssignableFrom(Type derivate, Type baseType)
		{
#if NETSTANDARD1_1
			return derivate.GetTypeInfo().IsAssignableFrom(baseType.GetTypeInfo());
#elif NETSTANDARD2_0
			return derivate.IsAssignableFrom(baseType);
#else
			return derivate.IsAssignableFrom(baseType);
#endif
		}
	}
}