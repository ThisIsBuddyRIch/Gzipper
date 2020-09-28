using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Gzipper.Integration.Test
{
	public class Gzipper_Test
	{
		[Test]
		[TestCase("TestData/Picture.jpg")]
		[TestCase("TestData/Text.txt")]
		public void CompressDecompress_ShouldWork(string inputFileName)
		{
			var compressedFileName = $"{inputFileName}.gz";
			var compressResult = Gzipper.Main(new[] {"compress", inputFileName, compressedFileName});
			compressResult.Should().Be(0);
			var decompressedFileName = $"{inputFileName}-dc{Path.GetExtension(inputFileName)}";
			var decompressResult = Gzipper.Main(new[] {"decompress", compressedFileName, decompressedFileName});
			decompressResult.Should().Be(0);
			Convert.ToBase64String(File.ReadAllBytes(inputFileName))
				.Should()
				.Be(Convert.ToBase64String(File.ReadAllBytes(decompressedFileName)));
		}
	}
}