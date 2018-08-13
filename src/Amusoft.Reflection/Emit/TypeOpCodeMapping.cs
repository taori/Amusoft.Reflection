using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Amusoft.Reflection
{
	public static class TypeOpCodeMapping
	{
		public static readonly Dictionary<Type, OpCode> Map = new Dictionary<Type, OpCode>
		{
			{typeof (sbyte), OpCodes.Ldind_I1},
			{typeof (byte), OpCodes.Ldind_U1},
			{typeof (char), OpCodes.Ldind_U2},
			{typeof (short), OpCodes.Ldind_I2},
			{typeof (ushort), OpCodes.Ldind_U2},
			{typeof (int), OpCodes.Ldind_I4},
			{typeof (uint), OpCodes.Ldind_U4},
			{typeof (long), OpCodes.Ldind_I8},
			{typeof (ulong), OpCodes.Ldind_I8},
			{typeof (bool), OpCodes.Ldind_I1},
			{typeof (double), OpCodes.Ldind_R8},
			{typeof (float), OpCodes.Ldind_R4}
		};
	}
}