using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Gzipper.ContentModels;
using Gzipper.Input;

namespace Gzipper.Gzip
{
	public class GzipDecompressor
	{
		private readonly IFileService _fileService;
		private const int ParallelismLevel = 8;

		public GzipDecompressor(IFileService fileService)
		{
			_fileService = fileService;
		}

		public void Decompress(InputModel inputModel)
		{
			using var reader = _fileService.GetReader(inputModel.InputFilePath!);
			var contentTable = ContentTableReader.Read(reader);
			using var inputPipe = new BlockingPipe<Chunk>(1000);
			using var outputPipe = new BlockingPipe<Chunk>(1000);
			var readerThread = new Thread(() => ReadChunks(inputPipe, contentTable, reader));
			var workerThreads = new List<Thread>();
			using var writer = _fileService.GetWriter(inputModel.OutputFilePath!);
			workerThreads.AddRange(
				Enumerable.Range(0, ParallelismLevel)
					.Select(_ => new Thread(() => DecompressChunks(inputPipe, outputPipe))));

			var writerThread = new Thread(() => WriteToOutput(outputPipe, writer));
			readerThread.Start();
			foreach (var thread in workerThreads)
			{
				thread.Start();
			}
			writerThread.Start();

			readerThread.Join();
			inputPipe.Close();
			foreach (var thread in workerThreads)
			{
				thread.Join();
			}
			outputPipe.Close();
			writerThread.Join();
		}

		private static void ReadChunks(BlockingPipe<Chunk> pipe, ContentTable contentTable, BinaryReader reader)
		{
			foreach (var item in contentTable.GetItems())
			{
				reader.BaseStream.Seek(item.CompressedContentPosition, SeekOrigin.Begin);
				var chunkContent = reader.ReadBytes(item.CompressedContentChunkSize);
				pipe.Produce(new Chunk(chunkContent, item.OriginalPosition));
			}
		}

		private static void DecompressChunks(BlockingPipe<Chunk> inputPipe, BlockingPipe<Chunk> outputPipe)
		{
			while (!inputPipe.IsComplete())
			{
				try
				{
					var chunk = inputPipe.Consume();
					var decompressContent = DecompressContent(chunk.Content);
					outputPipe.Produce(new Chunk(decompressContent, chunk.Position));
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}

		private static void WriteToOutput(BlockingPipe<Chunk> outputPipe, BinaryWriter writer)
		{
			while (!outputPipe.IsComplete())
			{
				try
				{
					WriteContentToFile(outputPipe.Consume(), writer);
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}

		private static void WriteContentToFile(Chunk chunk, BinaryWriter writer)
		{
			if (chunk.Content.Length == 0)
			{
				throw new ArgumentException("Content is empty");
			}

			writer.BaseStream.Seek(chunk.Position, SeekOrigin.Begin);
			writer.Write(chunk.Content);
		}

		private static byte[] DecompressContent(byte[] content)
		{
			using var sourceStream = new MemoryStream(content);
			using var targetStream = new MemoryStream();
			using var decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);
			decompressionStream.CopyTo(targetStream);
			return targetStream.ToArray();
		}
	}
}