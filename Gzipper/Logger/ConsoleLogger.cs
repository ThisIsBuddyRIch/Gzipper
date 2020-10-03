using System;

namespace Gzipper.Logger
{
	public class ConsoleLogger : ILogger
	{
		public void Write(string message)
		{
			Console.WriteLine(message);
		}
	}
}