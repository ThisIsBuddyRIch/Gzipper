namespace Gzipper
{
	public class Settings
	{
		public Settings(int parallelismLevel, int inputBufferSize, int outputBufferSize, int chunkSize)
		{
			ParallelismLevel = parallelismLevel;
			InputBufferSize = inputBufferSize;
			OutputBufferSize = outputBufferSize;
			ChunkSize = chunkSize;
		}

		public int ParallelismLevel { get; }

		public int InputBufferSize { get; }

		public int OutputBufferSize { get; }

		public int ChunkSize { get; }

		public override string ToString()
		{
			return $"{nameof(ParallelismLevel)}: {ParallelismLevel}, {nameof(InputBufferSize)}: {InputBufferSize}, " +
			       $"{nameof(OutputBufferSize)}: {OutputBufferSize}, {nameof(ChunkSize)}: {ChunkSize}";
		}
	}
}