using System;
using Gzipper.Input;

namespace Gzipper
{
	internal static class Program
	{
		private static int Main(string[] args)
		{
			var inputParser = new InputParser(new FileProvider());
			var parseResult = inputParser.Parse(args);
			if (parseResult.IsFail)
			{
				Console.WriteLine(parseResult.ErrorMessage);
				return 1;
			}

			return 0;
		}
	}
}