using System;
using System.Diagnostics;
using Gzipper.Content;
using Gzipper.Gzip;
using Gzipper.Input;
using Gzipper.Logger;

namespace Gzipper
{
	public static class Gzipper
	{
		public static int Main(string[] args)
		{
			var logger = new ConsoleLogger();
			try
			{
				var stopwatch = new Stopwatch();
				stopwatch.Start();
				var fileService = new FileService();
				var inputParser = new InputParser(new FileService());

				var inputParseResult = inputParser.Parse(args);
				if (inputParseResult.IsFail)
				{
					logger.Write(inputParseResult.ErrorMessage!);
					return 1;
				}

				var settings = new Settings(8, 1000, 2000, 1024 * 1024);
				var gzipProcessorFactory = new GzipProcessorFactory(fileService, logger, settings);
				logger.Write($"--- Start process with settings {settings} ---");
				gzipProcessorFactory
					.Create(inputParseResult.OperationType)
					.Process(inputParseResult.InputFilePath!, inputParseResult.OutputFilePath!);
				stopwatch.Stop();
				logger.Write($"--- Finish process {stopwatch.Elapsed} ---");
				return 0;
			}
			catch (Exception e)
			{
				logger.Write($"Something went wrong: {e.Message}");
				return 1;
			}
		}
	}
}