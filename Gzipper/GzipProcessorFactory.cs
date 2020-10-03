using System;
using Gzipper.Content;
using Gzipper.Content.Readers;
using Gzipper.Content.Writers;
using Gzipper.Gzip;
using Gzipper.Input;
using Gzipper.Logger;

namespace Gzipper
{
	public class GzipProcessorFactory
	{
		private readonly IFileService _fileService;
		private readonly ILogger _logger;
		private readonly int chunkSize = 1024 * 1024;

		public GzipProcessorFactory(IFileService fileService, ILogger logger)
		{
			_fileService = fileService;
			_logger = logger;
		}

		public GzipProcessor Create(OperationType operationType)
		{
			return operationType switch
			{
				OperationType.Compress => new GzipProcessor(
					_fileService,
					_logger,
					new ContentReader(_logger, chunkSize),
					new CompressedContentWriter(_logger),
					new ChunkProcessor(new ContentCompressor(), _logger)),
				OperationType.Decompress => new GzipProcessor(
					_fileService,
					_logger,
					new CompressedContentReader(_logger),
					new ContentWriter(_logger),
					new ChunkProcessor(new ContentDecompressor(), _logger)),
				_ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
			};
		}
	}
}