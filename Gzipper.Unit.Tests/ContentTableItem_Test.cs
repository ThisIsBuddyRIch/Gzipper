using System;
using FluentAssertions;
using Gzipper.ContentModels;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class ContentTableItem_Test
	{
		[Test]
		public void ContentTableItem_WhenSerialize_ShouldDeserialize()
		{
			var expected = new ContentTableItem(1000, 500, 400);
			var actual = new ContentTableItem(expected.Serialize());

			actual.CompressedContentPosition.Should().Be(expected.CompressedContentPosition);
			actual.CompressedContentChunkSize.Should().Be(expected.CompressedContentChunkSize);
			actual.OriginalPosition.Should().Be(expected.OriginalPosition);
		}

		[Test]
		public void ContentTableItem_WhenInvalidSize_ShouldThrowError()
		{
			Func<ContentTableItem> act = () => new ContentTableItem(new byte[]{ 0, 1, 2});
			act.Should().Throw<ArgumentException>();
		}
	}
}