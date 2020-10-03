using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gzipper.Infra;
using Gzipper.Infra.Logger;

namespace Gzipper.Content.Writers
{
	public class CompressedContentWriter : IContentWriter
	{
		private readonly ILogger _logger;
		private readonly Stopwatch _timer;

		public CompressedContentWriter(ILogger logger, TimerProvider timerProvider)
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
					var contentToWrite = BitConverter.GetBytes(chunk.Length).Concat(chunk).ToArray();
					_timer.Start();
					binaryWriter.Write(contentToWrite);
					_timer.Stop();
					_logger.Write($"Write compressed chunk {id}, length {chunk.Length}");
					id++;
				}
				catch (InvalidOperationException)
				{
					break;
				}
		}
	}
}