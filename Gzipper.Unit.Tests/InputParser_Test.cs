using System;
using FluentAssertions;
using Gzipper.Input;
using NSubstitute;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class InputParser_Test
	{
		private IFileService _fileService;
		private InputParser testObj;

		[SetUp]
		public void SetUp()
		{
			_fileService = Substitute.For<IFileService>();
			testObj = new InputParser(_fileService);
		}

		[Test]
		public void InputParse_WhenWrongCountArgument_ShouldReturnError()
		{
			_fileService.IsFileExist(Arg.Any<string>()).Returns(true);
			var actual = testObj.Parse(Array.Empty<string>());
			actual.IsFail.Should().BeTrue();
			actual.ErrorMessage.Should().NotBeEmpty();
		}

		[Test]
		public void InputParse_WhenArgumentHasNull_ShouldReturnError()
		{
			_fileService.IsFileExist(Arg.Any<string>()).Returns(true);
			var actual = testObj.Parse(new[] {"compress", "path", null});
			actual.IsFail.Should().BeTrue();
			actual.ErrorMessage.Should().NotBeEmpty();
		}

		[Test]
		public void InputParse_WhenWrongOperation_ShouldReturnError()
		{
			_fileService.IsFileExist(Arg.Any<string>()).Returns(true);
			var actual = testObj.Parse(new[] {"magic", "path", "path"});
			actual.IsFail.Should().BeTrue();
			actual.ErrorMessage.Should().NotBeEmpty();
			actual.ErrorMessage.Should().Contain("Supported operations");
		}

		[Test]
		public void InputParse_WhenWrongInputFile_ShouldReturnError()
		{
			_fileService.IsFileExist(Arg.Any<string>()).Returns(false);
			var actual = testObj.Parse(new[] {"compress", "path", "path"});
			actual.IsFail.Should().BeTrue();
			actual.ErrorMessage.Should().NotBeEmpty();
			actual.ErrorMessage.Should().Contain("Input file has not found");
		}

		[Test]
		public void InputParse_WhenGoodArgument_ShouldReturnSuccess()
		{
			_fileService.IsFileExist(Arg.Any<string>()).Returns(true);
			var expected = new[] {"compress", "inputPath", "outputPath"};
			var actual = testObj.Parse(expected);
			actual.IsFail.Should().BeFalse();
			actual.ErrorMessage.Should().BeNullOrEmpty();
			actual.OperationType.Should().Be(OperationType.Compress);
			actual.InputFilePath.Should().Be(expected[1]);
			actual.OutputFilePath.Should().Be(expected[2]);
		}
	}
}