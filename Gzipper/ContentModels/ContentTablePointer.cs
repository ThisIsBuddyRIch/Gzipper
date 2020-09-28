using System;

namespace Gzipper.ContentModels
{
	public class ContentTablePointer
	{
		public const int PointerLength = sizeof(long);

		public ContentTablePointer(byte[] serializedPointer)
		{
			if (serializedPointer.Length != sizeof(long))
			{
				throw new ArgumentException("Invalid serialized data");
			}

			Position = BitConverter.ToInt64(serializedPointer);
		}

		public ContentTablePointer(long position)
		{
			Position = position;
		}

		public long Position { get; }

		public byte[] Serialize()
		{
			return BitConverter.GetBytes(Position);
		}
	}
}