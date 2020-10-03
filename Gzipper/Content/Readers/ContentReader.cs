using System.Diagnostics;
using System.IO;
using Gzipper.Infra;
using Gzipper.Infra.Logger;

namespace Gzipper.Content.Readers
{
	public class ContentReader : IContentReader
	{
		private readonly ILogger _logger;
		private readonly int _chunkSize;
		private readonly Stopwatch _timer;

		public ContentReader(ILogger logger, int chunkSize, TimerProvider timerProvider)
		{
			_logger = logger;
			_chunkSize = chunkSize;
			_timer = timerProvider.CreateTimer("Read From I/O");
		}

		public void Read(BlockingDictionary<int, byte[]> dictionary, BinaryReader reader)
		{
			var i = 0;

			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				_timer.Start();
				var content = reader.ReadBytes(_chunkSize);
				_timer.Stop();
				dictionary.Add(i, content);
				_logger.Write($"Read chunk {i}");
				i++;
			}
		}
	}
}