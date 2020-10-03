using System.IO;

namespace Gzipper.Content.Writers
{
	public interface IContentWriter
	{
		void Write(BlockingDictionary<int, byte[]> outputDictionary, BinaryWriter binaryWriter);
	}
}