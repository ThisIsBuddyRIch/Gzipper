using System;
using System.IO;
using Gzipper.Logger;

namespace Gzipper.Content.Writers
{
	public class ContentWriter : IContentWriter
	{
		private readonly ILogger _logger;

		public ContentWriter(ILogger logger)
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
					binaryWriter.Write(chunk);
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
}