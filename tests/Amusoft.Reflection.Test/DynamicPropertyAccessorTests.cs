// Copyright 2018 Andreas Müller
// This file is a part of Amusoft and is licensed under Apache 2.0
// See https://github.com/taori/Amusoft.Reflection/blob/master/LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Amusoft.Reflection.Emit;
using Amusoft.Reflection.Test.Utility;
using NUnit.Framework;

namespace Amusoft.Reflection.Test
{
	[TestFixture]
	public class DynamicPropertyAccessorTests
	{
		private Measurement Measure(string message)
		{
			return new Measurement(message);
		}
		
		[Test]
		public void GetSetMethodThrow()
		{
			Assert.Throws<NotSupportedException>(() => DynamicPropertyAccessorFactory.Create<TestObject>(d => d.ErrorMethod()));
		}

		[Test]
		public void TestSetterCalls()
		{
			var instance = new TestObject();
			var stream = new MemoryStream();
			var accessor = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeBoxedObject);
			accessor.Set(instance, 1);
			accessor.Set(instance, "");
			accessor.Set(instance, null);
			accessor.Set(instance, (DateTime?)null);
			accessor.Set(instance, new object());
			accessor.Set(instance, default(int));
			accessor.Set(instance, default(short));
			accessor.Set(instance, default(short?));
			accessor.Set(instance, default(double));
			accessor.Set(instance, default(double?));
			accessor.Set(instance, default(MemoryStream));
			accessor.Set(instance, stream);

			stream.Dispose();
		}

		[Test]
		public void GetSetInt()
		{
			var testInstance = new TestObject();

			var accessorSomeInt = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeInt);

			Assert.That((int)accessorSomeInt.Get(testInstance), Is.EqualTo(default(int)));

			var setValuesInt = 5;

			accessorSomeInt.Set(testInstance, setValuesInt);

