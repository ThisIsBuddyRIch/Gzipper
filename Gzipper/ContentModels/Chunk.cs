namespace Gzipper.ContentModels
{
	public class Chunk
	{
		public Chunk(byte[] content, long position)
		{
			Content = content;
			Position = position;
		}

		public byte[] Content { get; }

		public long Position { get; }
	}
}