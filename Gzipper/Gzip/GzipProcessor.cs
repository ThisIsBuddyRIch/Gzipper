using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gzipper.Content.Readers;
using Gzipper.Content.Writers;
using Gzipper.Infra;
using Gzipper.Infra.Logger;

namespace Gzipper.Gzip
{
	public class GzipProcessor
	{
		private readonly IFileService _fileService;
		private readonly ILogger _logger;
		private readonly IContentReader _reader;
		private readonly IContentWriter _writer;
		private readonly IChunkProcessor _processor;
		private readonly Settings _settings;

		public GzipProcessor(
			IFileService fileService,
			ILogger logger,
			IContentReader reader,
			IContentWriter writer,
			IChunkProcessor processor,
			Settings settings)
		{
			_fileService = fileService;
			_logger = logger;
			_reader = reader;
			_writer = writer;
			_processor = processor;
			_settings = settings;
		}

		public void Process(string inputFilePath, string outputFilePath)
		{
			using var inputDictionary = new BlockingDictionary<int, byte[]>(
				_settings.InputBufferSize,
				new SortedDictionary<int, byte[]>(new Dictionary<int, byte[]>(_settings.InputBufferSize)));
			using var outputDictionary = new BlockingDictionary<int, byte[]>(
				_settings.OutputBufferSize,
				new Dictionary<int, byte[]>(_settings.OutputBufferSize));
			var workerThreads = new List<Thread>();

			_logger.Write($"Start processing with parallelism level {_settings.ParallelismLevel}");

			using var binaryReader = _fileService.GetReader(inputFilePath);
			var readerThread = new Thread(() => _reader.Read(inputDictionary, binaryReader));

			workerThreads.AddRange(
				Enumerable.Range(0, _settings.ParallelismLevel)
					.Select(_ => new Thread(() => _processor.ProcessChunks(inputDictionary, outputDictionary))));

			using var binaryWriter = _fileService.GetWriter(outputFilePath);
			var writerThread = new Thread(() => _writer.Write(outputDictionary, binaryWriter));

			readerThread.Start();
			foreach (var thread in workerThreads) thread.Start();
			writerThread.Start();
			readerThread.Join();
			inputDictionary.Close();

			foreach (var thread in workerThreads) thread.Join();

			outputDictionary.Close();
			writerThread.Join();
		}
	}
}