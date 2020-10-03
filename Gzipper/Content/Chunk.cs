namespace Gzipper.Content
{
	public class Chunk
	{
		public Chunk(byte[] content)
		{
			Content = content;
		}

		public byte[] Content { get; }
	}
}