using System;
using Gzipper.Content;
using Gzipper.Content.Readers;
using Gzipper.Content.Writers;
using Gzipper.Gzip;
using Gzipper.Infra;
using Gzipper.Infra.Logger;
using Gzipper.Input;

namespace Gzipper
{
	public class GzipProcessorFactory
	{
		private readonly IFileService _fileService;
		private readonly ILogger _logger;
		private readonly Settings _settings;
		private readonly TimerProvider _timerProvider;

		public GzipProcessorFactory(IFileService fileService, ILogger logger, Settings settings,
			TimerProvider timerProvider)
		{
			_fileService = fileService;
			_logger = logger;
			_settings = settings;
			_timerProvider = timerProvider;
		}

		public GzipProcessor Create(OperationType operationType)
		{
			return operationType switch
			{
				OperationType.Compress => new GzipProcessor(
					_fileService,
					_logger,
					new ContentReader(_logger, _settings.ChunkSize, _timerProvider),
					new CompressedContentWriter(_logger, _timerProvider),
					new ChunkProcessor(new ContentCompressor(), _logger),
					_settings),
				OperationType.Decompress => new GzipProcessor(
					_fileService,
					_logger,
					new CompressedContentReader(_logger, _timerProvider),
					new ContentWriter(_logger, _timerProvider),
					new ChunkProcessor(new ContentDecompressor(), _logger),
					_settings),
				_ => throw new ArgumentOutOfRangeException(nameof(operationType), operationType, null)
			};
		}
	}
}