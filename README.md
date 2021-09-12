# Amusoft.Reflection

## Project state

[![.GitHub](https://github.com/taori/Amusoft.Reflection/actions/workflows/dotnet.yml/badge.svg)](https://github.com/taori/Amusoft.Reflection/actions/workflows/dotnet.yml)
[![GitHub issues](https://img.shields.io/github/issues/taori/Amusoft.Reflection)](https://github.com/taori/Amusoft.Reflection/issues)
[![NuGet version (Amusoft.Reflection)](https://img.shields.io/nuget/v/Amusoft.Reflection.svg)](https://www.nuget.org/packages/Amusoft.Reflection/)

## Description

Extensions based around the reflection topic. The main reason this package exists is the need for property getting/setting with a performance faster than reflection through the use of compiled expressions

## How to use

```cs
var testInstance = new TestObject();
var accessorSomeInt = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeInt);
var readValue = (int)accessorSomeInt.Get(testInstance);
accessorSomeInt.Set(testInstance, 5);
```
