using System;
using System.IO;
using Gzipper.Logger;

namespace Gzipper.Content.Readers
{
	public class CompressedContentReader : IContentReader
	{
		private readonly ILogger _logger;

		public CompressedContentReader(ILogger logger)
		{
			_logger = logger;
		}

		public void Read(BlockingDictionary<int, Chunk> dictionary, BinaryReader reader)
		{
			var i = 0;
			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				var length = reader.ReadInt32();
				var content = reader.ReadBytes(length);
				dictionary.Add(i, new Chunk(content));
				_logger.Write($"Read compressed chunk {i}, length {length}");
				i++;
			}
		}
	}
}