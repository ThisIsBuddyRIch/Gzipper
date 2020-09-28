using System.Collections.Generic;
using System.IO;

namespace Gzipper.ContentModels
{
	public static class ContentTableReader
	{
		public static ContentTable Read(BinaryReader reader)
		{
			reader.BaseStream.Seek(-ContentTablePointer.PointerLength, SeekOrigin.End);
			var pointer = reader.ReadBytes(ContentTablePointer.PointerLength);
			var contentTablePointer = new ContentTablePointer(pointer);
			reader.BaseStream.Seek(contentTablePointer.Position, SeekOrigin.Begin);

			var contentTableBytes = new List<byte>();
			while (reader.BaseStream.Position < reader.BaseStream.Length - ContentTablePointer.PointerLength)
			{
				contentTableBytes.Add(reader.ReadByte());
			}

			reader.BaseStream.Seek(0, SeekOrigin.Begin);
			return new ContentTable(contentTableBytes.ToArray());
		}
	}
}