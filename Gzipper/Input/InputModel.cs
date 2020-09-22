namespace Gzipper.Input
{
	public class InputModel
	{
		private InputModel(OperationType operationType, string? inputFilePath, string? outputFilePath, string errorMessage)
		{
			OperationType = operationType;
			InputFilePath = inputFilePath;
			OutputFilePath = outputFilePath;
			ErrorMessage = errorMessage;
		}

		public OperationType OperationType { get; }

		public string? InputFilePath { get; }

		public string? OutputFilePath { get; }

		public string? ErrorMessage { get; }

		public bool IsFail => !string.IsNullOrEmpty(ErrorMessage);

		public static InputModel Success(OperationType operationType, string inputFilePath, string outputFilePath)
		{
			return new InputModel(operationType, inputFilePath, outputFilePath, null);
		}

		public static InputModel Fail(string error)
		{
			return new InputModel(OperationType.None, null, null, error);
		}
	}
}