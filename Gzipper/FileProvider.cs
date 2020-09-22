using System.IO;
using JetBrains.Annotations;

namespace Gzipper
{
	[UsedImplicitly]
	public class FileProvider : IFileProvider
	{
		public bool IsFileExist(string path)
		{
			return File.Exists(path);
		}

		public BinaryReader GetReader(string path)
		{
			return new BinaryReader(File.OpenRead(path));
		}

		public BinaryWriter GetWriter(string path)
		{
			return new BinaryWriter(File.OpenWrite(path));
		}
	}
}