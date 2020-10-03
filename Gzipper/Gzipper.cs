using System;
using Gzipper.Infra;
using Gzipper.Infra.Logger;
using Gzipper.Input;
using Microsoft.Extensions.Configuration;

namespace Gzipper
{
	public static class Gzipper
	{
		public static int Main(string[] args)
		{
			var logger = new ConsoleLogger();
			var timerProvider = new TimerProvider();
			try
			{
				var stopwatch = timerProvider.CreateTimer("total");
				stopwatch.Start();
				var fileService = new FileService();
				var inputParser = new InputParser(new FileService());

				var inputParseResult = inputParser.Parse(args);
				if (inputParseResult.IsFail)
				{
					logger.Write(inputParseResult.ErrorMessage!);
					return 1;
				}

				var settings = GetSettings();

				var gzipProcessorFactory = new GzipProcessorFactory(fileService, logger, settings, timerProvider);
				logger.Write($"--- Start process with settings {settings} ---");
				gzipProcessorFactory
					.Create(inputParseResult.OperationType)
					.Process(inputParseResult.InputFilePath!, inputParseResult.OutputFilePath!);
				stopwatch.Stop();
				logger.Write(timerProvider.GetReport());
				logger.Write($"--- Finish process ---");
				return 0;
			}
			catch (Exception e)
			{
				logger.Write($"Something went wrong: {e.Message}");
				return 1;
			}
		}

		private static Settings GetSettings()
		{
			return new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", false, false)
				.Build()
				.GetSection("settings")
				.Get<Settings>();
		}
	}
}