using System.Linq;

namespace Gzipper.Input
{
	public class InputParser
	{
		private readonly IFileProvider _fileProvider;
		private const string Compress = "compress";
		private const string Decompress = "decomopress";

		private readonly string[] operations = {Compress, Decompress};

		public InputParser(IFileProvider fileProvider)
		{
			_fileProvider = fileProvider;
		}

		public InputModel Parse(string[] args)
		{
			if (args == null || args.Length != 3 || args.Any(string.IsNullOrEmpty))
			{
				return InputModel.Fail("Cmd arguments is not correct. " +
									   $"Put arguments like: Gzipper {string.Join("/", operations)} " +
									   "input_file_path output_file_path");
			}

			var operationType = GetOperation(args[0]);
			if (operationType == OperationType.None)
			{
				return InputModel.Fail($"This operation type: {operationType} is not supported. " +
									   $"Supported operations {string.Join(",", operations)}");
			}

			if (!_fileProvider.IsFileExist(args[1]))
			{
				return InputModel.Fail($"Input file has not found: {args[1]}");
			}

			return InputModel.Success(operationType, args[1], args[2]);
		}

		private OperationType GetOperation(string operationArg)
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