using System.IO;

namespace Gzipper
{
	public interface IFileProvider
	{
		bool IsFileExist(string path);
		BinaryReader GetReader(string path);
		BinaryWriter GetWriter(string path);
	}
}