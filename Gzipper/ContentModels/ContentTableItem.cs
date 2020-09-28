using System;
using System.Linq;

namespace Gzipper.ContentModels
{
	public class ContentTableItem
	{
		public const int SerializedLength = sizeof(long) * 2 + sizeof(int);

		public ContentTableItem(
			long compressedContentPosition,
			int compressedContentChunkSize,
			long originalPosition)
		{
			CompressedContentPosition = compressedContentPosition;
			CompressedContentChunkSize = compressedContentChunkSize;
			OriginalPosition = originalPosition;
		}

		public ContentTableItem(byte[] serializedData)
		{
			if (serializedData.Length != SerializedLength)
			{
				throw new ArgumentException($"Corrupted table item, invalid length {serializedData.Length}");
			}

			CompressedContentPosition = BitConverter.ToInt64(serializedData[..sizeof(long)]);
			CompressedContentChunkSize = BitConverter.ToInt32(serializedData[sizeof(long)..^sizeof(long)]);
			OriginalPosition = BitConverter.ToInt64(serializedData[^sizeof(long)..]);
		}

		public long CompressedContentPosition { get; }

		public int CompressedContentChunkSize { get; }

		public long OriginalPosition { get; }

		public byte[] Serialize()
		{
			return BitConverter.GetBytes(CompressedContentPosition)
				.Concat(BitConverter.GetBytes(CompressedContentChunkSize))
				.Concat(BitConverter.GetBytes(OriginalPosition))
				.ToArray();
		}
	}
}