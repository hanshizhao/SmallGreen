using SmallGreen.Dto.Base;

namespace SmallGreen.Entity.Basic
{
    public class OperateResult
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool IsSuccess { get; set; }


        public OperateResult()
        {
            IsSuccess = true;
            Message = "操作成功";
        }

        public OperateResult(string message)
        {
            Message = message;
            IsSuccess = false;
        }

        public ApiResponse ToApiResponse()
        {
            return new ApiResponse
            {
                IsSuccess = IsSuccess,
                Message = Message
            };
        }

    }

    public class OperateResult<T> : OperateResult
    {

        public T? Content { get; set; }

        public OperateResult() : base()
        {

        }

        public OperateResult(string message) : base(message)
        {

        }

        public new ApiResponse<T> ToApiResponse()
        {
            return new ApiResponse<T>
            {
                IsSuccess = IsSuccess,
                Message = Message,
                Content = Content
            };
        }

    }
}
