namespace SmallGreen.Dto.Base
{
    public class ApiResponse
    {
        public string Message { get; set; } = "操作成功";
        public bool IsSuccess { get; set; }

        //public object? Content { get; set; }

        public ApiResponse(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public ApiResponse(bool isSuccess)
        {
            IsSuccess = isSuccess;
            //Content = content;
        }

        public ApiResponse()
        {

        }

        public static ApiResponse Fail(string message)
        {
            return new ApiResponse(message) { IsSuccess = false };
        }

        public static ApiResponse Success()
        {
            return new ApiResponse() { IsSuccess = true };
        }
    }

    public class ApiResponse<T>
    {
        public string Message { get; set; } = "操作成功";
        public bool IsSuccess { get; set; }
        public T? Content { get; set; }

        public ApiResponse(string message)
        {
            IsSuccess = false;
            Message = message;
        }

        public ApiResponse(bool isSuccess, T? content)
        {
            IsSuccess = isSuccess;
            Content = content;
        }

        public ApiResponse()
        {

        }

        public static ApiResponse<T> Fail(string message)
        {
            return new ApiResponse<T>(message);
        }

        public static ApiResponse<T> Success(T? content)
        {
            return new ApiResponse<T>(true, content);
        }
    }
}
