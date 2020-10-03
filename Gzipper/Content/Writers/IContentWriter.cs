using System.IO;

namespace Gzipper.Content.Writers
{
	public interface IContentWriter
	{
		void Write(BlockingDictionary<int, Chunk> outputDictionary, BinaryWriter binaryWriter);
	}
}