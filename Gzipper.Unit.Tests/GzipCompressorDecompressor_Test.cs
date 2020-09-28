using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Gzipper.Gzip;
using Gzipper.Input;
using NSubstitute;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class GzipCompressorDecompressor_Test
	{
		private IFileService _fileService;

		[SetUp]
		public void SetUp()
		{
			_fileService = Substitute.For<IFileService>();
		}

		[Test]
		[TestCase(10)]
		[TestCase(1000)]
		[TestCase(10000)]
		[TestCase(100000)]
		public void CompressDecompress_ShouldWork(int randomContentSize)
		{
			using var expectedDecompressedData = new MemoryStream(GetRandomContent(1000));
			using var compressedData = new MemoryStream();
			const string compressFilePath = "compress";
			_fileService.GetReader(compressFilePath).Returns(new BinaryReader(expectedDecompressedData));
			_fileService.GetWriter(compressFilePath).Returns(new BinaryWriter(compressedData));
			var compressor = new GzipCompressor(_fileService);
			compressor.Compress(InputModel.Success(OperationType.Compress, compressFilePath, compressFilePath));

			const string decompressFilePath = "decompress";
			using var actualDecompressedData = new MemoryStream();
			_fileService.GetReader(decompressFilePath).Returns(new BinaryReader(new MemoryStream(compressedData.ToArray())));
			_fileService.GetWriter(decompressFilePath).Returns(new BinaryWriter(actualDecompressedData));

			var decompressor = new GzipDecompressor(_fileService);
			decompressor.Decompress(InputModel.Success(OperationType.Decompress, decompressFilePath, decompressFilePath));
			compressedData.ToArray().Should().NotBeEmpty();
			actualDecompressedData.ToArray().Should().NotBeEmpty();
			Convert.ToBase64String(actualDecompressedData.ToArray()).Should()
				.Be(Convert.ToBase64String(expectedDecompressedData.ToArray()));
		}

		private static byte[] GetRandomContent(int length)
		{
			var random = new Random();
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var str = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
			return Encoding.UTF8.GetBytes(str);
		}
	}
}