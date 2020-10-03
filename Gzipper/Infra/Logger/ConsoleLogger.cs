using System;

namespace Gzipper.Infra.Logger
{
	public class ConsoleLogger : ILogger
	{
		public void Write(string message)
		{
			Console.WriteLine(message);
		}
	}
}