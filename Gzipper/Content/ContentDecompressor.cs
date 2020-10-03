using System.IO;
using System.IO.Compression;

namespace Gzipper.Content
{
	public class ContentDecompressor : IContentProcessor
	{
		public byte[] Process(byte[] content)
		{
			using var targetStream = new MemoryStream();
			using var sourceStream = new MemoryStream(content);
			using var decompressionStream = new GZipStream(sourceStream, CompressionMode.Decompress);
			decompressionStream.CopyTo(targetStream);
			return targetStream.ToArray();
		}
	}
}