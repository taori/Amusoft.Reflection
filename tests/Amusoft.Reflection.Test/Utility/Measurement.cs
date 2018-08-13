using System;
using System.Diagnostics;

namespace Amusoft.Reflection.Test.Utility
{
	public class Measurement : IDisposable
	{
		private string _message;
		private DateTime _start;

		public Measurement(string message)
		{
			_message = message;
			_start = DateTime.Now;
		}

		public void Dispose()
		{
			Elapsed = DateTime.Now - _start;
			Debug.WriteLine($"{_message} elapsed in {Elapsed.TotalMilliseconds}ms.");
		}

		public TimeSpan Elapsed { get; set; }
	}
}