			Assert.That((int)accessorSomeInt.Get(testInstance), Is.EqualTo(setValuesInt));
		}

		[Test]
		public void GetSetEnum()
		{
			var testInstance = new TestObject();

			var accessorSomeInt = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeEnum);

			Assert.That((TestEnum)accessorSomeInt.Get(testInstance), Is.EqualTo(default(TestEnum)));

			var setValuesInt = TestEnum.ValB;

			accessorSomeInt.Set(testInstance, setValuesInt);

			Assert.That((TestEnum)accessorSomeInt.Get(testInstance), Is.EqualTo(setValuesInt));
		}

		[Test]
		public void GetSetString()
		{
			var testInstance = new TestObject();

			var accessorContent = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.Content);

			Assert.That((string)accessorContent.Get(testInstance), Is.EqualTo(default(string)));

			var setValuesString = "hi";

			accessorContent.Set(testInstance, setValuesString);

			Assert.That((string)accessorContent.Get(testInstance), Is.EqualTo(setValuesString));
		}

		[Test]
		public void GetSetObject()
		{
			var testInstance = new TestObject();

			var accessorSomeBoxedObject = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeBoxedObject);

			Assert.That((object)accessorSomeBoxedObject.Get(testInstance), Is.EqualTo(default(object)));

			var setValuesBoxedObject = (object)500;

			accessorSomeBoxedObject.Set(testInstance, setValuesBoxedObject);

			Assert.That(accessorSomeBoxedObject.Get(testInstance), Is.EqualTo(setValuesBoxedObject));
		}

		[Test]
		public void GetSetDateTime()
		{
			var testInstance = new TestObject();

			var accessorDateTimeVal = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.DateTimeVal);
			Assert.That((DateTime?)accessorDateTimeVal.Get(testInstance), Is.EqualTo(default(DateTime?)));

			var setValuesDate = DateTime.Now;

			accessorDateTimeVal.Set(testInstance, setValuesDate);
			Assert.That((DateTime?)accessorDateTimeVal.Get(testInstance), Is.EqualTo(setValuesDate));
		}

		[Test]
		public void GetSetPropertyInfo()
		{
			var testInstance = new TestObject();

			var accessorRandomPropertyInfo = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.RandomPropertyInfo);
			Assert.That((PropertyInfo)accessorRandomPropertyInfo.Get(testInstance), Is.EqualTo(default(PropertyInfo)));

			var varSetValuePropertyInfo = typeof(TestObject).GetProperty("ErrorMethod");

			accessorRandomPropertyInfo.Set(testInstance, varSetValuePropertyInfo);

			Assert.That((PropertyInfo)accessorRandomPropertyInfo.Get(testInstance), Is.EqualTo(varSetValuePropertyInfo));
		}

		[Test]
		public void AccessorDuplicationCreationAccessPerformance()
		{
			var sw = new Stopwatch();
			var elapsed = new List<long>();
			long firstElapse;
			
			sw.Start();
			var firstCreation = DynamicPropertyAccessorFactory.Create(typeof(TestObject), nameof(TestObject.SomeInt));
			sw.Stop();
			firstElapse = sw.ElapsedTicks;
			Debug.WriteLine($"creation time 1 : ms: {sw.ElapsedMilliseconds} ticks: {sw.ElapsedTicks}");

			for (int i = 0; i < 1000; i++)
			{
				sw.Restart();
				var accCreation2 = DynamicPropertyAccessorFactory.Create(typeof(TestObject), nameof(TestObject.SomeInt));
				sw.Stop();
				elapsed.Add(sw.ElapsedTicks);
			}

			var averageCachedTime = new TimeSpan((long)elapsed.Average());
			
			Debug.WriteLine($"creation time 2 : ms: {averageCachedTime.TotalMilliseconds} ticks: {averageCachedTime.Ticks}");

			var expectedImprovement = 5000;
			var dividend = averageCachedTime.Ticks > 0 ? averageCachedTime.Ticks : 1;
			double fraction = (firstElapse / dividend);
			Assert.GreaterOrEqual(fraction, expectedImprovement, string.Format($"Performance should be >{{0}} times faster for an accessor which already exists but it is only {fraction}.", expectedImprovement));
		}

		/**
		 * Ergebnis war bei einem aktuellen Test:
		 * Clr : 700ms
		 * DPA : 1236ms
		 * Reflection : 3971ms
		 */
		[Test]
		public void PerformanceTest()
		{
			var iterations = 1_000_000;
			var testObjects = Enumerable.Range(0, iterations).Select(i => new TestObject()).ToList();
			TimeSpan reflectionTimeSpan;
			TimeSpan dynPropertyAccessorTimeSpan;
			TimeSpan clrAccessorTimespan;

			var prop = typeof (TestObject).GetProperty("SomeInt");
			
			using (var measurement = Measure("Retrieval of Clr Accessors"))
			{
				for (int i = 0; i < iterations; i++)
				{
					var subject = testObjects[i];

					subject.SomeInt = 1;
					subject.Content = "Hi";
					subject.SomeBoxedObject = 500;
					subject.DateTimeVal = DateTime.Now;
					subject.RandomPropertyInfo = prop;

					var t1 = subject.SomeInt;
					var t2 = subject.Content ;
					var t3 = subject.SomeBoxedObject;
					var t4 = subject.DateTimeVal;
					var t5 = subject.RandomPropertyInfo;
				}

				clrAccessorTimespan = measurement.Elapsed;
			}

			var accessorSomeInt = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeInt);
			var accessorContent = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.Content);
			var accessorSomeBoxedObject = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.SomeBoxedObject);
			var accessorDateTimeVal = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.DateTimeVal);
			var accessorRandomPropertyInfo = DynamicPropertyAccessorFactory.Create<TestObject>(d => d.RandomPropertyInfo);
			
			using (var measurement = Measure("Retrieval of DynamicPropertyAccessors"))
			{
				
				for (int i = 0; i < iterations; i++)
				{
					var subject = testObjects[i];

					accessorSomeInt.Set(subject, 1);
					accessorContent.Set(subject, "Hi");
					accessorSomeBoxedObject.Set(subject, 500);
					accessorDateTimeVal.Set(subject, DateTime.Now);
					accessorRandomPropertyInfo.Set(subject, prop);

					accessorSomeInt.Get(subject);
					accessorContent.Get(subject);
					accessorSomeBoxedObject.Get(subject);
					accessorDateTimeVal.Get(subject);
					accessorRandomPropertyInfo.Get(subject);
				}

				dynPropertyAccessorTimeSpan = measurement.Elapsed;
			}

			var pInt = typeof(TestObject).GetProperty("SomeInt");
			var pContent = typeof(TestObject).GetProperty("Content");
			var pSomeBoxedObject = typeof(TestObject).GetProperty("SomeBoxedObject");
			var pRandomPropertyInfo = typeof(TestObject).GetProperty("RandomPropertyInfo");
			var pDateTimeVal = typeof(TestObject).GetProperty("DateTimeVal");

			using (var measurement = Measure("Retrieval of reflection properties"))
			{

				for (int i = 0; i < iterations; i++)
				{
					var subject = testObjects[i];

					pInt.SetValue(subject, 1);
					pContent.SetValue(subject, "hi");
					pSomeBoxedObject.SetValue(subject, 500);
					pDateTimeVal.SetValue(subject, DateTime.Now);
					pRandomPropertyInfo.SetValue(subject, prop);

					pInt.GetValue(subject);
					pContent.GetValue(subject);
					pSomeBoxedObject.GetValue(subject);
					pDateTimeVal.GetValue(subject);
					pRandomPropertyInfo.GetValue(subject);
				}

				reflectionTimeSpan = measurement.Elapsed;
			}

			Console.WriteLine($"clrAccessorTimespan: {clrAccessorTimespan.TotalMilliseconds} ms");
			Console.WriteLine($"reflection: {reflectionTimeSpan.TotalMilliseconds} ms");
			Console.WriteLine($"dynPropertyAccessorTimeSpan: {dynPropertyAccessorTimeSpan.TotalMilliseconds} ms");

			Assert.GreaterOrEqual(reflectionTimeSpan.TotalMilliseconds, dynPropertyAccessorTimeSpan.TotalMilliseconds);
			Assert.GreaterOrEqual(dynPropertyAccessorTimeSpan.TotalMilliseconds, clrAccessorTimespan.TotalMilliseconds);
		}


		public enum TestEnum
		{
			ValA,
			ValB
		}
		
		public class TestObject
		{
			public TestEnum SomeEnum { get; set; }
			public int SomeInt { get; set; }
			public string Content { get; set; }
			public object SomeBoxedObject { get; set; }
			public PropertyInfo RandomPropertyInfo { get; set; }
			public DateTime? DateTimeVal { get; set; }

			public int ErrorMethod()
			{
				return 1;
			}
		}

		public class SomeOtherTestObject
		{
			void DoSomething(TestObject o)
			{
				o.SomeInt = 5;
			}
		}
	}
}
