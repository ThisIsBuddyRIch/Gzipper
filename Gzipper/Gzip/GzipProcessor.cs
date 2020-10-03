using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Gzipper.Content;
using Gzipper.Content.Readers;
using Gzipper.Content.Writers;
using Gzipper.Input;
using Gzipper.Logger;

namespace Gzipper.Gzip
{
	public class GzipProcessor
	{
		private readonly IFileService _fileService;
		private readonly ILogger _logger;
		private readonly IContentReader _reader;
		private readonly IContentWriter _writer;
		private readonly IChunkProcessor _processor;
		private const int ParallelismLevel = 8;
		private const int InputMaxBufferSize = 1000;
		private const int OutputMaxBufferSize = 2000;

		public GzipProcessor(
			IFileService fileService,
			ILogger logger,
			IContentReader reader,
			IContentWriter writer,
			IChunkProcessor processor)
		{
			_fileService = fileService;
			_logger = logger;
			_reader = reader;
			_writer = writer;
			_processor = processor;
		}

		public void Process(string inputFilePath, string outputFilePath)
		{
			using var inputDictionary = new BlockingDictionary<int, Chunk>(
				InputMaxBufferSize,
				new SortedDictionary<int, Chunk>(new Dictionary<int, Chunk>()));
			using var outputDictionary = new BlockingDictionary<int, Chunk>(
				OutputMaxBufferSize,
				new Dictionary<int, Chunk>(OutputMaxBufferSize));
			var workerThreads = new List<Thread>();

			_logger.Write($"Start processing with parallelism level {ParallelismLevel}");

			using var binaryReader = _fileService.GetReader(inputFilePath);
			var readerThread = new Thread(() => _reader.Read(inputDictionary, binaryReader));

			workerThreads.AddRange(
				Enumerable.Range(0, ParallelismLevel)
					.Select(_ => new Thread(() => _processor.ProcessChunks(inputDictionary, outputDictionary))));

			using var binaryWriter = _fileService.GetWriter(outputFilePath);
			var writerThread = new Thread(() => _writer.Write(outputDictionary, binaryWriter));

			readerThread.Start();
			foreach (var thread in workerThreads)
			{
				thread.Start();
			}
			writerThread.Start();
			readerThread.Join();
			inputDictionary.Close();

			foreach (var thread in workerThreads)
			{
				thread.Join();
			}

			outputDictionary.Close();
			writerThread.Join();
		}
	}
}
