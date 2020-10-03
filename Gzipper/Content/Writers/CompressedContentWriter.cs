using System;
using System.IO;
using System.Linq;
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

		public void Write(BlockingDictionary<int, byte[]> outputDictionary, BinaryWriter binaryWriter)
		{
			var id = 0;
			while (!outputDictionary.IsComplete())
			{
				try
				{
					var chunk = outputDictionary.GetByKey(id);
					var contentToWrite = BitConverter.GetBytes(chunk.Length).Concat(chunk).ToArray();
					binaryWriter.Write(contentToWrite);
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
}