using System;
using System.Linq;
using FluentAssertions;
using Gzipper.ContentModels;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class ContentTable_Test
	{
		[Test]
		public void ContentTable_WhenSerialize_ShouldDeserialize()
		{
			var expected = new ContentTable();
			expected.Add(new ContentTableItem(1000, 300, 400));
			expected.Add(new ContentTableItem(4589, 200, 5678));
			expected.Add(new ContentTableItem(1238, 546, 12312));
			var actualItems = new ContentTable(expected.Serialize())
				.GetItems();
			var expectedItems = expected.GetItems()
				.OrderBy(x => x.CompressedContentPosition)
				.ToArray();
			foreach (var i in Enumerable.Range(0, actualItems.Length))
			{
				Assert(expectedItems[i], actualItems[i]);
			}
		}

		[Test]
		public void ContentTable_WhenWrongSerializedData_ShouldThrow()
		{
			Func<ContentTableItem> act = () => new ContentTableItem(new byte[]{ 0, 1, 2});
			act.Should().Throw<ArgumentException>();
		}

		private static void Assert(ContentTableItem expected, ContentTableItem actual)
		{
			actual.CompressedContentPosition.Should().Be(expected.CompressedContentPosition);
			actual.CompressedContentChunkSize.Should().Be(expected.CompressedContentChunkSize);
			actual.OriginalPosition.Should().Be(expected.OriginalPosition);
		}
	}
}