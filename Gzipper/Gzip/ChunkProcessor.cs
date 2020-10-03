using System;
using Gzipper.Content;
using Gzipper.Logger;

namespace Gzipper.Gzip
{
	public  class ChunkProcessor : IChunkProcessor
	{
		private readonly IContentProcessor _contentProcessor;
		private readonly ILogger _logger;

		public ChunkProcessor(IContentProcessor contentProcessor, ILogger _logger)
		{
			_contentProcessor = contentProcessor;
			this._logger = _logger;
		}

		public void ProcessChunks(BlockingDictionary<int, Chunk> inputDictionary, BlockingDictionary<int, Chunk> outputDictionary)
		{
			while (!inputDictionary.IsComplete())
			{
				try
				{
					var (id, chunk) = inputDictionary.GetFirstItem();
					var processedChunk = _contentProcessor.Process(chunk.Content);
					outputDictionary.Add(id, new Chunk(processedChunk));
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