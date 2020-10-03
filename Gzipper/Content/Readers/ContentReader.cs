using System.IO;
using Gzipper.Logger;

namespace Gzipper.Content.Readers
{
	public class ContentReader : IContentReader
	{
		private readonly ILogger _logger;
		private readonly int _chunkSize;

		public ContentReader(ILogger logger, int chunkSize)
		{
			_logger = logger;
			_chunkSize = chunkSize;
		}

		public void Read(BlockingDictionary<int, Chunk> dictionary, BinaryReader reader)
		{
			var i = 0;

			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				var content = reader.ReadBytes(_chunkSize);
				dictionary.Add(i, new Chunk(content));
				_logger.Write($"Read chunk {i}");
				i++;
			}
		}
	}
}