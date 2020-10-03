using System;
using Gzipper.Content;
using Gzipper.Logger;

namespace Gzipper.Gzip
{
	public  class ChunkProcessor : IChunkProcessor
	{
		private readonly IContentProcessor _contentProcessor;
		private readonly ILogger _logger;

		public ChunkProcessor(IContentProcessor contentProcessor, ILogger logger)
		{
			_contentProcessor = contentProcessor;
			_logger = logger;
		}

		public void ProcessChunks(BlockingDictionary<int, byte[]> inputDictionary, BlockingDictionary<int, byte[]> outputDictionary)
		{
			while (!inputDictionary.IsComplete())
			{
				try
				{
					var (id, chunk) = inputDictionary.GetFirstItem();
					var processedChunk = _contentProcessor.Process(chunk);
					outputDictionary.Add(id, processedChunk);
					_logger.Write($"Process chunk {id}");
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}
	}
}