using System.IO;

namespace Gzipper.Content.Readers
{
	public interface IContentReader
	{
		void Read(BlockingDictionary<int, Chunk> dictionary, BinaryReader reader);
	}
}