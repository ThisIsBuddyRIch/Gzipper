using System.Linq;

namespace Gzipper.Input
{
	public class InputParser
	{
		private readonly IFileService _fileService;
		private const string Compress = "compress";
		private const string Decompress = "decompress";

		private readonly string[] _operations = {Compress, Decompress};

		public InputParser(IFileService fileService)
		{
			_fileService = fileService;
		}

		public InputModel Parse(string[] args)
		{
			if (args == null || args.Length != 3 || args.Any(string.IsNullOrEmpty))
			{
				return InputModel.Fail("Cmd arguments is not correct. " +
									   $"Put arguments like: Gzipper {string.Join("/", _operations)} " +
									   "input_file_path output_file_path");
			}

			var operationType = GetOperation(args[0]);
			if (operationType == OperationType.None)
			{
				return InputModel.Fail($"This operation type: {operationType} is not supported. " +
									   $"Supported operations {string.Join(",", _operations)}");
			}

			if (!_fileService.IsFileExist(args[1]))
			{
				return InputModel.Fail($"Input file has not found: {args[1]}");
			}

			return InputModel.Success(operationType, args[1], args[2]);
		}

		private static OperationType GetOperation(string operationArg)
		{
			return operationArg switch
			{
				Compress => OperationType.Compress,
				Decompress => OperationType.Decompress,
				_ => OperationType.None
			};
		}
	}
}