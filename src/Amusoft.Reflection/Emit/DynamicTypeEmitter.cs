﻿// Copyright 2018 Andreas Müller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;
using System.Reflection;
using System.Reflection.Emit;
using Amusoft.Reflection.Compatibility;

namespace Amusoft.Reflection.Emit
{
	internal static class DynamicTypeEmitter
	{
		private static string GenerateAccessorName(Type targetType, string propertyName)
		{
			return string.Format($"{nameof(DynamicPropertyAccessor)}_{0}_{1}", targetType.FullName.Replace('.', '_'), propertyName);
		}

		private static ModuleBuilder GetDynamicModule(AssemblyBuilder newAssembly)
		{
			lock (typeof(DynamicTypeEmitter))
			{
				return newAssembly.DefineDynamicModule("DynamicPropertyAccessorModule");
			}
		}

		private static AssemblyBuilder GetDynamicAssembly()
		{
			lock (typeof(DynamicTypeEmitter))
			{
				var assemblyName = new AssemblyName { Name = "DynamicPropertyAccessorAssembly" };
				return AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			}
		}

		public static Type EmitType(Type targetType, string propertyName)
		{
			var newAssembly = GetDynamicAssembly();
			var newModule = GetDynamicModule(newAssembly);

			var dynamicType = newModule.DefineType(GenerateAccessorName(targetType, propertyName), TypeAttributes.Public);

			dynamicType.AddInterfaceImplementation(typeof(IPropertyDelegate));
			dynamicType.DefineDefaultConstructor(MethodAttributes.Public);

			GenerateGetter(dynamicType, targetType, propertyName);
			GenerateSetter(dynamicType, targetType, propertyName);

			// Load the type
			return dynamicType.CreateTypeInfo().AsType();
		}

		private static void GenerateSetter(TypeBuilder dynamicType, Type targetType, string propertyName)
		{
			var setParamTypes = new[] {typeof(object), typeof(object)};

			var setMethod = dynamicType.DefineMethod(nameof(IPropertyDelegate.Set),
				MethodAttributes.Public | MethodAttributes.Virtual,
				typeof(void),
				setParamTypes);

			var setGenerator = setMethod.GetILGenerator();

			EmitSetter(setGenerator, targetType, propertyName);
		}

		private static void GenerateGetter(TypeBuilder dynamicType, Type targetType, string propertyName)
		{
			var getParamTypes = new[] {typeof(object)};
			var getReturnType = typeof(object);
			var getMethod = dynamicType.DefineMethod(nameof(IPropertyDelegate.Get),
				MethodAttributes.Public | MethodAttributes.Virtual,
				getReturnType,
				getParamTypes);

			var getGenerator = getMethod.GetILGenerator();

			EmitGetter(getGenerator, targetType, propertyName);
		}

		private static void EmitGetter(ILGenerator getGenerator, Type targetType, string propertyName)
		{
			var privateGetMethod = CompatTypeExtensions.GetMethod(targetType, "get_" + propertyName);

			if (privateGetMethod != null)
			{
				getGenerator.DeclareLocal(typeof(object));
				//Load the first argument 
				getGenerator.Emit(OpCodes.Ldarg_1);
				//Cast to the source type
				getGenerator.Emit(OpCodes.Castclass, targetType);
				//Get the property value
				getGenerator.EmitCall(OpCodes.Call, privateGetMethod, null);

				if (privateGetMethod.ReturnType.GetTypeInfo().IsValueType)
				{
					//Box
					getGenerator.Emit(OpCodes.Box, privateGetMethod.ReturnType);
				}
				//Store it
				// following IL seems unnecessary according to test results
				// original source contained the following 2 lines
				//				getGenerator.Emit(OpCodes.Stloc_0);
				//				getGenerator.Emit(OpCodes.Ldloc_0);
			}
			else
			{
				getGenerator.ThrowException(typeof(MissingMethodException));
			}

			getGenerator.Emit(OpCodes.Ret);
		}

		private static void EmitSetter(ILGenerator setGenerator, Type targetType, string propertyName)
		{
			var privateSetMethod = CompatTypeExtensions.GetMethod(targetType, "set_" + propertyName);
			if (privateSetMethod != null)
			{
				Type paramType = privateSetMethod.GetParameters()[0].ParameterType;

				setGenerator.DeclareLocal(paramType);
				//Load the first argument //(target object)
				setGenerator.Emit(OpCodes.Ldarg_1);
				//Cast to the source type
				setGenerator.Emit(OpCodes.Castclass, targetType);
				//Load the second argument 
				setGenerator.Emit(OpCodes.Ldarg_2);
				//(value object)
				if (paramType.GetTypeInfo().IsValueType)
				{
					//Unbox it 
					setGenerator.Emit(OpCodes.Unbox, paramType);
					//and load
					if (TypeOpCodeMapping.Map.TryGetValue(paramType, out var code))
					{
						var load = code;
						setGenerator.Emit(load);
					}
					else
					{
						setGenerator.Emit(OpCodes.Ldobj, paramType);
					}
				}
				else
				{
					setGenerator.Emit(OpCodes.Castclass, paramType); //Cast class
				}
				setGenerator.EmitCall(OpCodes.Callvirt, privateSetMethod, null); //Set the property value
			}
			else
			{
				setGenerator.ThrowException(typeof(MissingMethodException));
			}
			setGenerator.Emit(OpCodes.Ret);
		}
	}
}