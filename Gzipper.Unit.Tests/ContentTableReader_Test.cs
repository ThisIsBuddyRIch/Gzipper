using System;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Gzipper.ContentModels;
using NUnit.Framework;

namespace Gzipper.Unit.Tests
{
	public class ContentTableReader_Test
	{
		[Test]
		public void ReadContentTable_ShouldRead()
		{
			var expected = new ContentTable();
			expected.Add(new ContentTableItem(1000, 300, 400));
			expected.Add(new ContentTableItem(1238, 546, 12312));
			expected.Add(new ContentTableItem(4589, 200, 5678));

			var content = Encoding.Default.GetBytes(new string('x', 1000));
			var contentTablePointer = new ContentTablePointer(content.Length);
			var source = new MemoryStream(content.Concat(expected.Serialize()).Concat(contentTablePointer.Serialize()).ToArray());
			using var reader = new BinaryReader(source);
			var actual = ContentTableReader.Read(reader);
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


		private static void Assert(ContentTableItem expected, ContentTableItem actual)
		{
			actual.CompressedContentPosition.Should().Be(expected.CompressedContentPosition);
			actual.CompressedContentChunkSize.Should().Be(expected.CompressedContentChunkSize);
			actual.OriginalPosition.Should().Be(expected.OriginalPosition);
		}
	}
}