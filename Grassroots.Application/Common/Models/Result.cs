namespace Grassroots.Application.Common.Models
{
    public class Result<T>
    {
        public bool Succeeded { get; private set; }
        public T Data { get; private set; }
        public string Error { get; private set; }

        private Result(bool succeeded, T data, string error)
        {
            Succeeded = succeeded;
            Data = data;
            Error = error;
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, data, null);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error);
        }
    }
} 