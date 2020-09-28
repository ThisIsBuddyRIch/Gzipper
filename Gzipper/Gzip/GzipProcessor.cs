using System;
using Gzipper.Input;

namespace Gzipper.Gzip
{
	public class GzipProcessor
	{
		private readonly GzipCompressor _compressor;
		private readonly GzipDecompressor _decompressor;

		public GzipProcessor(GzipCompressor compressor, GzipDecompressor decompressor)
		{
			_compressor = compressor;
			_decompressor = decompressor;
		}

		public void Process(InputModel inputModel)
		{
			switch (inputModel.OperationType)
			{
				case OperationType.Compress: _compressor.Compress(inputModel);
					break;
				case OperationType.Decompress: _decompressor.Decompress(inputModel);
					break;
				default:
					throw new ArgumentException($"Wrong operation type: {inputModel.OperationType}");
			}
		}
	}
}