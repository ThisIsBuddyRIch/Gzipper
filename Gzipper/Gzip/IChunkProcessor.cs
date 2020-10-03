namespace Gzipper.Gzip
{
	public interface IChunkProcessor
	{
		void ProcessChunks(BlockingDictionary<int, byte[]> inputDictionary,
			BlockingDictionary<int, byte[]> outputDictionary);
	}
}