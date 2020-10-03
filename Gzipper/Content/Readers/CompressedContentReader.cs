using System.Diagnostics;
using System.IO;
using Gzipper.Infra;
using Gzipper.Infra.Logger;

namespace Gzipper.Content.Readers
{
	public class CompressedContentReader : IContentReader
	{
		private readonly ILogger _logger;
		private readonly Stopwatch _timer;

		public CompressedContentReader(ILogger logger, TimerProvider timerProvider)
		{
			_logger = logger;
			_timer = timerProvider.CreateTimer("Read From I/O");
		}

		public void Read(BlockingDictionary<int, byte[]> dictionary, BinaryReader reader)
		{
			var i = 0;
			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				_timer.Start();
				var length = reader.ReadInt32();
				var content = reader.ReadBytes(length);
				_timer.Stop();
				dictionary.Add(i, content);
				_logger.Write($"Read compressed chunk {i}, length {length}");
				i++;
			}
		}
	}
}