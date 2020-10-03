using System;
using System.Diagnostics;
using System.IO;
using Gzipper.Infra;
using Gzipper.Infra.Logger;

namespace Gzipper.Content.Writers
{
	public class ContentWriter : IContentWriter
	{
		private readonly ILogger _logger;
		private readonly Stopwatch _timer;

		public ContentWriter(ILogger logger, TimerProvider timerProvider)
		{
			_logger = logger;
			_timer = timerProvider.CreateTimer("Write to I/O");
		}

		public void Write(BlockingDictionary<int, byte[]> outputDictionary, BinaryWriter binaryWriter)
		{
			var id = 0;
			while (!outputDictionary.IsComplete())
				try
				{
					var chunk = outputDictionary.GetByKey(id);
					_timer.Start();
					binaryWriter.Write(chunk);
					_timer.Stop();
					_logger.Write($"Write chunk {id}");
					id++;
				}
				catch (InvalidOperationException)
				{
					break;
				}
		}
	}
}