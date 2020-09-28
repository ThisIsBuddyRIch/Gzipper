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
	public class GzipCompressor
	{
		private readonly IFileService _fileService;
		private const int CompressChunkSize = 1024;
		private const int ParallelismLevel = 8;

		public GzipCompressor(IFileService fileService)
		{
			_fileService = fileService;
		}

		public void Compress(InputModel inputModel)
		{
			using var inputPipe = new BlockingPipe<Chunk>(1000);
			using var outputPipe = new BlockingPipe<Chunk>(1000);
			var contentTable = new ContentTable();
			var workerThreads = new List<Thread>();
			using var reader = _fileService.GetReader(inputModel.InputFilePath!);
			var readerThread = new Thread(() => ReadChunks(inputPipe, reader));
			using var writer = _fileService.GetWriter(inputModel.OutputFilePath!);
			workerThreads.AddRange(
				Enumerable.Range(0, ParallelismLevel)
					.Select(_ => new Thread(() => CompressChunks(inputPipe, outputPipe))));
			var writerThread = new Thread(() => WriteToOutput(outputPipe, writer, contentTable));
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

			WriteContentTable(writer, contentTable);
		}

		private static void ReadChunks(BlockingPipe<Chunk> pipe, BinaryReader reader)
		{
			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				var currentPosition = reader.BaseStream.Position;
				var content = reader.ReadBytes(CompressChunkSize);
				pipe.Produce(new Chunk(content, currentPosition));
			}
		}

		private static void CompressChunks(BlockingPipe<Chunk> inputPipe, BlockingPipe<Chunk> outputPipe)
		{
			while (!inputPipe.IsComplete())
			{
				try
				{
					var chunk = inputPipe.Consume();
					var compressedContent = CompressContent(chunk.Content);
					outputPipe.Produce(new Chunk(compressedContent, chunk.Position));
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}

		private static void WriteToOutput(BlockingPipe<Chunk> outputPipe, BinaryWriter binaryWriter, ContentTable contentTable)
		{
			while (!outputPipe.IsComplete())
			{
				try
				{
					var chunk = outputPipe.Consume();
					WriteContentToFile(chunk, binaryWriter, contentTable);
				}
				catch (InvalidOperationException)
				{
					break;
				}
			}
		}

		private static void WriteContentToFile(Chunk chunk, BinaryWriter writer, ContentTable contentTable)
		{
			if (chunk.Content.Length == 0)
			{
				throw new ArgumentException("Content is empty");
			}

			var currentPosition = writer.BaseStream.Position;
			writer.Write(chunk.Content);
			contentTable.Add(
				new ContentTableItem(
					currentPosition,
					chunk.Content.Length,
					chunk.Position));
		}

		private static byte[] CompressContent(byte[] content)
		{
			using var targetStream = new MemoryStream();
			using var compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
			compressionStream.Write(content, 0, content.Length);
			compressionStream.Close();
			return targetStream.ToArray();
		}

		private static void WriteContentTable(BinaryWriter writer, ContentTable contentTable)
		{
			var currentPosition = new ContentTablePointer(writer.BaseStream.Position);
			writer.Write(contentTable.Serialize());
			writer.Write(currentPosition.Serialize());
		}
	}
}
