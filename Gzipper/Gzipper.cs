using System;
using Gzipper.Gzip;
using Gzipper.Input;

namespace Gzipper
{
	public static class Gzipper
	{
		public static int Main(string[] args)
		{
			try
			{
				var fileService = new FileService();
				var inputParser = new InputParser(new FileService());
				var inputParseResult = inputParser.Parse(args);
				if (inputParseResult.IsFail)
				{
					Console.WriteLine(inputParseResult.ErrorMessage);
					return 1;
				}

				var gzipProcessor = new GzipProcessor(new GzipCompressor(fileService), new GzipDecompressor(fileService));
				gzipProcessor.Process(inputParseResult);
				return 0;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Something went wrong: {e.Message}");
				return 1;
			}
		}
	}
}