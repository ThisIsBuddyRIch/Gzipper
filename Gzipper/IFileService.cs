using System.IO;

namespace Gzipper
{
	public interface IFileService
	{
		bool IsFileExist(string path);
		BinaryReader GetReader(string path);
		BinaryWriter GetWriter(string path);
	}
}