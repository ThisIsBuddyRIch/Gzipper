using System;
using System.IO;
using Gzipper.Logger;

namespace Gzipper.Content.Writers
{
	public class CompressedContentWriter : IContentWriter
	{
		private readonly ILogger _logger;

		public CompressedContentWriter(ILogger logger)
		{
			_logger = logger;
		}

		public void Write(BlockingDictionary<int, Chunk> outputDictionary, BinaryWriter binaryWriter)
		{
			var id = 0;
			while (!outputDictionary.IsComplete())
			{
				try
				{
					var chunk = outputDictionary.GetByKey(id);
					binaryWriter.Write(chunk.Content.Length);
					binaryWriter.Write(chunk.Content);
					_logger.Write($"Write compressed chunk {id}, length {chunk.Content.Length}");
					id++;
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}
	}
}