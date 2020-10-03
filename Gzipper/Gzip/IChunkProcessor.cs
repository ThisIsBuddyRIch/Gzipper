using Gzipper.Content;

namespace Gzipper.Gzip
{
	public interface IChunkProcessor
	{
		void ProcessChunks(BlockingDictionary<int, Chunk> inputDictionary, BlockingDictionary<int, Chunk> outputDictionary);
	}
}