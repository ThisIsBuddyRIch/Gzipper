using Newtonsoft.Json;

namespace Gzipper.Infra
{
	[JsonObject("settings")]
	public class Settings
	{
		[JsonConstructor]
		public Settings()
		{
		}

		public Settings(int parallelismLevel, int inputBufferSize, int outputBufferSize, int chunkSize)
		{
			ParallelismLevel = parallelismLevel;
			InputBufferSize = inputBufferSize;
			OutputBufferSize = outputBufferSize;
			ChunkSize = chunkSize;
		}

		[JsonProperty("parallelismLevel")] public int ParallelismLevel { get; set; }

		[JsonProperty("inputBufferSize")] public int InputBufferSize { get; set; }

		[JsonProperty("outputBufferSize")] public int OutputBufferSize { get; set; }

		[JsonProperty("chunkSize")] public int ChunkSize { get; set; }

		public override string ToString()
		{
			return $"{nameof(ParallelismLevel)}: {ParallelismLevel}, {nameof(InputBufferSize)}: {InputBufferSize}, " +
			       $"{nameof(OutputBufferSize)}: {OutputBufferSize}, {nameof(ChunkSize)}: {ChunkSize}";
		}
	}
}