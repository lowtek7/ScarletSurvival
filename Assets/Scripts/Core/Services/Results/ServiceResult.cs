using Scarlet.Core.Services.Enums;

namespace Scarlet.Core.Services.Results
{
	public readonly struct ServiceResult<T>
	{
		public bool IsSuccess { get; }
		public T Value { get; }
		public string Error { get; }
		public ServiceError ErrorCode { get; }

		private ServiceResult(bool isSuccess, T value, string error, ServiceError errorCode)
		{
			IsSuccess = isSuccess;
			Value = value;
			Error = error;
			ErrorCode = errorCode;
		}

		public static ServiceResult<T> Success(T value) =>
			new(true, value, null, ServiceError.None);

		public static ServiceResult<T> Failure(string error, ServiceError errorCode = ServiceError.NotInitialized) =>
			new(false, default, error, errorCode);
	}
}
