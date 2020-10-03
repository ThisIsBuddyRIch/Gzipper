using System.IO;
using System.IO.Compression;

namespace Gzipper.Content
{
	public class ContentCompressor : IContentProcessor
	{
		public byte[] Process(byte[] content)
		{
			using var sourceStream = new MemoryStream(content);
			using var targetStream = new MemoryStream();
			using GZipStream compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
			sourceStream.CopyTo(compressionStream);
			compressionStream.Close();
			return targetStream.ToArray();
		}
	}
}