using System.IO;
using System.IO.Compression;

namespace Gzipper.Content
{
	public class ContentCompressor : IContentProcessor
	{
		public byte[] Process(byte[] content)
		{
			using var targetStream = new MemoryStream();
			using var compressionStream = new GZipStream(targetStream, CompressionMode.Compress);
			compressionStream.Write(content, 0, content.Length);
			compressionStream.Close();
			return targetStream.ToArray();
		}
	}
}